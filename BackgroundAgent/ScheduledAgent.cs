//#define DEBUG_AGENT

using System.Diagnostics;
using System.Windows;
using DigitalDash.Core.Classes;
using Microsoft.Phone.Marketplace;
using Microsoft.Phone.Scheduler;
using System;
using Microsoft.Phone.Shell;
using Microsoft.Phone.UserData;

namespace BackgroundAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private Battery Battery { get; set; }
        private AppSettings AppSettings { get; set; }

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });

        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        protected override void OnInvoke(ScheduledTask task)
        {
            RefreshLiveTile();

            //var toast = new ShellToast { Title = "DigitalDash", Content = "Updating live tile" };
            //toast.Show();

            if(Battery == null) Battery = new Battery();
            if(AppSettings == null) AppSettings = new AppSettings();

            // If debugging is enabled, launch the agent again in one minute.
//#if DEBUG_AGENT
//            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
//#endif

        }

        private void RefreshLiveTile()
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
            var licenseInfo = new LicenseInformation();

            if (!licenseInfo.IsTrial())
            {
                var appointment = Calendar.GetAppointment(e);
                if (appointment == null) return;

                LiveTile.Update(appointment.Subject, appointment.Location, appointment.DateAndTime, Battery, AppSettings);
            }else
            {
                LiveTile.Update("Sample Calendar Item", "Get full version", "to display calendar items", Battery, AppSettings);
            }

            // Call NotifyComplete to let the system know the agent is done working.
            NotifyComplete();
        }
    }
}