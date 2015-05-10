//#define DEBUG_AGENT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using DigitalDash.Core.Classes;
using DigitalDash.UI.Classes;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;

namespace DigitalDash.Pages
{
    public partial class SettingsPano
    {
        private readonly PhotoChooserTask _photoChooserTask;
        private readonly ScheduledBackgroundAgent _backgroundAgent;
        private readonly AppSettings _appSettings;
        private readonly Photos _photos;

        public SettingsPano()
        {
            InitializeComponent();

            SetAboutText();

            _photoChooserTask = new PhotoChooserTask();
            _backgroundAgent = new ScheduledBackgroundAgent();
            _appSettings = new AppSettings();
            _photos = new Photos();

            // Need to set this up here as when the chooser opens we navigate away
            _photoChooserTask.Completed += PhotoChooserTaskCompleted;

            var savedImage = _photos.LoadBackgroundImage();
            if(savedImage != null) CurrentBackground.Source = savedImage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            // Get a dictionary of query string keys and values.
            IDictionary<string, string> queryStrings = this.NavigationContext.QueryString;

            // Ensure that there is at least one key in the query string, and check whether the "token" key is present.
            if (queryStrings.ContainsKey("token"))
            {
                // Retrieve the photo from the media library using the token passed to the app.
                var library = new MediaLibrary();
                var photoFromLibrary = library.GetPictureFromToken(queryStrings["token"]);
                if (Photos.SaveBackgroundImage(photoFromLibrary.GetImage()))
                {
                    GoHome();
                }
                else
                {
                    MessageBox.Show("Sorry, unable to use that photo. Please try another one.");
                }
            }

            if (queryStrings.ContainsKey("goto"))
            {
                SettingsPanorama.DefaultItem = SettingsPanorama.Items[Convert.ToInt32(queryStrings["goto"])];
            }

            EnableAnimatedGifSetting.IsChecked = _appSettings.EnableAnimatedGifSetting;

            BackgroundAgentToggle.IsChecked = _backgroundAgent.IsRunning();
            AgentStatus.Text = string.Format("Keeps the live tile content up to date. Last run: {0}", _backgroundAgent.LastRun());


            base.OnNavigatedTo(e);
        }

        private void PhotoChooserTaskCompleted(object sender, PhotoResult e)
        {
            if (e.TaskResult != TaskResult.OK) return;

            // Clear out the setting for albums so we can use this specific image.
            _appSettings.PhotoAlbumNameSetting = string.Empty;

            if (Photos.SaveBackgroundImage(e.ChosenPhoto))
            {
                GoHome();
            }
            else
            {
                MessageBox.Show("Sorry, unable to use that photo. Please try another one.");
            }
        }

        private void BackgroundAgentToggleChecked(object sender, RoutedEventArgs e)
        {
            _backgroundAgent.StartPeriodicAgent();
        }

        private void BackgroundAgentToggleUnchecked(object sender, RoutedEventArgs e)
        {
            _backgroundAgent.RemoveAgent();
        }

        private void ContactDevClick(object sender, RoutedEventArgs e)
        {
            var emailComposeTask = new EmailComposeTask { To = "soutarm@gmail.com", Body = "", Subject = "DigitalDash Feedback" };
            emailComposeTask.Show();
        }

        private void SingleImageClick(object sender, RoutedEventArgs e)
        {
            _photoChooserTask.Show();
        }

        private void CancelPickAlbumClick(object sender, RoutedEventArgs e)
        {
            PickAlbumPanel.Visibility = Visibility.Collapsed;
        }

        private void ImageGalleryClick(object sender, RoutedEventArgs e)
        {
            //const string locationFormat = "C:\\Data\\Users\\Public\\Pictures\\{0}\\{1}";
            const string countFormat = "{0} pictures";

            var mediaLib = new MediaLibrary();
            var picAlbums = mediaLib.RootPictureAlbum.Albums.Where(alb => alb.Pictures.Any());
            AvailableAlbums.ItemsSource = picAlbums.Select(alb => new{Name = alb.Name, Count = string.Format(countFormat, alb.Pictures.Count)}).ToList();

            PickAlbumPanel.Visibility = Visibility.Visible;
        }

        private void ResetBackgroundClick(object sender, RoutedEventArgs e)
        {
            _appSettings.PhotoAlbumNameSetting = string.Empty;
            Photos.DeleteExistingBackground();

            GoHome();
        }

        private void CreditLinkTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var task = new WebBrowserTask();
            task.Uri = new Uri("http://ipapun.deviantart.com/art/Aurora-Deep-Flow-139274877", UriKind.Absolute);
            task.Show();
        }
        private void ShowError(string errorMessage)
        {
            ErrorMessage.Text = errorMessage;
            ErrorMessage.Visibility = Visibility.Visible;
        }

        private void GoHome()
        {
            NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
        }

        private void AlbumTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pickedAlbum = (StackPanel)sender;
            var albumName = string.Empty;
            foreach (var child in pickedAlbum.Children.OfType<TextBlock>())
            {
                child.Foreground = new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
                if (child.Name == "AlbumName") albumName = child.Text;
            }

            try
            {
                var mediaLib = new MediaLibrary();
                var picAlbum = mediaLib.RootPictureAlbum.Albums.SingleOrDefault(alb => alb.Name == albumName);

                if (picAlbum == null || !picAlbum.Pictures.Any())
                {
                    MessageBox.Show("Sorry, unable to retrieve photos from that album.") ;
                }
                else
                {
                    _appSettings.PhotoAlbumNameSetting = albumName;
                    
                    // Need to async this bitch
                    _photos.RefreshAlbumPhoto();
                    
                    GoHome();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        private void EnableAnimatedGifSettingClick(object sender, RoutedEventArgs e)
        {
            _appSettings.EnableAnimatedGifSetting = !_appSettings.EnableAnimatedGifSetting;

            if (_appSettings.EnableAnimatedGifSetting)
            {
                new AnimatedGifs().CycleAnimatedGif(_appSettings);
            }
        }

        private void RateAppClick(object sender, RoutedEventArgs e)
        {
            new MarketplaceReviewTask().Show();
        }

        public void SetAboutText()
        {
            var aboutText = "It all started with a simple wish, a wish that the Lock Screen wouldn't go" + 
                " dark after 10 seconds so I could use it as a digital photo viewer and dashboard. " +
                Environment.NewLine + Environment.NewLine + "I use DigitalDash every day and have some cool ideas on how to improve it. " +
                Environment.NewLine + Environment.NewLine + "I made this for myself but I hope you like it too!" +
                Environment.NewLine + Environment.NewLine + "Michael Soutar" +
                Environment.NewLine + Environment.NewLine + "twitter.com/mrated";

            AboutText.Text = aboutText;
        }
    }
}