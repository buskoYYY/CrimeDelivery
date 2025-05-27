using System.Collections.Generic;
using Io.AppMetrica;
using UnityEngine;

public static class AnalyticsEvents
{
    public static void SendEvent(string eventName)
    {
        AppMetrica.ReportEvent(eventName);
        AppMetrica.SendEventsBuffer();
    }

    public static void SendEvent(string eventName, int levelIndex)
    {
            Dictionary<string, object> levelCompleteData = new Dictionary<string, object>
         {
            {"level_number", levelIndex }
        };

            string json = JsonUtility.ToJson(levelCompleteData);
            AppMetrica.ReportEvent(eventName, json);
            AppMetrica.SendEventsBuffer();
    }
}

