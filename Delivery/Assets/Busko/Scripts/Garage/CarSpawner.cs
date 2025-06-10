using ArcadeBridge.ArcadeIdleEngine.Gathering;
using System;
using UnityEngine;

namespace ArcadeBridge
{
    public class CarSpawner : Spawner
    {
        public override void CreateObject()
        {
            if (_objectForInteractionCurrent)
            {
                _objectForInteractionCurrent.GetComponent<GatherableSource>().OnSetActiveFalse -= SetActiveTrue;
                Destroy(_objectForInteractionCurrent.gameObject);
                _objectForInteractionCurrent = null;
            }
            base.CreateObject();
            //SubscrabeForReturn();
            
        }
        private void SubscrabeForReturn()
        {
            _objectForInteractionCurrent.GetComponent<GatherableSource>().OnSetActiveFalse -= SetActiveTrue;
            _objectForInteractionCurrent.GetComponent<GatherableSource>().OnSetActiveFalse += SetActiveTrue;
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
