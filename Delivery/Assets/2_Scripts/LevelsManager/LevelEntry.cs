using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelEntry
{
    public string levelName;
    public string levelType;

    [SerializeField]
    public List<Field> fields = new List<Field>();

    public LevelEntry(Dictionary<string, string> data)
    {
        foreach (var kvp in data)
        {
            if (kvp.Key == "LevelName")
                levelName = kvp.Value;
            else if (kvp.Key == "LevelType")
                levelType = kvp.Value;
            else
            {
                fields.Add(new Field { key = kvp.Key, value = kvp.Value });
            }
        }
    }

    public string GetField(string key)
    {
        var field = fields.Find(f => f.key == key);
        return field != null ? field.value : null;
    }


}
