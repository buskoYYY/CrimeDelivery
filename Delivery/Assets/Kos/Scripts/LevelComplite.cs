using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
namespace ArcadeBridge
{
    public class LevelComplete : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject finishCanvas;
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private string sceneToReload; // Имя сцены

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;
            
            // Canvas завершения уровня
            if (finishCanvas != null)
            {
                finishCanvas.SetActive(true);
                Time.timeScale = 0f;
            }
            else
                Debug.LogWarning("Finish Canvas is not assigned!", this);
        }


        public void RestartLevel()
        {
            Time.timeScale = 1f;
            // Загружаем сцену по имени
            if (!string.IsNullOrEmpty(sceneToReload))
            {
                SceneManager.LoadScene(sceneToReload);
            }
            else
            {
                // Загружаем сцену с индексом 0 (главное меню/стартовая сцена)
                SceneManager.LoadScene(0);
                Debug.Log("Loading default scene with index 0: " + SceneManager.GetSceneByBuildIndex(0).name);
            }
        }
    }
}
