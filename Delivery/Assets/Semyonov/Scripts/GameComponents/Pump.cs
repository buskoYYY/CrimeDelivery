using ArcadeBridge.ArcadeIdleEngine.Actors;
using System;
using UnityEngine;

namespace ArcadeBridge
{
    public class Pump: ObjectForInteraction
    {
        [SerializeField] private HoseNozzle _hoseNozzle;
        public Vector3 localPosHoseOnPlayer;

        private int _wheelsPumped;

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<ArcadeIdleMover>(out ArcadeIdleMover mover))
            {
                _hoseNozzle.transform.parent = mover.transform;
                _hoseNozzle.transform.localPosition = localPosHoseOnPlayer;
                _hoseNozzle.transform.forward = mover.transform.forward;
            }
        }

        public void WheelPumped()
        {
            _wheelsPumped++;

            if (_wheelsPumped == 4)
            {
                if (SaveLoadService.instance != null)
                {
                    SaveLoadService.instance.PlayerProgress.isWheelsPumped = true;
                    SaveLoadService.instance.DelayedSaveProgress();
                    Debug.Log("W P ");
                }
                _hoseNozzle.transform.parent = transform;
                Destroy(gameObject);
            }
        }
    }
}
