using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArcadeBridge
{
    public class SceneLoad : MonoBehaviour
    {
        void Start()
        {
            SceneManager.LoadScene(1);
        }

    }
}
