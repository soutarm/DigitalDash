using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DigitalDash.Core.Classes;
using DigitalDash.UI.Classes;
using Microsoft.Phone.Marketplace;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.UserData;

namespace DigitalDash.Pages
{
    public partial class MainPage
    {

        private readonly AppSettings _appSettings;
        private readonly DispatcherTimer _timer;
        private DateTime _timePhotoUpdated;
        private readonly Battery _battery;
        private readonly Photos _photos;
        private readonly AnimatedGifs _animatedGifs;
        private readonly ScheduledBackgroundAgent _scheduledBackgroundAgent;

        private readonly float _scaleFactor;
        private readonly int _viewportHeight;
        private readonly int _viewportWidth;

        private bool _settingsMode;
        //private Accelerometer _accelerometer;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _appSettings = new AppSettings();
            _timer = new DispatcherTimer();
            _battery = new Battery();
            //_accelerometer = new Accelerometer();
            _animatedGifs = new AnimatedGifs();
            _scheduledBackgroundAgent = new ScheduledBackgroundAgent();

            _scaleFactor = (float) Application.Current.Host.Content.ScaleFactor/100;
            _viewportHeight = (int) (Application.Current.Host.Content.ActualHeight*_scaleFactor);
            _viewportWidth = (int) (Application.Current.Host.Content.ActualWidth*_scaleFactor);

            ImagePanel.Height = _viewportHeight;
            BackgroundImage.Height = _viewportHeight;
            ImagePanelTemp.Height = _viewportHeight;
            BackgroundImageTemp.Height = _viewportHeight;

            _photos = new Photos
                          {
                              ViewportHeight = _viewportHeight,
                              ViewportWidth = _viewportWidth
                          };

            var phoneAppService = PhoneApplicationService.Current;
            phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            // Update every second for items that change frequently
            _timer.Interval = new TimeSpan(0, 0, 0, 1);
            _timer.Tick += TimerTick;
        }

        
        //Code for initialization, capture completed, image availability events; also setting the source for the viewfinder.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                if (_appSettings.EnableAnimatedGifSetting)
                {
                    DisplayAnimatedGif();
                }
                else
                {
                    LoadBackgroundImage();
                }

                if (!_appSettings.AnimatePhotosSetting)
                {
                    _photos.StopAndClearAnimation();
                    // Instantiate the Accelerometer.
                    //try
                    //{
                    //    _accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(500);
                    //    _accelerometer.CurrentValueChanged += AccelerometerCurrentValueChanged;
                    //    //_accelerometer.Start();
                    //}
                    //catch (Exception ex)
                    //{
                    //    DisplayError(ex);
                    //}
                }
                else
                {
                    _photos.StartAnimation();
                }

                if (_appSettings.FirstTimeSetting)
                {
                    HelpPanel.Visibility = Visibility.Visible;
                    ApplicationBar.Mode = ApplicationBarMode.Default;
                    _appSettings.FirstTimeSetting = false;
                }

                BrowserBg.Visibility = _appSettings.EnableAnimatedGifSetting ? Visibility.Visible : Visibility.Collapsed;
                BatteryPanel.Visibility = _appSettings.ShowBatterySetting ? Visibility.Visible : Visibility.Collapsed;
                ClockDisplay.Visibility = _appSettings.ShowClockSetting ? Visibility.Visible : Visibility.Collapsed;
                DateDisplay.Visibility = _appSettings.ShowDateSetting ? Visibility.Visible : Visibility.Collapsed;
                DayDisplay.Visibility = _appSettings.ShowDateSetting ? Visibility.Visible : Visibility.Collapsed;
                CalendarPanel.Visibility = _appSettings.ShowCalendarSetting ? Visibility.Visible : Visibility.Collapsed;

                SetSettingsDisplay();

                SetTheme(false);

                SetPinIcon();

                RefreshScreen(false);

                _timePhotoUpdated = System.DateTime.Now;
                _timer.Start();

