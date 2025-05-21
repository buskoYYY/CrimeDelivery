using UnityEngine;
using System.Collections.Generic;

namespace ArcadeBridge
{
    public class LevelConfigurator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> levelCon; // Список префабов уровней
        public int indexLevel; // Индекс уровня для спавна

        void Start()
        {
            SpawnLevelConfig();
        }

        private void SpawnLevelConfig()
        {
            // Проверяем, что список не пустой
            if (levelCon == null || levelCon.Count == 0)
            {
                Debug.LogError("Level config list is empty!");
                return;
            }

            // Проверяем, что индекс в допустимых пределах
            if (indexLevel < 0 || indexLevel >= levelCon.Count)
            {
                Debug.LogError("Invalid level index!");
                return;
            }

            // Проверяем, что префаб существует
            if (levelCon[indexLevel] == null)
            {
                Debug.LogError("Level prefab is null!");
                return;
            }

            // Спавним префаб в позицию (0, 0, 0) с нулевым вращением
            Instantiate(levelCon[indexLevel], Vector3.zero, Quaternion.identity);
        }
    }
}
