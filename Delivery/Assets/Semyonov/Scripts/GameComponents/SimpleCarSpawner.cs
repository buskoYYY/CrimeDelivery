using UnityEngine;
using UnityEngine.Splines;

namespace ArcadeBridge
{
    public class SimpleCarSpawner : MonoBehaviour
    {
        private const float _maxDistanceForSpawn = 130;
        private const float _minDistanceForSpawn = 50;

        private Transform _player;

        [SerializeField] private SplineWalker[] _carPrefabs;
        [SerializeField] private SimpleCarSpawnPoint[] _spawnPoints;

        private float _checkTime = 2f;
        private float _maxCheckTime = 2f;

        public void SetPlayer(Transform player)
        {
            this._player = player;
        }
        [ContextMenu("FindAllSpawnPoints")]
        public void FindAllSpawnPoints()
        {
            _spawnPoints = FindObjectsByType<SimpleCarSpawnPoint>(FindObjectsSortMode.None);
        }
        void Update()
        {
            if (!_player) return;

            _checkTime -= Time.deltaTime;

            if(_checkTime <= 0)
            {
                _checkTime = _maxCheckTime;

                for(int i =0; i < _spawnPoints.Length; i++)
                {
                    if (_spawnPoints[i].IsSpawned)
                    {
                        float distanceToCar = Vector3.Distance(_spawnPoints[i].car.transform.position, _player.position);

                        if (distanceToCar >= _maxDistanceForSpawn)
                        {
                            _spawnPoints[i].DestroyCar();
                        }

                        continue;
                    }

                    float distanceToSpawnPoint = Vector3.Distance(_spawnPoints[i].transform.position, _player.position);

                    if (distanceToSpawnPoint < _maxDistanceForSpawn
                            && distanceToSpawnPoint > _minDistanceForSpawn)
                    {
                        _spawnPoints[i].InstantiateCar(_carPrefabs);
                    }
                }
            }
        }
    }
}
