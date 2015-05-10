using System.Windows;

namespace DigitalDash.Core.Classes
{
    public class AnimatedGifs
    {
        public static string[] AnimatedWallpapers = new string[]
            {
                "http://www.cell11.com/images/screensavers/73-Cell11.com.gif",
                "http://www.cell11.com/images/screensavers/94-Cell11.com.gif",
                "http://www.cell11.com/images/screensavers/238-Cell11.com.gif",
                "http://www.cell11.com/images/screensavers/123-Cell11.com.gif",
                "http://www.cell11.com/images/screensavers/34-Cell11.com.gif",
                "http://www.cell11.com/images/screensavers/54-Cell11.com.gif",
                "http://24.media.tumblr.com/318f2b781705db067f74b5768c8a5c35/tumblr_mjpt7kCE7U1qgcra2o1_500.gif"
            };

        /// <summary>
        /// Replaces the markup in the browser control with an IMG tag containing a URL
        /// </summary>
        /// <param name="appSettings"></param>
        /// <param name="viewportWidth"></param>
        /// <param name="viewportHeight"></param>
        public string GetAnimatedGifHtml(AppSettings appSettings, int viewportWidth, int viewportHeight)
        {
            var backgroundColor = IsThemeDark() ? "#000" : "#FFF";

            var imageUrl = AnimatedWallpapers[appSettings.CurrentAnimatedGifSetting];

            return string.Format("<html><head><meta name='viewport' content='width={0}' id='viewport' /></head><body style='background:{2};margin:0'><img style='height:{1}px' src='{3}' /></body></html>", viewportWidth, viewportHeight, backgroundColor, imageUrl);
        }

        /// <summary>
        /// Cycles the appsetting for current animated GIF to the next image
        /// </summary>
        /// <param name="appSettings"></param>
        public void CycleAnimatedGif(AppSettings appSettings)
        {
            var currentAnimatedIndex = appSettings.CurrentAnimatedGifSetting;
            if (appSettings.EnableAnimatedGifSetting)
            {
                currentAnimatedIndex = currentAnimatedIndex + 1;
                if (currentAnimatedIndex >= AnimatedGifs.AnimatedWallpapers.Length) currentAnimatedIndex = 0;

                appSettings.CurrentAnimatedGifSetting = currentAnimatedIndex;
            }
        }

        private static bool IsThemeDark()
        {
            return ((Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible);
        }
    }
}
