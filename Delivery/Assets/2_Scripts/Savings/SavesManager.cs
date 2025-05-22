using System.IO;
using UnityEngine;

public static class SavesManager
{
    private static string savePath = Application.persistentDataPath + "/save.json";

    public static void SaveGame(SavigsData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }

    public static SavigsData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<SavigsData>(json);
        }
        else
        {
            Debug.Log("Нет сохранений, создаём новые");
            SavigsData data = new SavigsData();
            SaveGame(data);
            return null;
        }
    }
}