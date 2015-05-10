//#define DEBUG_AGENT

using System;
using Microsoft.Phone.Scheduler;

namespace DigitalDash.UI.Classes
{
    public class ScheduledBackgroundAgent
    {
        private const string PeriodicTaskName = "DigitalDashAgent";
        public static bool AgentsAreEnabled = true;

        public bool StartPeriodicAgent()
        {
            // Variable for tracking enabled status of background agents for this app.
            AgentsAreEnabled = true;

            // Obtain a reference to the period task, if one exists
            var periodicTask = GetTask();

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (periodicTask != null)
            {
                RemoveAgent();
            }

            periodicTask = new PeriodicTask(PeriodicTaskName);

            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            periodicTask.Description = "This agent ensures the DigitalDash Live Tile is up to date.";

            // Place the call to Add in a try block in case the user has disabled agents.
            try
            {
                ScheduledActionService.Add(periodicTask);

                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
//#if(DEBUG_AGENT)
//                ScheduledActionService.LaunchForTest(PeriodicTaskName, TimeSpan.FromSeconds(60));
//#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    AgentsAreEnabled = false;
                    return false;
                }

                if (
                    exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.

                }
                return false;
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
                return false;
            }
            return true;
        }

        public bool IsRunning()
        {
            return GetTask() != null;
        }

        public string LastRun()
        {
            var periodicTask = GetTask();
            if (periodicTask != null)
            {
                return string.Format("{0} ({1})", periodicTask.LastScheduledTime.ToString("h:mmtt"), periodicTask.LastExitReason);
            }

            return "never";
        }

        public void ToggleAgent()
        {
            var periodicTask = GetTask();

            if (periodicTask != null)
            {
                RemoveAgent();
            }
            else
            {
                StartPeriodicAgent();
            }
        }

        public PeriodicTask GetTask()
        {
            return ScheduledActionService.Find(PeriodicTaskName) as PeriodicTask;
        }

        public string RemoveAgent()
        {
            try
            {
                ScheduledActionService.Remove(PeriodicTaskName);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }
    }
}
