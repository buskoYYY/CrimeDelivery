using System.Collections;
using UnityEngine;

namespace ArcadeBridge
{
    public class BoostFire : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField] private string carTag = "Car";
        [SerializeField] private float damage = 100f;
        [SerializeField] private float effectDuration = 0.5f; // время горения машины

        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem fireEffect;
        [SerializeField] private Vector3 effectOffset = Vector3.zero;

        [Header("Behavior Settings")]
        [SerializeField] private bool destroyAfterEffect = true;

        [SerializeField] private float destroyTime = 60f; // время действия ауры

        private ParticleSystem _activeFireEffect;

        private void Start()
        {
            if (destroyAfterEffect)
            {
                Destroy(gameObject, destroyTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(carTag)) return;

            CarDamageHandler carDamageHandler = other.GetComponentInParent<CarDamageHandler>();
            if (carDamageHandler == null) return;

            AttachFireEffect(carDamageHandler);
            StartCoroutine(ApplyDamageOverTime(carDamageHandler));
        }

        private IEnumerator ApplyDamageOverTime(CarDamageHandler carDamageHandler)
        {
            yield return new WaitForSeconds(effectDuration);

            carDamageHandler.ApplyHealth(-damage);

            CleanUp();
        }

        private void AttachFireEffect(CarDamageHandler carDamageHandler)
        {
            if (fireEffect != null)
            {
                _activeFireEffect = Instantiate(
                    fireEffect,
                    carDamageHandler.transform.position + effectOffset,
                    Quaternion.identity,
                    carDamageHandler.transform
                );
                _activeFireEffect.Play();
            }
        }

        private void CleanUp()
        {
            if (_activeFireEffect != null)
            {
                _activeFireEffect.Stop();
                Destroy(_activeFireEffect.gameObject, _activeFireEffect.main.duration);
            }
        }

        private void OnDestroy()
        {
            if (_activeFireEffect != null)
            {
                Destroy(_activeFireEffect.gameObject);
            }
        }
    }
}
