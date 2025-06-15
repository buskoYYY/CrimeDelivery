using UnityEngine;

namespace ArcadeBridge
{
    public class ShieldEffect : MonoBehaviour
    {
        [Header("Physics Settings")]
        [SerializeField] private string carTag = "Car";
        [SerializeField] private float pushForce = 10f;
        [SerializeField] private float upwardAngle = 45f;
        [SerializeField] private ForceMode forceMode = ForceMode.Impulse;

        [Header("Effect Settings")]
        [SerializeField] private float destroyTime = 10f;
        [SerializeField] private bool destroyAfterEffect = true;
        [SerializeField] private ParticleSystem impactEffect;
        [SerializeField] private AudioClip impactSound;
        [SerializeField] private float soundVolume = 0.7f;

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

            ProcessCollision(other);
        }

        private void ProcessCollision(Collider other)
        {
            // Try to get Rigidbody in parent if not found on current object
            Rigidbody carRigidbody = other.GetComponentInParent<Rigidbody>();
            if (carRigidbody == null) return;

            Vector3 collisionPoint = other.ClosestPoint(transform.position);
            Vector3 pushDirection = CalculatePushDirection(other.transform);

            ApplyForce(carRigidbody, pushDirection);
            PlayEffects(collisionPoint);
        }

        private Vector3 CalculatePushDirection(Transform carTransform)
        {
            Vector3 direction = (carTransform.position - transform.position).normalized;

            // Convert angle to radians
            float angleRad = upwardAngle * Mathf.Deg2Rad;

            // Create upward angled direction
            Vector3 angledDirection = new Vector3(
                direction.x * Mathf.Cos(angleRad),
                Mathf.Sin(angleRad),
                direction.z * Mathf.Cos(angleRad)
            );

            return angledDirection.normalized;
        }

        private void ApplyForce(Rigidbody rb, Vector3 direction)
        {
            rb.AddForce(direction * pushForce, forceMode);
        }

        private void PlayEffects(Vector3 position)
        {
            // Visual effect
            if (impactEffect != null)
            {
                Instantiate(impactEffect, position, Quaternion.identity);
            }

            // Sound effect
            if (impactSound != null)
            {
                AudioSource.PlayClipAtPoint(impactSound, position, soundVolume);
            }
        }

        // Visualize push direction in editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector3 direction = CalculatePushDirection(transform);
            Gizmos.DrawRay(transform.position, direction * 3f);
            Gizmos.DrawWireSphere(transform.position + direction * 3f, 0.2f);
        }
    }
}
