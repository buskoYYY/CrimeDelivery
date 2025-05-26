using UnityEngine;
using System.Collections.Generic;

public class BatyaLevel : ILevelHandler
{
    public List<string> GetExpectedFields()
    {
        return new List<string> { "EnemiesCount", "HasBoss" };
    }

    public void LoadFromDictionary( Dictionary<string, string> specific)
    {

        string enemies = specific.TryGetValue("EnemiesCount", out var ec) ? ec : "0";
        string boss = specific.TryGetValue("HasBoss", out var hb) ? hb : "false";

        Debug.Log($"[Batya] | Enemies: {enemies} | Boss: {boss}");
    }
}
