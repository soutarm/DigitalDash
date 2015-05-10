using System;
using Microsoft.Phone.Shell;

namespace DigitalDash.UI.Classes
{
    public class LiveTile
    {

        public static void Create()
        {
            // Make sure we don't add another tile!
            if (DigitalDash.Core.Classes.LiveTile.GetTile() == null)
            {
                // Create a new tile!
                var tileUri = new Uri("/Pages/MainPage.xaml?tile=flip", UriKind.Relative);
                var liveTile = new DigitalDash.Core.Classes.LiveTile();
                var tileData = liveTile.CreateFlipTileData();
                ShellTile.Create(tileUri, tileData, true);

                var ourTile = new DigitalDash.Core.Classes.LiveTile();
                ourTile.Update();
            }
       }
       
        public static void Remove()
        {
            var tile = DigitalDash.Core.Classes.LiveTile.GetTile();

            if (tile != null)
            {
                tile.Delete();
            }

            new ScheduledBackgroundAgent().RemoveAgent();
        }
    }
}
