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
        [SerializeField] private float healAmount = 50f; // Количество восстанавливаемого HP
        
        private bool _isProcessing;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(carTag)) return;
            if (_isProcessing) return;
            
            _isProcessing = true;
            
            // Получаем Destructible у объекта, который вошел в триггер
            Destructible targetDestructible = other.GetComponentInParent<Destructible>();
            if (targetDestructible != null)
            {
                FullHeal(targetDestructible);
            }
            
            if (destroyAfterEffect)
                Destroy(gameObject, 0.1f);
        }

        private void FullHeal(Destructible destructible)
        {
            // Прямое восстановление через поля
            destructible._currentHitPoints = destructible._totalHitPoints;

            // Визуальный эффект
            if (healEffect != null)
            {
                ParticleSystem effect = Instantiate(healEffect, 
                                                  destructible.transform.position, 
                                                  Quaternion.identity);
                effect.transform.SetParent(destructible.transform);
            }
            
            Debug.Log($"{destructible.gameObject.name} fully healed! " +
                     $"HP: {destructible._currentHitPoints}/{destructible._totalHitPoints}");
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
