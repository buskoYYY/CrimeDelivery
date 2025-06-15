using UnityEngine;

namespace ArcadeBridge
{
    public class BoostShield : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string carTag = "Car";
        [SerializeField] private bool destroyAfterEffect = true;
        [SerializeField] private GameObject shieldEffect;
        private bool _isProcessing;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(carTag) || _isProcessing) return;
            _isProcessing = true;

            Transform carParent = other.transform.root;
            if (carParent != null) carParent = carParent.transform;

            // Создаём эффект щита и делаем его дочерним к родителю машины
            if (shieldEffect != null && carParent != null)
            {
                GameObject shieldInstance = Instantiate(
                    shieldEffect,
                    carParent.position,
                    carParent.rotation,
                    carParent
                );
            }
           /*     // Создаём эффект щита и делаем его дочерним к машине
                if (shieldEffect != null)
            {
                GameObject shieldInstance = Instantiate(shieldEffect,
                    other.transform.position,
                    other.transform.rotation,
                    other.transform); // Делаем дочерним
            }*/

            // Уничтожаем сам объект BoostShield (опционально)
            if (destroyAfterEffect)
                Destroy(gameObject);
        }
    }
}
