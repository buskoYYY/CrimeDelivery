using UnityEngine;

namespace ArcadeBridge
{
    public class Spawner: MonoBehaviour
    {
        [SerializeField] private ObjectForInteraction _objectForInteraction;
        public void CreateObject()
        {
            Instantiate(_objectForInteraction);
        }
    }
}
