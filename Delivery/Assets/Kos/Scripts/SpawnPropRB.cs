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

            Instantiate(
                _rbProp, 
                transform.position, 
                spawnWithRotation ? transform.rotation : Quaternion.identity
            );

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
