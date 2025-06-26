using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcadeBridge
{
    public class SirenControl : MonoBehaviour
    {
        [SerializeField] private PoliceSpawner _policeSpawner;

        private List<PoliceSounds> _sounds = new List<PoliceSounds>();

        public Transform player;

        private PoliceSounds _car1;
        private PoliceSounds _car2;

        private float _updateTime = .5f;
        private float _maxUpdateTime = .5f;

        private int _indexCar1;
        private int _indexCar2;
        void Awake()
        {
            _policeSpawner.Spawned += OnSpawned;
            _policeSpawner.Destroyed += OnDestroyed;
        }

        private void Update()
        {
            _updateTime -= Time.deltaTime;

            if (_updateTime <= 0)
            {
                _updateTime = _maxUpdateTime;

                float minDistance = float.MaxValue;
                float distance;
                for (int i = 0; i < _sounds.Count; i++)
                {
                    if ((distance = Vector3.Distance(_sounds[i].transform.position, player.transform.position))
                            < minDistance)
                    {
                         _indexCar1 = i;
                         minDistance = distance;
                    }
                }

                minDistance = float.MaxValue;
                for (int i = 0; i < _sounds.Count; i++)
                {
                    if (i == _indexCar1) continue;

                    if ((distance = Vector3.Distance(_sounds[i].transform.position, player.transform.position))
                            < minDistance)
                    {
                         _indexCar2 = i;
                         minDistance = distance;
                    }
                }

                if (_sounds.Count <= 2)
                {
                    foreach (PoliceSounds sound in _sounds)
                    {
                        sound.PlayFlashLightSound();
                    }
                }
                else
                {
                    if (_car1 != _sounds[_indexCar1]
                        && _car1 != _sounds[_indexCar2])
                    {
                        OnCarsNearPlayerChanged();
                    }
                    if (_car2 != _sounds[_indexCar1]
                        && _car2 != _sounds[_indexCar2])
                    {
                        OnCarsNearPlayerChanged();
                    }
                }
            }
        }

        private void OnCarsNearPlayerChanged()
        {
            _car1 = _sounds[_indexCar1];
            _car2 = _sounds[_indexCar2];

            for (int i = 0; i < _sounds.Count; i++)
            {
                if (i == _indexCar1 || i == _indexCar2) continue;

                _sounds[i].PausePlayingFlashLightSound();
            }

            _car1.PlayFlashLightSound();
            _car2.PlayFlashLightSound();
        }

        private void OnDestroyed(GameObject obj)
        {
            PoliceSounds s = obj.GetComponent<PoliceSounds>();

            s.PausePlayingFlashLightSound();

            int i = _sounds.IndexOf(s);
            if (i == _indexCar1)
            {
                _car1 = null;
                _indexCar1 = 0;
            }
            if (i == _indexCar2)
            {
                _car2 = null;
                _indexCar2 = 0;
            }

            _sounds.Remove(s);
        }

        private void OnSpawned(GameObject obj)
        {
            _sounds.Add(obj.GetComponent<PoliceSounds>());
        }

        private void OnDestroy()
        {
            if (_policeSpawner)
            {
                _policeSpawner.Spawned -= OnSpawned;
                _policeSpawner.Destroyed -= OnDestroyed;
            }
        }
    }
}
