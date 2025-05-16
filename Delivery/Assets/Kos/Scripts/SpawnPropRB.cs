using UnityEngine;

namespace ArcadeBridge
{
    [RequireComponent(typeof(Collider))] // Гарантируем наличие коллайдера
    public class SpawnPropRB : MonoBehaviour
    {
        [SerializeField] private GameObject _rbProp; // Префаб для спавна
        [SerializeField] private string playerTag = "Player"; // Тег игрока
        [SerializeField] private bool spawnWithRotation = true; // Сохранять вращение
        
        private void OnTriggerEnter(Collider other)
        {
            // Проверяем тег столкнувшегося объекта
            if (!other.CompareTag(playerTag)) return;
            
            SpawnAndDestroy();
        }

        private void SpawnAndDestroy()
        {
            if (_rbProp == null)
            {
                Debug.LogError("RB Prop prefab is not assigned!", this);
                return;
            }

            // Спавним новый объект
            Instantiate(
                _rbProp, 
                transform.position, 
                spawnWithRotation ? transform.rotation : Quaternion.identity
            );

            // Удаляем текущий объект
            Destroy(gameObject);
        }

        // Для визуализации в редакторе
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
