using UnityEngine;

namespace ArcadeBridge
{
    public class BoostJump : MonoBehaviour
    {
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private ForceMode jumpForceMode = ForceMode.Impulse;

        [Header("Effect Settings")]
        [SerializeField] private ParticleSystem jumpEffect;
        //[SerializeField] private AudioClip jumpSound;
        [SerializeField] private float soundVolume = 0.7f;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("BoostCar requires a Rigidbody component!");
            }
        }

        /// <summary>
        /// Public method to make the object jump
        /// </summary>
        public void Jump()
        {
            if (rb == null) return;

            // Apply jump force
            rb.AddForce(Vector3.up * jumpForce, jumpForceMode);

            // Play effects
           PlayJumpEffects();
        }

        private void PlayJumpEffects()
        {
            // Visual effect
            if (jumpEffect != null)
            {
                Instantiate(jumpEffect, transform.position, Quaternion.identity);
            }

            // Sound effect
            /*if (jumpSound != null)
            {
                AudioSource.PlayClipAtPoint(jumpSound, transform.position, soundVolume);
            }*/
        }

        // Visualize jump force in editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector3.up * 2f);
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.2f);
        }
    }
}
