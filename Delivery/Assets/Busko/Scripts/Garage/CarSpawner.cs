using ArcadeBridge.ArcadeIdleEngine.Gathering;
using System;
using UnityEngine;

namespace ArcadeBridge
{
    public class CarSpawner : Spawner
    {
        public event Action<GatherableSource> CarSpawned;

        public override void CreateObject()
        {
            base.CreateObject();
            SubscrabeForReturn();
            CarSpawned?.Invoke(ObjectForInteraction.GetComponent<GatherableSource>());
        }
        private void SubscrabeForReturn()
        {
            ObjectForInteraction.GetComponent<GatherableSource>().OnSetActiveFalse += SetActiveTrue;
        }

        private void SetActiveTrue(GatherableSource obj)
        {
            gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            if(ObjectForInteraction && ObjectForInteraction.GetComponent<GatherableSource>())
              ObjectForInteraction.GetComponent<GatherableSource>().OnSetActiveFalse -= SetActiveTrue;
        }
    }
}
