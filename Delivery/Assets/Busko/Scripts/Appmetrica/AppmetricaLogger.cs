using Io.AppMetrica;
using Newtonsoft.Json;
using System.Collections.Generic;

public class AppMetricaLogger
{
    private static AppMetricaLogger _instance;
    private static readonly object _lock = new object();
    private AppMetricaLogger() { }
    public static AppMetricaLogger Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new AppMetricaLogger();
                }
                return _instance;
            }
        }
    }

    public void ReportEvent(string eventName)
    {
        AppMetrica.ReportEvent(eventName);
    }

    public void ReportEvent(string eventName, Dictionary<string, object> parameters)
    {
        string json = JsonConvert.SerializeObject(parameters);

        AppMetrica.ReportEvent(eventName, json);
    }
}

