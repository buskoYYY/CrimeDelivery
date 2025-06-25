using Io.AppMetrica;
using System.Collections.Generic;

public static class AnalyticsEvents
{
    public static void SendEvent(string eventName)
    {
        AppMetricaLogger.Instance.ReportEvent(eventName);
        AppMetrica.SendEventsBuffer();
    }

    public static void SendEvent(string eventName, int levelIndex)
    {
            Dictionary<string, object> levelCompleteData = new Dictionary<string, object>
         {
            {"level_number", levelIndex }
        };

            AppMetricaLogger.Instance.ReportEvent(eventName, levelCompleteData);
            AppMetrica.SendEventsBuffer();
    }
}

