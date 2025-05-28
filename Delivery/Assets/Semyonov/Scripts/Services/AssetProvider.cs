using UnityEngine;

namespace ArcadeBridge
{
    public class AssetProvider : MonoBehaviour
    {
        public static AssetProvider instance;
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("SaveLoadService already has");
                Destroy(gameObject);
                return;
            }
            instance = this;
        }
        public T Instantiate<T>(string path) where T : Object =>
           Object.Instantiate(Load<T>(path));

        public T Instantiate<T>(string path, Vector3 at) where T : Object =>
            Object.Instantiate(Load<T>(path), at, Quaternion.identity);

        public T Instantiate<T>(string path, Vector3 at, Vector3 rotation) where T : Object =>
            Object.Instantiate(Load<T>(path), at, Quaternion.Euler(rotation));
        public T Instantiate<T>(string path, Vector3 at, Quaternion rotation, Transform parent) where T : Object =>
            Object.Instantiate(Load<T>(path), at, rotation, parent);

        public T Instantiate<T>(string path, Transform parent) where T : Object =>
            Object.Instantiate(Load<T>(path), parent);

        public T Load<T>(string path) where T : Object =>
           Resources.Load<T>(path);
        public T[] LoadAll<T>(string path) where T : Object =>
           Resources.LoadAll<T>(path);
    }
}
