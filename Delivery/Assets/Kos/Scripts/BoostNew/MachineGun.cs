using UnityEngine;

namespace ArcadeBridge
{
    public class MachineGun : MonoBehaviour
    {
        [Header("Tracking Settings")]
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float maxDetectionDistance = 50f;

        [Header("Effects Settings")]
        [SerializeField] private ParticleSystem gunParticles;
        [SerializeField] private float particlesActivationDelay = 0.2f;

        [Header("Damage Settings")]
        [SerializeField] private float damagePerSecond = 5f;
        [SerializeField] private float damageInterval = 0.2f;

        private Transform currentTarget;
        private AIDriftController targetDriftController;
        private CarDamageHandler targetDamageHandler;
        private float targetDetectionTime;
        private bool wasTargetDetected;
        private float lastDamageTime;

        private void Update()
        {
            FindNearestCar();
            RotateTowardsTarget();
            HandleParticles();
            ApplyDamageToTarget();
        }

        private void FindNearestCar()
        {
            GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
            float closestDistance = Mathf.Infinity;
            Transform closestCar = null;
            bool targetFound = false;

            foreach (GameObject car in cars)
            {
                float distance = Vector3.Distance(transform.position, car.transform.position);

                if (distance < closestDistance && distance <= maxDetectionDistance)
                {
                    AIDriftController driftController = car.GetComponent<AIDriftController>();
                    CarDamageHandler damageHandler = car.GetComponentInParent<CarDamageHandler>();

                    if (driftController != null && damageHandler != null)
                    {
                        closestDistance = distance;
                        closestCar = car.transform;
                        targetDriftController = driftController;
                        targetDamageHandler = damageHandler;
                        targetFound = true;
                    }
                }
            }

            if (targetFound && !wasTargetDetected)
            {
                targetDetectionTime = Time.time;
            }

            currentTarget = closestCar;
            wasTargetDetected = targetFound;
        }

        private void RotateTowardsTarget()
        {
            if (currentTarget == null) return;

            Vector3 direction = currentTarget.position - transform.position;
            direction.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        private void HandleParticles()
        {
            if (gunParticles == null) return;

            if (wasTargetDetected && currentTarget != null)
            {
                if (Time.time >= targetDetectionTime + particlesActivationDelay)
                {
                    if (!gunParticles.isPlaying)
                    {
                        gunParticles.Play();
                    }
                }
            }
            else
            {
                if (gunParticles.isPlaying)
                {
                    gunParticles.Stop();
                }
            }
        }

        private void ApplyDamageToTarget()
        {
            if (!wasTargetDetected || targetDamageHandler == null) return;
            if (Time.time < lastDamageTime + damageInterval) return;

            // Наносим урон с учетом временного интервала
            float calculatedDamage = damagePerSecond * damageInterval;
            targetDamageHandler.ApplyDamage(calculatedDamage);
            lastDamageTime = Time.time;
        }
    }
}
