using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorSpace
{
    public class ClearPlayerProgress : EditorWindow
    {
        private static string PlayerDataPathFile = "/playerProgress.json";

        [MenuItem("Game/Clear player progress")]
        public static void DeleteData()
        {
            File.WriteAllText(Application.persistentDataPath + PlayerDataPathFile, "");
        }
    }
}
