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

    public static void RaceCompleteEvent(int raceCount, string completeType, int completedDeliviries, int maxDeliviries, float raceTime)
    {
        Dictionary<string, object> levelCompleteData = new Dictionary<string, object>
         {
            {"race_count", raceCount },
            {"complete_type", completeType },
            {"completed_deliviries", completedDeliviries },
            {"max_deliviries", maxDeliviries },
            {"race_time", maxDeliviries }
        };

        string json = JsonUtility.ToJson(levelCompleteData);
        AppMetrica.ReportEvent("level_complete", json);
        AppMetrica.SendEventsBuffer();
    }

    public static void CarToBreakBought(int raceCount, int openedCarsCount, int playerMoney)
    {
        Dictionary<string, object> levelCompleteData = new Dictionary<string, object>
         {
            {"race_count", raceCount },
            {"opened_cars_count", openedCarsCount },
            {"player_money", playerMoney }
        };

        string json = JsonUtility.ToJson(levelCompleteData);
        AppMetrica.ReportEvent("car_to_break_bought", json);
        AppMetrica.SendEventsBuffer();
    }

    public static void OpenTool(int raceCount, string toolType, int playerMoney)
    {
        Dictionary<string, object> levelCompleteData = new Dictionary<string, object>
         {
            {"race_count", raceCount },
            {"toolType", toolType },
            {"player_money", playerMoney }
        };

        string json = JsonUtility.ToJson(levelCompleteData);
        AppMetrica.ReportEvent("open_tool", json);
        AppMetrica.SendEventsBuffer();
    }

    public static void BoughtConstructionPart(int raceCount, string carName, string constructionPart, int playerMoney)
    {
        Dictionary<string, object> levelCompleteData = new Dictionary<string, object>
         {
            {"race_count", raceCount },
            {"car_name", carName },
            {"construction_part", constructionPart },
            {"player_money", playerMoney }
        };

        string json = JsonUtility.ToJson(levelCompleteData);
        AppMetrica.ReportEvent("bought_construction_part", json);
        AppMetrica.SendEventsBuffer();
    }
}