using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArcadeBridge
{
    public class BootStrap : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadScene(1);
        }
    }
}
