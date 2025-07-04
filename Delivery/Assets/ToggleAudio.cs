using UnityEngine;

namespace ArcadeBridge
{
    public class ToggleAudio : MonoBehaviour
    {
        public void ToggleAudioActivator()
        {
            if (AudioListener.volume <= 0)
                AudioListener.volume = 1;
            else
                AudioListener.volume = 0;
        }
    }
}
