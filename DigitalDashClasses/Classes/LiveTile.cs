using System;
using System.Linq;
using Microsoft.Phone.Shell;

namespace DigitalDash.Core.Classes
{
    public class LiveTile
    {
        public string Title { get; set; }
        public int Count { get; set; }
        public string BackTitle { get; set; }
        public string BackContent { get; set; }
        public string WideBackContent { get; set; }
        public string SmallBackgroundImage { get; set; }
        public string BackgroundImage { get; set; }
        public string WideBackgroundImage { get; set; }
        public string BackBackgroundImage { get; set; }
        public string WideBackBackgroundImage { get; set; }

        public const int MediumMaxChars = 13;
        public const int LargeMaxChars = 27;

        private readonly Uri _smallImage = new Uri("Assets/ApplicationIcon.png", UriKind.Relative);
        private Uri _mediumImage = new Uri("Assets/Images/mediumtilebg.jpg", UriKind.Relative);
        private Uri _largeImage = new Uri("Assets/Images/largetilebg.jpg", UriKind.Relative);

        private readonly Uri _mediumChargingIcon = new Uri("Assets/ApplicationIconCharging.png", UriKind.Relative);
        private readonly Uri _smallChargingIcon = new Uri("Assets/ApplicationIconChargingSmall.png", UriKind.Relative);
        private readonly Uri _mediumDefaultIcon = new Uri("Assets/ApplicationIcon.png", UriKind.Relative);
        private readonly Uri _smallDefaultIcon = new Uri("Assets/ApplicationIconSmall.png", UriKind.Relative);

        public void Update()
        {
            // find the tile object for the application tile that using "flip" contains string in it.
            var tile = GetTile();

            // Update existing tile...
            if (tile != null)
            {
                var newTile = CreateFlipTileData();

                tile.Update(newTile);

                // Update the main tile with the battery count...
                var mainTile = ShellTile.ActiveTiles.First();
                if (mainTile != null)
                {
                    var liveTile = new IconicTileData();
                    liveTile.Count = this.Count;
                    liveTile.IconImage = this.Count > 0 ? _mediumChargingIcon : _mediumDefaultIcon;
                    liveTile.SmallIconImage = this.Count > 0 ? _smallChargingIcon : _smallDefaultIcon;
                    mainTile.Update(liveTile);
                }
            }
        }

        public static ShellTile GetTile()
        {
            return ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("flip".ToString()));
        }

        public static void Update(string line1, string line2, string line3, Battery battery, AppSettings appSettings)
        {
            const string contentFormat = "{1}{0}{2}{0}{3}";

            line1 = line1.IsNullOrEmpty() ? "DigitalDash" : line1;
            line2 = line2.IsNullOrEmpty() ? string.Empty : line2;
            line3 = line3.IsNullOrEmpty() ? string.Empty : line3;

            var liveTile = new LiveTile();

            liveTile.Title = "DigitalDash | " + System.DateTime.Now.ToString("h:mmtt");
            liveTile.BackContent = string.Format(contentFormat, Environment.NewLine, line1.Trim(LiveTile.MediumMaxChars), line2.Trim(LiveTile.MediumMaxChars), line3.Trim(LiveTile.MediumMaxChars));
            liveTile.WideBackContent = string.Format(contentFormat, Environment.NewLine, line1.Trim(LiveTile.LargeMaxChars), line2.Trim(LiveTile.LargeMaxChars), line3.Trim(LiveTile.LargeMaxChars));
            liveTile.Count = battery.GetBatteryStatus(appSettings).RemainingChargePercentTested;
            liveTile.Update();
        }

        public static bool Exists()
        {
            return GetTile() != null;
        }

        public ShellTileData CreateFlipTileData()
        {
            var appSettings = new AppSettings();
            var battery = new Battery();

            if(!appSettings.BackgroundImageFileLocation.IsNullOrEmpty())
            {
                _mediumImage = new Uri(Photos.MediumLiveTileBackgroundImageFileLocation(), UriKind.Absolute);
                _largeImage = new Uri(Photos.LargeLiveTileBackgroundImageFileLocation(), UriKind.Absolute);
            }

            return new FlipTileData()
            {
                SmallBackgroundImage = _smallImage,
                BackgroundImage = _mediumImage,
                WideBackgroundImage = _largeImage,
                Title = this.Title,
                BackTitle = this.BackTitle,
                BackContent = this.BackContent,
                WideBackContent = this.WideBackContent,
                Count = battery.GetBatteryStatus(appSettings).RemainingChargePercentTested,
                BackBackgroundImage = null,
                WideBackBackgroundImage = null
            };
        }
    }
}
