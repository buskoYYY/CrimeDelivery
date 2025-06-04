using ArcadeBridge.ArcadeIdleEngine.Actors;
using System;
using UnityEngine;

namespace ArcadeBridge
{
    public class Pump: ObjectForInteraction
    {
        public event Action OnWheelsPumped;
        public event Action OnPumpTaken;
        public bool PumpTaken { get; private set; }

        [SerializeField] private HoseNozzle _hoseNozzle;
        [SerializeField] private Collider _collider;

        public Vector3 localPosHoseOnPlayer;

        private Vector3 _localPosHozeNozzleBase;
        private Transform _hoseNozzleParent;

        private int _wheelsPumped;

        private void Start()
        {
            _localPosHozeNozzleBase = _hoseNozzle.transform.localPosition;
            _hoseNozzleParent = _hoseNozzle.transform.parent;
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<ArcadeIdleMover>(out ArcadeIdleMover mover))
            {
                if (SaveLoadService.instance.PlayerProgress.isWheelsPumped) return;

                if (SequenceOfActivities.Instance.GameFactory.ConstructedCar.ConstructedDetailsCount < 4) return;

                _hoseNozzle.transform.parent = mover.transform;
                _hoseNozzle.transform.localPosition = localPosHoseOnPlayer;
                _hoseNozzle.transform.forward = mover.transform.forward;
                _collider.enabled = false;

                PumpTaken = true;
                OnPumpTaken?.Invoke();
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
                }
                _hoseNozzle.transform.parent = _hoseNozzleParent;
                _hoseNozzle.transform.localPosition = _localPosHozeNozzleBase;

                OnWheelsPumped?.Invoke();
                PumpTaken = false;
                //Destroy(gameObject);
            }
        }
    }
}
