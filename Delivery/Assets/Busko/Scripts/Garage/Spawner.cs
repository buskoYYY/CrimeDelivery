using System;
using UnityEngine;

namespace ArcadeBridge
{
    public class Spawner: MonoBehaviour
    {
        public event Action<ObjectForInteraction> ObjectSpawned;

        public ObjectForInteraction ObjectForInteraction => _objectForInteractionCurrent;

        protected ObjectForInteraction _objectForInteractionCurrent;
        [SerializeField] private ObjectForInteraction _objectForInteraction;
        public virtual void CreateObject()
        {
            if(_objectForInteractionCurrent && !_objectForInteractionCurrent.gameObject.activeSelf)
            {
                _objectForInteractionCurrent.gameObject.SetActive(true);
            }
            else
            {
                _objectForInteractionCurrent = Instantiate(_objectForInteraction);
            }
            ObjectSpawned?.Invoke(_objectForInteractionCurrent);
        }

        public void DestroyObjectForInteraction()
        {
            if (_objectForInteractionCurrent)
                Destroy(_objectForInteractionCurrent.gameObject);
        }
    }
}
