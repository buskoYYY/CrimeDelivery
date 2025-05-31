using UnityEditor;
using UnityEngine;

namespace EditorSpace
{
    public class ClearPlayerPrefs : EditorWindow
    {
        [MenuItem("Game/Clear player prefs")]
        public static void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
