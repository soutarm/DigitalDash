using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using ExifLib;
using Microsoft.Xna.Framework.Media;

namespace DigitalDash.Core.Classes
{
    public class Photos
    {
        private readonly Storyboard _storyboard;

        public int ViewportHeight;
        public int ViewportWidth;
        public const double ActualHeight = 850;
        public const double ActualWidth = 480;


        public static string DefaultBackgroundImageName = "digitaldash_bg.jpg";
        //public static string DefaultTileImageNameSmall = "/Shared/ShellContent/digitaldash_bg_Small.jpg";
        public static string DefaultTileImageNameMedium = "/Shared/ShellContent/digitaldash_bg_Medium.jpg";
        public static string DefaultTileImageNameLarge = "/Shared/ShellContent/digitaldash_bg_Large.jpg";

        //public static int SmallTileWidth = 159;
        //public static int SmallTileHeight = 159;
        public static int MediumTileWidth = 672; // double 336 for hi res screens;
        public static int MediumTileHeight = 672; // double 336 for hi res screens;
        public static int LargeTileHeight = 672; // double 336;
        public static int LargeTileWidth = 1384; // double 692;

        public Photos()
        {
            _storyboard = new Storyboard();
        }

        public static string MediumLiveTileBackgroundImageFileLocation()
        {
            return BackgroundImageFileLocation(DefaultTileImageNameMedium);
        }
        public static string LargeLiveTileBackgroundImageFileLocation()
        {
            return BackgroundImageFileLocation(DefaultTileImageNameLarge);
        }

        private static string BackgroundImageFileLocation(string fileName)
        {
            //return string.Format("{0}{1}", new AppSettings().BackgroundImageFileLocation, fileName);
            return string.Format("isostore:{0}", fileName);
        }

        /// <summary>
        /// Removes the current background image from isolated storage
        /// </summary>
        public static void DeleteExistingBackground()
        {
            using (var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(DefaultBackgroundImageName)) myIsolatedStorage.DeleteFile(DefaultBackgroundImageName);
                //if (myIsolatedStorage.FileExists(DefaultTileImageNameSmall)) myIsolatedStorage.DeleteFile(DefaultTileImageNameSmall);
                if (myIsolatedStorage.FileExists(DefaultTileImageNameMedium)) myIsolatedStorage.DeleteFile(DefaultTileImageNameMedium);
                if (myIsolatedStorage.FileExists(DefaultTileImageNameLarge)) myIsolatedStorage.DeleteFile(DefaultTileImageNameLarge);
            }

            new AppSettings().BackgroundImageFileLocation = string.Empty;
        }

