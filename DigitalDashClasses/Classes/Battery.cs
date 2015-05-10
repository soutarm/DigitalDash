using Microsoft.Phone.Info;

namespace DigitalDash.Core.Classes
{
    public class Battery
    {

        public int RemainingChargePercent { get; set; }
        public int RemainingChargePercentTested { get; set; }
        public bool IsCharging { get; set; }
        public string Text { get; set; }


        /// <summary>
        /// Returns the remaining battery percent plus a text description
        /// </summary>
        /// <param name="appSettings"></param>
        /// <returns></returns>
        public Battery GetBatteryStatus(AppSettings appSettings)
        {
            var battery = Windows.Phone.Devices.Power.Battery.GetDefault();
            var batteryPercent = battery.RemainingChargePercent;
            var batteryStatus = new Battery {RemainingChargePercent = batteryPercent};

            if (appSettings.ShowBatteryPercentSetting)
            {
                batteryStatus.RemainingChargePercent = batteryPercent;
                batteryStatus.Text = string.Format("{0}%", batteryPercent);
            }
            else
            {
                var minutesLeft = battery.RemainingDischargeTime.TotalMinutes;
                var hoursLeft = (int)(minutesLeft / 60);
                minutesLeft = minutesLeft - (hoursLeft * 60);
                batteryStatus.Text = string.Format("{0} hrs, {1} min remaining", hoursLeft, minutesLeft);
            }

            // If we're plugged in let's modify the text a little (RemainingDischargeTime gives a stupid high number when plugged in)
            if (DeviceStatus.PowerSource == PowerSource.External)
            {
                batteryStatus.IsCharging = true;
                var suffix = batteryPercent > 99 ? "fully charged" : "charging";
                batteryStatus.Text = !appSettings.ShowBatteryPercentSetting ? suffix : batteryStatus.Text;
            }

            batteryStatus.RemainingChargePercentTested = appSettings.ShowBatterySetting && batteryPercent < 100 ? batteryPercent : 0;

            return batteryStatus;
        }

    }
}
