using System;
using UnityEngine;

namespace ArcadeBridge
{
    public class FinishConstructionWindow: MonoBehaviour
    {
        public event Action OnDestroyInvoked;

        private void Start()
        {
            GetComponent<ViewCunstructedCar>().PreviewCar();
            GetComponent<ViewCunstructedCar>().ShowCarPreview(SaveLoadService.instance.GetLastOpenedIndexCar());
        }
        private void OnDestroy()
        {
            OnDestroyInvoked?.Invoke();
        }
    }
}
