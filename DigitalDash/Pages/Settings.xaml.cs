using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace DigitalDash.Pages
{
    public partial class Settings
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void ContactDevClick(object sender, RoutedEventArgs e)
        {
            var emailComposeTask = new EmailComposeTask {To = "soutarm@gmail.com", Body = "", Subject = "DigitalDash Feedback"};
            emailComposeTask.Show();
        }

        private void ChoosePhotoClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}