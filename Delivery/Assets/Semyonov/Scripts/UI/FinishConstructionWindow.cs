using System;
using UnityEngine;

namespace ArcadeBridge
{
    public class FinishConstructionWindow: MonoBehaviour
    {
        public event Action OnDestroyInvoked;

        private void OnDestroy()
        {
            OnDestroyInvoked?.Invoke();
        }
    }
}
