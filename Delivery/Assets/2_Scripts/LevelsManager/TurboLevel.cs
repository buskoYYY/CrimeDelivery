using UnityEngine;
using System.Collections.Generic;

public class TurboLevel : ILevelHandler
{
    public List<string> GetExpectedFields()
    {
        return new List<string> { "Speed", "Difficulty" };
    }

    public void LoadFromDictionary(Dictionary<string, string> specific)
    {
        string speed = specific.TryGetValue("Speed", out var spd) ? spd : "0";
        string difficulty = specific.TryGetValue("Difficulty", out var d) ? d : "Normal";

        Debug.Log($"[Turbo] Speed: {speed} | Difficulty: {difficulty}");
    }
}
