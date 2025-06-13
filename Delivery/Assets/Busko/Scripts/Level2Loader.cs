using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArcadeBridge
{
    public class Level2Loader : MonoBehaviour
    {
        public void LoadLevel2()
        {
            SceneManager.LoadScene(2);
        }
    }
}
