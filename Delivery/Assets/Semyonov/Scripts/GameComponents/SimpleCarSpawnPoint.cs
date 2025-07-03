using UnityEngine;
using UnityEngine.Splines;

namespace ArcadeBridge
{
    public class SimpleCarSpawnPoint: MonoBehaviour
    {
        [SerializeField] private SplineContainer _splineContainer;
        public bool IsSpawned { get; private set; }
        public bool IsVisible { get; private set; }
        //public PathProgressTracker car { get; private set; }
        public SplineWalker car { get; private set; }
        //public SplineAnimate car { get; private set; }

        public void InstantiateCar(SplineWalker[] carPrefabs)
        {
            if (Physics.Raycast(transform.position, Vector3.up, 1f)) return;

            if (IsVisible) return;

            int randomIndex = UnityEngine.Random.Range(0, carPrefabs.Length);

            car = Instantiate(carPrefabs[randomIndex], transform.position, transform.rotation);

            car.splineContainer = _splineContainer;

            IsSpawned = true;
        }

        public void DestroyCar()
        {
            IsSpawned = false;
            Destroy(car.gameObject);
        }

        private void OnBecameVisible()
        {
            IsVisible = true;
        }
        private void OnBecameInvisible()
        {
            IsVisible = false;
        }
    }
}
