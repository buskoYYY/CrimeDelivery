using UnityEngine;

public static class LevelFactory
{
    public static ILevelHandler CreateLevel(string levelType)
    {
        switch (levelType)
        {
            case "Turbo": return new TurboLevel();
            case "Batya": return new BatyaLevel();
            default:
                Debug.LogError($"❌ Unknown LevelType: {levelType}");
                return null;
        }
    }
}
