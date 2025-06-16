using UnityEngine;
using System.Collections;

namespace ArcadeBridge
{

    [RequireComponent(typeof(Collider))]
    public class SpawnPropRB : MonoBehaviour
    {
        [SerializeField] private GameObject _rbProp;
        [SerializeField] private string carTag = "Car";
        [SerializeField] private bool spawnWithRotation = true;
        [SerializeField] private float delayBeforeSpawn = 5f; // Задержка перед спавном

        private Rigidbody _rb;
        private bool _isDestroying = false;

        [SerializeField] private bool scaleWithMainObject = true;

        private bool isRbMassOvveride = true;
        private int ovverideMassIndex = 0; //(1)мелкие объекты 150 // (2)столбы, деревья 800 // (3)отбойники 1500
        private int[] masses = new int[] { 150, 800, 1500 };

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            
            if (_rb != null)
            {
                StartCoroutine(DelayedSpawnAndDestroy());
            }
        }

        private IEnumerator DelayedSpawnAndDestroy()
        {
            yield return new WaitForSeconds(delayBeforeSpawn);
            
            if (!_isDestroying)
            {
                SpawnAndDestroy();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(carTag)) return;
            
            // Если есть Rigidbody, прерываем корутину и сразу выполняем
            if (_rb != null)
            {
                StopAllCoroutines();
            }
            SpawnAndDestroy();
        }

        private void SpawnAndDestroy()
        {
            if (_isDestroying) return;
            _isDestroying = true;
            
            if (_rbProp == null)
            {
                Debug.LogError("RB Prop prefab is not assigned!", this);
                return;
            }

            GameObject propInstance = Instantiate(
                _rbProp,
                transform.position,
                spawnWithRotation ? transform.rotation : Quaternion.identity
            );

            if (scaleWithMainObject)
                propInstance.transform.localScale = transform.localScale;

            if (isRbMassOvveride && propInstance.TryGetComponent<Rigidbody>(out Rigidbody propRigidbody))
            {
                if (ovverideMassIndex > masses.Length - 1)
                    ovverideMassIndex = 0;

                propRigidbody.mass = masses[ovverideMassIndex];
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            if (_rbProp != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(transform.position, _rbProp.GetComponent<Collider>().bounds.size);
            }
        }
    }
}
