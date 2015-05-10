using System;
using System.IO.IsolatedStorage;

namespace DigitalDash.Core.Classes
{
    public class AppSettings
    {
        // Our settings
        private readonly IsolatedStorageSettings _settings;

        // The key names of our settings
        private const string ShowBatterySettingKeyName = "ShowBatterySetting";
        private const string FirstTimeSettingKeyName = "FirstTimeSetting";
        private const string IsFullVersionKeyName = "IsFullVersion";
        private const string ShowCalendarSettingKeyName = "ShowCalendarSetting";
        private const string ShowClockSettingKeyName = "ShowClockSetting";
        private const string ShowDateSettingKeyName = "ShowDateSetting";
        private const string EnableAnimatedGifSettingKeyName = "EnableAnimatedGifSetting";
        private const string AnimatePhotosSettingKeyName = "AnimatePhotosSetting";
        private const string Use12HourClockSettingKeyName = "Use12HourClockSetting";
        private const string CurrentAnimatedGifSettingKeyName = "CurrentAnimatedGifSetting";
        private const string ShowBatteryPercentSettingKeyName = "ShowBatteryPercentSetting";
        private const string PhotoAlbumNameSettingKeyName = "PhotoAlbumNameSetting";
        private const string BackgroundImageFileLocationKeyName = "BackgroundImageFileLocation";
        private const string PhotoTimeoutKeyName = "PhotoTimeout";
        private const string CurrentThemeKeyName = "CurrentTheme";


