using System.IO;
using UnityEditor;

namespace EditorSpace
{
    public class ClearPlayerProgress : EditorWindow
    {
        private static string PlayerDataPathFile = "Assets/Resources/playerProgress.txt";

        [MenuItem("Game/Clear player progress")]
        public static void DeleteData()
        {
            File.WriteAllText(PlayerDataPathFile, "");
        }
    }
}
