using ArcadeBridge.ArcadeIdleEngine.Helpers;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Items
{
    public class Item : MonoBehaviour
    {
        [SerializeField] ItemDefinition _definition;
        
        Vector3 _defaultLocalScale;

        [SerializeField] private bool _onlyDestroyWithoutPool;
        
        public ItemDefinition Definition => _definition;

        void Awake()
        {
            _defaultLocalScale = transform.localScale;
        }

        public void ReleaseToPool()
        {
            transform.localScale = _defaultLocalScale;
            TweenHelper.KillAllTweens(transform);

            if (_onlyDestroyWithoutPool)
                Destroy(gameObject);
            else
                _definition.Pool.Release(this);

        }
    }
}