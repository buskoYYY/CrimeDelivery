using System.Collections.Generic;
using Io.AppMetrica;
using UnityEngine;

public static class AnalyticsEvents
{
    public static void SendEvent(string eventName)
    {
        if (AppMetrica.IsActivated())
        {
            AppMetrica.ReportEvent(eventName);
        }
        else
        {
            Debug.LogWarning("AppMetrica SDK is not initialized.");
        }
    }

    public static void SendEvent(string eventName, int levelIndex)
    {
        if (AppMetrica.IsActivated())
        {
            Dictionary<string, object> levelCompleteData = new Dictionary<string, object>
         {
            {"level_number", levelIndex }
        };

            string json = JsonUtility.ToJson(levelCompleteData);
            AppMetrica.ReportEvent(eventName, json);
        }
        else
        {
            Debug.LogWarning("AppMetrica SDK is not initialized.");
        }
    }
}

