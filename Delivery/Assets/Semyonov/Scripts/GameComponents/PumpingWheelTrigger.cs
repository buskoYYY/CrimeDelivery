using ArcadeBridge.ArcadeIdleEngine.Actors;
using DG.Tweening;
using System;
using UnityEngine;

namespace ArcadeBridge
{
    public class PumpingWheelTrigger: MonoBehaviour
    {
        private ArcadeIdleMover _player;
        private HoseNozzle _hoseNozzle;

        private Sequence _sequence;

        [SerializeField] private Collider _collider;
        [SerializeField] private float _targetScale = .2f;

        private void Start()
        {
            if (SaveLoadService.instance != null && SaveLoadService.instance.PlayerProgress.isWheelsPumped)
            {
                transform.localScale = new Vector3(.2f, .2f, .2f);
            }
        }
        public void Activate()
        {
            _collider.enabled = true;
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<ArcadeIdleMover>(out ArcadeIdleMover player))
            {
                _player = player;
                if (_player && _hoseNozzle)
                {
                    _collider.enabled = false;
                }
            }

            if (other.TryGetComponent<HoseNozzle>(out HoseNozzle hoseNozzle))
            {
                _hoseNozzle = hoseNozzle;

                _hoseNozzle.transform.parent = null;
                _hoseNozzle.transform.forward = -transform.right;
                _hoseNozzle.transform.position = transform.position - _hoseNozzle.transform.forward * .2f;

                _sequence.Kill();
                _sequence = DOTween.Sequence().SetUpdate(true);

                _sequence.Append(transform.DOScale(.2f, 3f));

                _sequence.onComplete += OnFinishPumping;

                if(_player && _hoseNozzle)
                    _collider.enabled = false;
            }
        }


        private void OnFinishPumping()
        {
            _hoseNozzle.transform.parent = _player.transform;

            _hoseNozzle.transform.forward = _player.transform.forward;

            _hoseNozzle.transform.localPosition = _hoseNozzle.pump.localPosHoseOnPlayer;

            _hoseNozzle.pump.WheelPumped();

            _sequence.onComplete -= OnFinishPumping;
        }
        private void OnDestroy()
        {
            _sequence.Kill();
        }
    }
}