        /// <summary>
        /// Takes the input writeable bitmap and saves it to writeable storage for use as our wallpaper
        /// </summary>
        /// <param name="stream"> </param>
        /// <returns></returns>
        public static bool SaveBackgroundImage(System.IO.Stream stream)
        {
            try
            {
                var scaleFactor = (float)Application.Current.Host.Content.ScaleFactor / 100;
                var viewportHeight = (int)(Application.Current.Host.Content.ActualHeight * scaleFactor);

                var rotation = 0;
            
                //read exif data
                stream.Seek(0, SeekOrigin.Begin);
                var info = ExifReader.ReadJpeg(stream, "");

                if (info.Orientation == ExifOrientation.TopRight)
                {
                    rotation = 90;
                }
                if (info.Orientation == ExifOrientation.BottomRight)
                {
                    rotation = 180;
                }
                if (info.Orientation == ExifOrientation.BottomLeft)
                {
                    rotation = -90;
                }


                var bmp = new BitmapImage();
                bmp.CreateOptions = BitmapCreateOptions.None;
                bmp.SetSource(rotation != 0 ? Utilities.RotateStream(stream, rotation) : stream);
                var wb = new WriteableBitmap(bmp);

                // Create a filename for JPEG file in isolated storage.
                var imageName = DefaultBackgroundImageName;

                DeleteExistingBackground();

                // Create virtual store and file stream. Check for duplicate tempJPEG files.
                using (var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    var fileStream = myIsolatedStorage.CreateFile(imageName);

                    new AppSettings().BackgroundImageFileLocation = fileStream.Name.Replace("file:///", string.Empty).Replace(imageName, string.Empty);

                    // Always save the image as our viewport height and calculate the width based on that.
                    var scaleRatio = (double)viewportHeight / (double)wb.PixelHeight;
                    var imageHeight = viewportHeight;
                    var imageWidth = (int)(wb.PixelWidth * scaleRatio);

                    wb.SaveJpeg(fileStream, imageWidth, imageHeight, 0, 85);

                    fileStream.Close();

                    // Save the two live tile images...
                    //if (LiveTile.Exists())
                    //{
                        var mediumStream = myIsolatedStorage.CreateFile(DefaultTileImageNameMedium);
                        ResizeCropAndSave(mediumStream, bmp, MediumTileHeight, MediumTileWidth);
                        mediumStream.Close();

                        var largeStream = myIsolatedStorage.CreateFile(DefaultTileImageNameLarge);
                        ResizeCropAndSave(largeStream, bmp, LargeTileHeight, LargeTileWidth);
                        largeStream.Close();
                    //}
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static void ResizeCropAndSave(Stream fileStream, BitmapImage bmp, int maxHeight, int maxWidth)
        {

            var wb = new WriteableBitmap(bmp);

            double scaleRatio;
            int resizeWidth;
            int resizeHeight;

            var cropWidth = 0;
            var cropHeight = 0;

            // Scale the image down to fit within the maximum limits
            if (wb.PixelWidth < wb.PixelHeight || maxWidth > maxHeight)
            {
                scaleRatio = maxWidth / (double)wb.PixelWidth; 
                resizeWidth = maxWidth;
                resizeHeight = (int) (wb.PixelHeight*scaleRatio);
                cropHeight = (int)((resizeHeight - maxHeight) * 0.25);
            }
            else
            {
                scaleRatio = maxHeight / (double)wb.PixelHeight;
                resizeWidth = (int)(wb.PixelWidth * scaleRatio);
                resizeHeight = maxHeight;
                cropWidth = (int)((resizeWidth - maxWidth) * 0.5);
            }

            // Scale the image down to our max height.
            var resized = wb.Resize(resizeWidth, resizeHeight, WriteableBitmapExtensions.Interpolation.Bilinear);

            var cropped = resized.Crop(cropWidth, cropHeight, maxWidth, maxHeight);
            cropped.SaveJpeg(fileStream, maxWidth, maxHeight, 0, 85);
        }

        /// <summary>
        /// If the user has selected an album to view this will grab a random photo from that album and set it as our background.
        /// </summary>
        public bool RefreshAlbumPhoto()
        {
            var appSettings = new AppSettings();
            if (appSettings.PhotoAlbumNameSetting != string.Empty)
            {
                var mediaLib = new MediaLibrary();
                var picAlbum = mediaLib.RootPictureAlbum.Albums.SingleOrDefault(alb => alb.Name == appSettings.PhotoAlbumNameSetting);

                if (picAlbum == null || !picAlbum.Pictures.Any())
                {
                    // No use having an empty album as our source
                    appSettings.PhotoAlbumNameSetting = string.Empty;
                }
                else
                {
                    // If we're not animating the background only grab portriat oriented photos
                    var pics = appSettings.AnimatePhotosSetting
                                   ? picAlbum.Pictures.ToList()
                                   : picAlbum.Pictures.Where(p => p.Height > p.Width).ToList();

                    if (pics.Any())
                    {
                        var numberOfPics = pics.Count();
                        var randy = new Random();
                        var randyInt = randy.Next(0, numberOfPics - 1);

                        var nextPic = pics[randyInt].GetImage();
                        
                        if (Photos.SaveBackgroundImage(nextPic))
                        {
                            return true;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public BitmapImage LoadBackgroundImage()
        {
            // Load our bitmap from isolated storage
            using (var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(Photos.DefaultBackgroundImageName))
                {
                    var bmp = new BitmapImage();
                    using (var fileStream = myIsolatedStorage.OpenFile(Photos.DefaultBackgroundImageName, FileMode.Open, FileAccess.Read))
                    {
                        bmp.SetSource(fileStream);
                    }

                    return bmp;
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the current background image as the source of the input Image and sets the width of the StackPanel and image within
        /// </summary>
        /// <param name="targetImage"></param>
        /// <param name="imagePanel"></param>
        /// <param name="isMainPanel"> </param>
        public void SetBackgroundImage(Image targetImage, StackPanel imagePanel, bool isMainPanel)
        {
            var bmp = LoadBackgroundImage();
            if (bmp == null || targetImage == null || imagePanel == null || ViewportHeight <= 0 || ViewportWidth <= 0) return;

            targetImage.HorizontalAlignment = HorizontalAlignment.Left;
            targetImage.Stretch = Stretch.Uniform;
            targetImage.Source = bmp;

            // If animation is disabled only portrait oriented photos will come through
            if (bmp.PixelWidth < bmp.PixelHeight)
            {
                SetImageToFit(targetImage, imagePanel, bmp);
            }
            else
            {
                //var imageAnimateWidth = bmp.PixelWidth * scaleRatio;

                targetImage.Width = (int) bmp.PixelWidth;
                imagePanel.Width = (int) bmp.PixelWidth;

                if (isMainPanel && bmp.PixelWidth > ViewportWidth)
                {
                    var doubleHeight = (double) bmp.PixelHeight;
                    var scaleRatio = doubleHeight / ActualHeight;

                    var widthDiff = (bmp.PixelWidth - (ViewportWidth*scaleRatio))*-1;
                    AnimateBackground(widthDiff, targetImage);
                }
                //else
                //{
                //    //targetImage.Width = ViewportWidth;
                //    //targetImage.Height = ViewportHeight;
                //    //targetImage.Stretch = Stretch.UniformToFill;
                //    //imagePanel.Width = ViewportWidth;
                //    //imagePanel.Height = actualHeight;
                //    SetImageToFit(targetImage, imagePanel, bmp);
                //}
            }
        }

        private static void SetImageToFit(Image targetImage, StackPanel imagePanel, BitmapImage bmp)
        {
            imagePanel.Width = ActualWidth;
            imagePanel.Height = ActualHeight;


            //if (bmp.PixelWidth > bmp.PixelHeight)
            //{
            //    imagePanel.Width = bmp.PixelWidth;
            //    targetImage.Width = imagePanel.ActualWidth;
            //    targetImage.Height = double.NaN;
            //}
            //else
            //{
                //imagePanel.Height = bmp.PixelHeight;
                targetImage.Width = double.NaN;
                targetImage.Height = ActualHeight;
                targetImage.Stretch = Stretch.UniformToFill;
                //targetImage.Stretch = Stretch.Uniform;
                targetImage.HorizontalAlignment = HorizontalAlignment.Center;
            //}
        }

        #region Animation


        public void StopAnimation()
        {
            if (_storyboard.GetCurrentState() == ClockState.Active)
            {
                _storyboard.Stop();
            }
        }

        public void StopAndClearAnimation()
        {
            this.StopAnimation();

            if (_storyboard.Children.Any()) _storyboard.Children.Clear();
        }

        public void StartAnimation()
        {
            _storyboard.Begin();
        }
        
        public void AnimateBackground(double widthDiff, Image backgroundImage)
        {
            if (widthDiff > 0 || widthDiff > -10) return;

            // Clear out any existing keyframes
            StopAndClearAnimation();

            backgroundImage.RenderTransform = new CompositeTransform();

            var easing = new QuadraticEase { EasingMode = EasingMode.EaseInOut };

            // create the timeline
            var animation = new DoubleAnimationUsingKeyFrames();
            animation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.Zero, Value = 0.0, EasingFunction = easing });
            animation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(40000), Value = widthDiff, EasingFunction = easing });
            animation.AutoReverse = true;

            // notice the first parameter takes a timeline object not the storyboard itself
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            Storyboard.SetTarget(animation, backgroundImage);

            _storyboard.RepeatBehavior = RepeatBehavior.Forever;

            // Have to do a stop() here otherwise adding children chucks an exception
            _storyboard.Stop();
            _storyboard.Children.Add(animation);

            // start the animation
            if (new AppSettings().AnimatePhotosSetting) _storyboard.Begin();
        }

        #endregion

    }
}
