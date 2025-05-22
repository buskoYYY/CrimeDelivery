using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private string csvFileName = "levels";
    [SerializeField] private List<LevelEntry> loadedLevels = new List<LevelEntry>();

    public IReadOnlyList<LevelEntry> Levels => loadedLevels;

    [ExecuteInEditMode]
    [ContextMenu("LoadConfig")]
    public void LoadLevelsActivator()
    {
        LoadLevelsFromCSV(csvFileName);
    }

    public void LoadLevelsFromCSV(string fileName)
    {
        loadedLevels.Clear();

        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError($"CSV file '{fileName}' not found in Resources.");
            return;
        }

        

        using (var reader = new System.IO.StringReader(csvFile.text))
        {
            string headerLine = reader.ReadLine();
            if (headerLine == null) return;

            var headers = headerLine.Split(',');

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');
                var rowData = new Dictionary<string, string>();

                for (int i = 0; i < headers.Length && i < values.Length; i++)
                    rowData[headers[i]] = values[i];

                if (!rowData.TryGetValue("LevelType", out var levelType))
                    continue;

                var handler = LevelFactory.CreateLevel(levelType);
                if (handler == null) continue;

                var levelEntry = new LevelEntry(rowData);
                loadedLevels.Add(levelEntry);

            }
        }
    }

    public LevelEntry FindLevelByName(string name)
    {
        return loadedLevels.Find(l => l.levelName == name);
    }

    /*public void ActivateLevel()
    {
        LevelEntry level = loadedLevels[currentLevel];

        if (level != null && level.levelType == "Turbo")
        {
            int speed = level.GetInt("Speed");
            Debug.Log($"Turbo Level: {level.levelName} - {speed} speed");
        }
    }
    */
}