        // The default value of our settings
        private const bool ShowBatterySettingDefault = true;
        private const bool FirstTimeSettingDefault = true;
        private const bool IsFullVersionDefault = false;
        private const bool ShowCalendarSettingDefault = true;
        private const bool ShowClockSettingDefault = true;
        private const bool ShowDateSettingDefault = true;
        private const bool EnableAnimatedGifSettingDefault = false;
        private const bool AnimatePhotosSettingDefault = true;
        private const bool Use12HourClockSettingDefault = true;
        private const int CurrentAnimatedGifSettingDefault = 0;
        private const bool ShowBatteryPercentSettingDefault = true;
        private const string PhotoAlbumNameSettingDefault = "";
        private const string BackgroundImageFileLocationDefault = "";
        private const int PhotoTimeoutDefault = 5;
        private const string CurrentThemeDefault = "Default";

        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public AppSettings()
        {
            // Get the settings for this application.
            _settings = IsolatedStorageSettings.ApplicationSettings;
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (_settings.Contains(key))
            {
                // If the value has changed
                if (_settings[key] != value)
                {
                    // Store the new value
                    _settings[key] = value;
                    valueChanged = true;
                }
            }
                // Otherwise create the key.
            else
            {
                _settings.Add(key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (_settings.Contains(key))
            {
                value = (T) _settings[key];
            }
                // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            _settings.Save();
        }

        public void ResetAllSettings()
        {
            ShowBatterySetting = ShowBatterySettingDefault;
            //FirstTimeSetting = FirstTimeSettingDefault;

            // Don't want to kill their license
            //IsFullVersionSetting = IsFullVersionDefault;
            ShowCalendarSetting = ShowCalendarSettingDefault;
            ShowClockSetting = ShowClockSettingDefault;
            ShowDateSetting = ShowDateSettingDefault;
            EnableAnimatedGifSetting = EnableAnimatedGifSettingDefault;
            AnimatePhotosSetting = AnimatePhotosSettingDefault;
            Use12HourClockSetting = Use12HourClockSettingDefault;
            CurrentAnimatedGifSetting = CurrentAnimatedGifSettingDefault;
            ShowBatteryPercentSetting = ShowBatteryPercentSettingDefault;
            PhotoAlbumNameSetting = PhotoAlbumNameSettingDefault;
            BackgroundImageFileLocation = BackgroundImageFileLocationDefault;
            PhotoTimeoutSetting = PhotoTimeoutDefault;
            CurrentThemeSetting = CurrentThemeDefault;
        }

        /// <summary>
        /// Property to get and set the IsFullVersion
        /// </summary>
        public bool IsFullVersionSetting
        {
            get { return GetValueOrDefault(IsFullVersionKeyName, IsFullVersionDefault); }
            set { if (AddOrUpdateValue(IsFullVersionKeyName, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the ShowBatterySetting
        /// </summary>
        public bool ShowBatterySetting
        {
            get { return GetValueOrDefault(ShowBatterySettingKeyName, ShowBatterySettingDefault); }
            set { if (AddOrUpdateValue(ShowBatterySettingKeyName, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the FirstTimeSetting
        /// </summary>
        public bool FirstTimeSetting
        {
            get { return GetValueOrDefault(FirstTimeSettingKeyName, FirstTimeSettingDefault); }
            set { if (AddOrUpdateValue(FirstTimeSettingKeyName, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the ShowClockSetting
        /// </summary>
        public bool ShowClockSetting
        {
            get { return GetValueOrDefault(ShowClockSettingKeyName, ShowClockSettingDefault); }
            set { if (AddOrUpdateValue(ShowClockSettingKeyName, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the ShowDateSetting
        /// </summary>
        public bool ShowDateSetting
        {
            get { return GetValueOrDefault(ShowDateSettingKeyName, ShowDateSettingDefault); }
            set { if (AddOrUpdateValue(ShowDateSettingKeyName, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the ShowCalendarSetting
        /// </summary>
        public bool ShowCalendarSetting
        {
            get { return GetValueOrDefault(ShowCalendarSettingKeyName, ShowCalendarSettingDefault); }
            set { if (AddOrUpdateValue(ShowCalendarSettingKeyName, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the EnableAnimatedGifSetting
        /// </summary>
        public bool EnableAnimatedGifSetting
        {
            get { return GetValueOrDefault(EnableAnimatedGifSettingKeyName, EnableAnimatedGifSettingDefault); }
            set { if (AddOrUpdateValue(EnableAnimatedGifSettingKeyName, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the AnimatePhotosSetting
        /// </summary>
        public bool AnimatePhotosSetting
        {
            get { return GetValueOrDefault(AnimatePhotosSettingKeyName, AnimatePhotosSettingDefault); }
            set { if (AddOrUpdateValue(AnimatePhotosSettingKeyName, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the Use12HourClockSetting
        /// </summary>
        public bool Use12HourClockSetting
        {
            get { return GetValueOrDefault(Use12HourClockSettingKeyName, Use12HourClockSettingDefault); }
            set { if (AddOrUpdateValue(Use12HourClockSettingKeyName, value)) Save(); }
        }

        /// <summary>
        /// Index of the last viewed animated background in the array of backgrounds.
        /// </summary>
        public int CurrentAnimatedGifSetting
        {
            get { return GetValueOrDefault(CurrentAnimatedGifSettingKeyName, CurrentAnimatedGifSettingDefault); }
            set { if (AddOrUpdateValue(CurrentAnimatedGifSettingKeyName, value)) Save(); }
        }

        /// <summary>
        /// Whether to display the percent or time remaining next to the battery icon
        /// </summary>
        public bool ShowBatteryPercentSetting
        {
            get { return GetValueOrDefault(ShowBatteryPercentSettingKeyName, ShowBatteryPercentSettingDefault); }
            set { if (AddOrUpdateValue(ShowBatteryPercentSettingKeyName, value)) Save(); }
        }

        /// <summary>
        /// If we have a value here then the user has chosen an album for their photos
        /// </summary>
        public string PhotoAlbumNameSetting
        {
            get { return GetValueOrDefault(PhotoAlbumNameSettingKeyName, PhotoAlbumNameSettingDefault); }
            set { if (AddOrUpdateValue(PhotoAlbumNameSettingKeyName, value)) Save(); }
        }


        /// <summary>
        /// Hold the physical location of the background image in local storage
        /// </summary>
        public string BackgroundImageFileLocation
        {
            get { return GetValueOrDefault(BackgroundImageFileLocationKeyName, BackgroundImageFileLocationDefault); }
            set { if (AddOrUpdateValue(BackgroundImageFileLocationKeyName, value)) Save(); }
        }

        /// <summary>
        /// How long to wait before switching to the next photo
        /// </summary>
        public int PhotoTimeoutSetting
        {
            get { return GetValueOrDefault(PhotoTimeoutKeyName, PhotoTimeoutDefault); }
            set { if (AddOrUpdateValue(PhotoTimeoutKeyName, value > 1 ? value : 1)) Save(); }
        }

        /// <summary>
        /// The current theme name
        /// </summary>
        public string CurrentThemeSetting
        {
            get { return GetValueOrDefault(CurrentThemeKeyName, CurrentThemeDefault); }
            set { if (AddOrUpdateValue(CurrentThemeKeyName, value)) Save(); }
        }


    }
}
