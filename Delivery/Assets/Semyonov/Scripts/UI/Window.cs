using System;
using UnityEngine;

namespace ArcadeBridge
{
    public class Window: MonoBehaviour
    {
        public event Action OnDestroyInvoked;
        protected virtual void OnDestroy()
        {
            OnDestroyInvoked?.Invoke();
        }
    }
}
