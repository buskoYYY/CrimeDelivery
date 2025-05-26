using UnityEngine;

namespace ArcadeBridge
{
    public class TimeManager : MonoBehaviour
    {
        public static bool IsPaused { get; private set; }

        public static void Pause()
        {
            Time.timeScale = 0;
            IsPaused = true;
        }

        public static void Run()
        {
            Time.timeScale = 1;
            IsPaused = false;
        }
    }
}
