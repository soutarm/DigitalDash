using System;
using System.Windows.Navigation;

namespace DigitalDash.UI.Classes
{
    public class CustomUriMapper : UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            string tempUri = uri.ToString();

            // Launch from the rich media Open link.
            // Incoming URI example: /MainPage.xaml?Action=RichMediaEdit&token=%7Bed8b7de8-6cf9-454e-afe4-abb60ef75160%7D
            if (!(tempUri.Contains("RichMediaEdit")) && (tempUri.Contains("token")))
            {
                // Redirect to RichMediaPage.xaml.
                string mappedUri = tempUri.Replace("MainPage", "SettingsPano");
                return new Uri(mappedUri, UriKind.Relative);
            }

            // Otherwise perform normal launch.
            return uri;
        }
    }
}