                CheckLicense();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
                Photos.DeleteExistingBackground();
            }
        }


        //protected void AccelerometerCurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        //{
        //    //_photos.StopAnimation();
        //    if (!_appSettings.AnimatePhotosSetting)
        //    {
        //        var transform = new CompositeTransform();
        //        var acceleration = e.SensorReading.Acceleration;

        //        // If user tilts the phone with 0.35 in X axis, Handle/Change the pivot index
        //        if (acceleration.X <= -0.35)
        //        {
        //            transform.TranslateX = acceleration.X * -10;
        //        }
        //        if (acceleration.X >= 0.35)
        //        {
        //            transform.TranslateX = acceleration.X * 10;
        //        }
        //        BackgroundImage.RenderTransform = transform;
        //    }
        //}


        private void LoadBackgroundImage()
        {
            // Load our bitmap from isolated storage
            try
            {
                _photos.SetBackgroundImage(BackgroundImage, ImagePanel, true);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            _timer.Stop();

            _photos.StopAnimation();

            _settingsMode = false;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            // Update low frequency stuff every minute, the rest can be done every second.
            RefreshScreen(System.DateTime.Now.Second != 0);

            // Update background image every 5 minutes if the user has selected an album
            if (!_appSettings.PhotoAlbumNameSetting.IsNullOrEmpty() &&
                Utilities.IsGoodDivision(System.DateTime.Now.Minute - _timePhotoUpdated.Minute, _appSettings.PhotoTimeoutSetting)
                && System.DateTime.Now.Second == 0)
            {
                UpdatePhoto();
                _timePhotoUpdated = System.DateTime.Now;
            }
        }

        private void RefreshScreen(bool isShortTime)
        {
            var dt = DateTime.Now;

            var timeFormatString = _appSettings.Use12HourClockSetting ? "h:mm" : "H:mm";

            ClockDisplay.Text = dt.ToString(timeFormatString);
            DayDisplay.Text = dt.DayOfWeek.ToString();
            DateDisplay.Text = string.Format("{0:MMMM} {1}", dt, dt.Day);
            if (_appSettings.ShowBatterySetting) RefreshBatteryIcon();

            if (!isShortTime)
            {
                if (_appSettings.ShowCalendarSetting) RefreshCalendar();
            }
        }

        private void RefreshBatteryIcon()
        {
            if (!_appSettings.ShowBatterySetting || _battery == null) return;

            var batteryStatus = _battery.GetBatteryStatus(_appSettings);
            //BatteryCharging.Visibility = batteryStatus.IsCharging ? Visibility.Visible : Visibility.Collapsed;
            //BatteryIcon.Width = (int) (batteryStatus.RemainingChargePercent*0.25);
            BatteryPercent.Text = batteryStatus.Text;
        }

        private void RefreshCalendar()
        {
            var appts = new Appointments();

            //Identify the method that runs after the asynchronous search completes.
            appts.SearchCompleted += AppointmentsSearchCompleted;

            var start = DateTime.Now;
            var end = start.AddDays(7);
            const int max = 20;

            //Start the asynchronous search.
            appts.SearchAsync(start, end, max, "DigitalDash Calendar Search");
        }

        protected void AppointmentsSearchCompleted(object sender, AppointmentsSearchEventArgs e)
        {
            var appointment = Calendar.GetAppointment(e);
            if (appointment == null) return;
            CalendarTitle.Text = appointment.Subject;
            CalendarLocation.Text = appointment.Location;
            CalendarTime.Text = appointment.DateAndTime;

            // Update the live tile(s)
            if (DigitalDash.Core.Classes.LiveTile.Exists())
            {
                DigitalDash.Core.Classes.LiveTile.Update(appointment.Subject, appointment.Location, appointment.DateAndTime, _battery, _appSettings);
            }
        }

        private void DisplayError(Exception ex)
        {
            DebugText.Text = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        private void BatteryPercentTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_settingsMode)
            {
                _appSettings.ShowBatterySetting = !_appSettings.ShowBatterySetting;
                BatteryPanel.Opacity = _appSettings.ShowBatterySetting ? 1 : 0.5;
            }
            else
            {
                _appSettings.ShowBatteryPercentSetting = !_appSettings.ShowBatteryPercentSetting;
                RefreshBatteryIcon();
            }
        }

        private void CalendarPanelTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_settingsMode)
            {
                _appSettings.ShowCalendarSetting = !_appSettings.ShowCalendarSetting;
                CalendarPanel.Opacity = _appSettings.ShowCalendarSetting ? 1 : 0.5;
            }
        }

        private void DateTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_settingsMode)
            {
                _appSettings.ShowDateSetting = !_appSettings.ShowDateSetting;
                DateDisplay.Opacity = _appSettings.ShowDateSetting ? 1 : 0.5;
                DayDisplay.Opacity = _appSettings.ShowDateSetting ? 1 : 0.5;
            }
        }

        private void ClockTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_settingsMode)
            {
                _appSettings.ShowClockSetting = !_appSettings.ShowClockSetting;
                ClockDisplay.Opacity = _appSettings.ShowClockSetting ? 1 : 0.5;
            }
            else
            {
                this.SetTheme(true);
            }
        }

        protected void SetTheme(bool toggleTheme)
        {
            if ((toggleTheme && _appSettings.CurrentThemeSetting == "SimpleTop") || (!toggleTheme && _appSettings.CurrentThemeSetting != "SimpleTop"))
            {
                ClockAndDatePanel.Style = (Style)this.Resources["DefaultClockPanel"];
                ClockDisplay.Style = (Style)this.Resources["DefaultClock"];
                DayDisplay.Style = (Style)this.Resources["DefaultDay"];
                DateDisplay.Style = (Style)this.Resources["DefaultDate"];

                CalendarPanel.Style = (Style)this.Resources["DefaultCalendarPanel"];
                CalendarTitle.Style = (Style)this.Resources["DefaultCalendarTitle"];
                CalendarLocation.Style = (Style)this.Resources["DefaultCalendarLocation"];
                CalendarTime.Style = (Style)this.Resources["DefaultCalendarTime"];

                _appSettings.CurrentThemeSetting = "Default";
            }
            else if ((toggleTheme && _appSettings.CurrentThemeSetting == "Default") || (!toggleTheme && _appSettings.CurrentThemeSetting != "Default"))
            {
                ClockAndDatePanel.Style = (Style)this.Resources["SimpleTopClockPanel"];
                ClockDisplay.Style = (Style)this.Resources["SimpleTopClock"];
                DayDisplay.Style = (Style)this.Resources["SimpleTopDay"];
                DateDisplay.Style = (Style)this.Resources["SimpleTopDate"];

                CalendarPanel.Style = (Style)this.Resources["SimpleTopCalendarPanel"];
                CalendarTitle.Style = (Style)this.Resources["SimpleTopCalendarTitle"];
                CalendarLocation.Style = (Style)this.Resources["SimpleTopCalendarLocation"];
                CalendarTime.Style = (Style)this.Resources["SimpleTopCalendarTime"];

                _appSettings.CurrentThemeSetting = "SimpleTop";
            }
        }

        protected void UpdatePhoto()
        {
            _photos.RefreshAlbumPhoto();
            _photos.SetBackgroundImage(BackgroundImageTemp, ImagePanelTemp, false);
            ImagePanelTemp.Visibility = Visibility.Visible;
            FadeImageIn.Begin();

            if (DigitalDash.Core.Classes.LiveTile.Exists()) UpdateLiveTile();
        }

        private void DisplayAnimatedGif()
        {
            BrowserBg.NavigateToString(_animatedGifs.GetAnimatedGifHtml(_appSettings, _viewportWidth, _viewportHeight));
        }

        /// <summary>
        /// Navigate to the settings page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsButtonClick(object sender, EventArgs e)
        {
            const string defaultCalendarText = "Calendar Item";

            _settingsMode = !_settingsMode;

            SetSettingsDisplay();

            if (_settingsMode)
            {
                ApplicationBar.Mode = ApplicationBarMode.Default;
                RefreshBatteryIcon();
                RefreshCalendar();
                BatteryPanel.Opacity = _appSettings.ShowBatterySetting ? 1 : 0.5;
                CalendarPanel.Opacity = _appSettings.ShowCalendarSetting ? 1 : 0.5;
                ClockDisplay.Opacity = _appSettings.ShowClockSetting ? 1 : 0.5;
                DateDisplay.Opacity = _appSettings.ShowDateSetting ? 1 : 0.5;
                DayDisplay.Opacity = _appSettings.ShowDateSetting ? 1 : 0.5;

                ClockAndDatePanel.Visibility = Visibility.Visible;

                BatteryPanel.Visibility = Visibility.Visible;
                CalendarPanel.Visibility = Visibility.Visible;
                ClockDisplay.Visibility = Visibility.Visible;
                DateDisplay.Visibility = Visibility.Visible;
                DayDisplay.Visibility = Visibility.Visible;

                CalendarTitle.Text = string.IsNullOrEmpty(CalendarTitle.Text) ? defaultCalendarText : CalendarTitle.Text;
            }
            else
            {
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
                BatteryPanel.Visibility = _appSettings.ShowBatterySetting ? Visibility.Visible : Visibility.Collapsed;
                CalendarPanel.Visibility = _appSettings.ShowCalendarSetting ? Visibility.Visible : Visibility.Collapsed;
                ClockDisplay.Visibility = _appSettings.ShowClockSetting ? Visibility.Visible : Visibility.Collapsed;
                DateDisplay.Visibility = _appSettings.ShowDateSetting ? Visibility.Visible : Visibility.Collapsed;
                DayDisplay.Visibility = _appSettings.ShowDateSetting ? Visibility.Visible : Visibility.Collapsed;

                ClockAndDatePanel.Visibility = !_appSettings.ShowClockSetting && !_appSettings.ShowDateSetting
                                                   ? Visibility.Collapsed
                                                   : Visibility.Visible;

                if (CalendarTitle.Text == defaultCalendarText) CalendarTitle.Text = string.Empty;
            }
        }

        protected void SetSettingsDisplay()
        {
            SettingsModePanel.Visibility = _settingsMode ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GoToSettingsPageButtonClick(object sender, EventArgs eventArgs)
        {
            _settingsMode = false;
            NavigationService.Navigate(new Uri("/Pages/SettingsPano.xaml", UriKind.Relative));
        }

        private void FadeImageInComplete(object sender, EventArgs e)
        {
            _photos.StopAnimation();
            _photos.SetBackgroundImage(BackgroundImage, ImagePanel, true);
            BackgroundImageTemp.Opacity = 0;
            ImagePanelTemp.Visibility = Visibility.Collapsed;
        }

        private void PinLiveTileClick(object sender, EventArgs e)
        {
            if (DigitalDash.Core.Classes.LiveTile.Exists())
            {
                DigitalDash.UI.Classes.LiveTile.Remove();
            }
            else
            {
                
                // You want the live tile, you need the agent!
                _scheduledBackgroundAgent.StartPeriodicAgent();

                DigitalDash.UI.Classes.LiveTile.Create();

                UpdateLiveTile();
            }

            SetPinIcon();
        }

        public ApplicationBarIconButton FindAppBarIconButtonByIconName(string nameToFind)
        {
            return ApplicationBar.Buttons.Cast<ApplicationBarIconButton>().FirstOrDefault(testButton => testButton.IconUri.ToString().Contains(nameToFind));
        }

        public void SetPinIcon()
        {
            if (ApplicationBar.Buttons.Count > 0)
            {
                var pinIcon = FindAppBarIconButtonByIconName("pintile.png");

                if (DigitalDash.Core.Classes.LiveTile.Exists())
                {
                    pinIcon.IconUri = new Uri(@"\Assets\images\unpintile.png", UriKind.Relative);
                    pinIcon.Text = "Unpin Tile";
                }
                else
                {
                    pinIcon.IconUri = new Uri(@"\Assets\images\pintile.png", UriKind.Relative);
                    pinIcon.Text = "Pin Tile";
                }
            }
        }

        public void UpdateLiveTile()
        {
            DigitalDash.Core.Classes.LiveTile.Update(CalendarTitle.Text, CalendarLocation.Text, CalendarTime.Text, _battery, _appSettings);
        }

        private void NextPhotoClick(object sender, EventArgs e)
        {
            if (_appSettings.EnableAnimatedGifSetting)
            {
                _animatedGifs.CycleAnimatedGif(_appSettings);
                DisplayAnimatedGif();
            }else if (!_appSettings.PhotoAlbumNameSetting.IsNullOrEmpty())
            {
                UpdatePhoto();
            }
        }

        private void ApplicationBarStateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {
            ApplicationBar.Opacity = e.IsMenuVisible ? 0.9 : 0.4;
        }

        private void ChangeBackgroundMenuItemClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SettingsPano.xaml?goto=1", UriKind.Relative));
        }

        private void CheckLicense()
        {
            if (!_appSettings.IsFullVersionSetting)
            {
                var licenseInfo = new LicenseInformation();
                if (!licenseInfo.IsTrial())
                {
                    _appSettings.IsFullVersionSetting = true;
                }
            }

            SetLicenseItems();
        }

        private void SetLicenseItems()
        {
            if (_appSettings.IsFullVersionSetting)
            {
                var upgradeIcon = FindAppBarIconButtonByIconName("upgrade.png");
                upgradeIcon.IsEnabled = false;
                WatermarkPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void UpgradeClick(object sender, EventArgs e)
        {
            _appSettings.IsFullVersionSetting = false;
            new MarketplaceDetailTask().Show();
        }

        private void RateAppClick(object sender, EventArgs e)
        {
            new MarketplaceReviewTask().Show();
        }

        private void HelpPanelTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HelpPanel.Visibility = Visibility.Collapsed;
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }

        private void HelpClick(object sender, EventArgs e)
        {
            HelpPanel.Visibility = Visibility.Visible;
            ApplicationBar.Mode = ApplicationBarMode.Default;
        }
    }
}