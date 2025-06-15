using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    public class BustHealth : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string carTag = "Car";
        [SerializeField] private bool destroyAfterEffect = true;
        [SerializeField] private ParticleSystem healEffect;
        [SerializeField] private float healAmount = 100f; // Количество восстанавливаемого HP

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
                AddHealth(carDamageHandler);
            }
            
            if (destroyAfterEffect)
                Destroy(gameObject, 0.1f);
        }

        private void AddHealth(CarDamageHandler carDamageHandler)
        {
            float _hp = carDamageHandler.MaxHealth - carDamageHandler.CurrentHealth;
            if (healAmount > _hp)
            {
                carDamageHandler.ApplyHealth(_hp);
            }
            else
            carDamageHandler.ApplyHealth(healAmount);

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
