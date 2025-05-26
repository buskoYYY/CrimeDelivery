using UnityEngine;
using DestroyIt;

namespace ArcadeBridge
{
    public class BustEffect : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string carTag = "Car";
        [SerializeField] private bool destroyAfterEffect = true;
        [SerializeField] private ParticleSystem healEffect;
        //[SerializeField] private float healAmount = 50f; // Количество восстанавливаемого HP
        
        private bool _isProcessing;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(carTag)) return;
            if (_isProcessing) return;
            
            _isProcessing = true;

            // Получаем Destructible у объекта, который вошел в триггер
            CarDamageHandler carDamageHandler = other.GetComponentInParent<CarDamageHandler>();
            if (carDamageHandler != null)
            {
                FullHeal(carDamageHandler);
            }
            
            if (destroyAfterEffect)
                Destroy(gameObject, 0.1f);
        }

        private void FullHeal(CarDamageHandler carDamageHandler)
        {
            // Прямое восстановление через поля
            //carDamageHandler.currentHealth = carDamageHandler.maxHealth;

            // Визуальный эффект
            if (healEffect != null)
            {
                ParticleSystem effect = Instantiate(healEffect,
                                                  carDamageHandler.transform.position, 
                                                  Quaternion.identity);
                effect.transform.SetParent(carDamageHandler.transform);
            }
        }

        private void OnDrawGizmos()
        {
            if (healEffect != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }
        }
    }
}
