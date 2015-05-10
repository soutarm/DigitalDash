using System;
using System.Linq;
using Microsoft.Phone.UserData;

namespace DigitalDash.Core.Classes
{
    public class Calendar
    {
        public string Subject { get; set; }
        public string Location { get; set; }
        public string DateAndTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Calendar GetAppointment(AppointmentsSearchEventArgs e)
        {
            // Make sure we have a subject otherwise we don't know what this calendar item is!
            if (e.Results.All(r => r.Subject == null)) return null;

            var appointement = new Calendar();

            // Get calendar items with a subject.
            var calendarItem = e.Results.First(r => r.Subject != null && (!r.IsAllDayEvent || (r.IsAllDayEvent && (System.DateTime.Now - r.StartTime).TotalHours < 12)));
            if (calendarItem != null)
            {
                appointement.Subject = calendarItem.Subject;
                appointement.Location = calendarItem.Location;

                appointement.DateAndTime = string.Format("{0}", GetRelativeDate(calendarItem.StartTime));

                if (calendarItem.IsAllDayEvent)
                {
                    if (calendarItem.EndTime.Subtract(calendarItem.StartTime) > new TimeSpan(1, 0, 0, 0))
                    {
                        appointement.DateAndTime += " - " + GetRelativeDate(calendarItem.EndTime);
                    }
                    else
                    {
                        appointement.DateAndTime += ": All day";
                    }
                }
                else
                {
                    appointement.DateAndTime += ": " + calendarItem.StartTime.ToString("h:mm tt");
                    if (calendarItem.EndTime != calendarItem.StartTime)
                    {
                        appointement.DateAndTime = appointement.DateAndTime += " - " + calendarItem.EndTime.ToString("h:mm tt");
                    }
                }
            }

            return appointement;
        }


        /// <summary>
        /// Returns a string relative to today's date for easy readability
        /// </summary>
        /// <param name="dateToCheck"></param>
        /// <returns></returns>
        private static string GetRelativeDate(DateTime dateToCheck)
        {
            var today = System.DateTime.Now;

            if ((dateToCheck - today).TotalDays < 8)
            {
                if (today.Day == dateToCheck.Day) return "Today";
                if (dateToCheck.Day == dateToCheck.Day + 1) return "Tommorrow";

                return dateToCheck.ToString("dddd");
            }

            return string.Format("{0:MMMM} {1}", dateToCheck, dateToCheck.Day);
        }
    }
}
