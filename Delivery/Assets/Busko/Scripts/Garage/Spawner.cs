using UnityEngine;

namespace ArcadeBridge
{
    public class Spawner: MonoBehaviour
    {
        public ObjectForInteraction ObjectForInteraction => _objectForInteractionCurrent;

        private ObjectForInteraction _objectForInteractionCurrent;
        [SerializeField] private ObjectForInteraction _objectForInteraction;
        public virtual void CreateObject()
        {
            _objectForInteractionCurrent = Instantiate(_objectForInteraction);
        }
    }
}
