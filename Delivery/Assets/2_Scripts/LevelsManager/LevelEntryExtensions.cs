public static class LevelEntryExtensions
{
    public static int GetInt(this LevelEntry entry, string key, int defaultValue = 0)
    {
        string value = entry.GetField(key);
        return int.TryParse(value, out int result) ? result : defaultValue;
    }

    public static bool GetBool(this LevelEntry entry, string key, bool defaultValue = false)
    {
        string value = entry.GetField(key);
        return bool.TryParse(value, out bool result) ? result : defaultValue;
    }

    public static float GetFloat(this LevelEntry entry, string key, float defaultValue = 0f)
    {
        string value = entry.GetField(key);
        return float.TryParse(value, out float result) ? result : defaultValue;
    }
}
