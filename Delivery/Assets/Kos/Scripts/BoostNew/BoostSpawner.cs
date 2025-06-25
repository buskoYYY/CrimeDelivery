using UnityEngine;

namespace ArcadeBridge
{
    public class BoostSpawner : MonoBehaviour
    {
        [Header("Weapon Settings")]
        [SerializeField] private GameObject tipWeapon;

        [Header("Spawn Position Settings")]
        [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0f, 1f);
        [SerializeField] private bool useLocalSpace = true;
        [SerializeField] private bool makeChildIfLocal = true; // Новый параметр

        [Header("Spawn Rotation Settings")]
        [SerializeField] private bool useCustomRotation = false;
        [SerializeField] private Vector3 customRotationEuler = Vector3.zero;

        public void SpawnWeapon()
        {
            if (!ValidateWeapon()) return;

            Vector3 spawnPosition = CalculateSpawnPosition();
            Quaternion spawnRotation = CalculateSpawnRotation();

            GameObject spawnedWeapon = Instantiate(
                tipWeapon,
                spawnPosition,
                spawnRotation
            );

            // Делаем дочерним, если нужно
            if (useLocalSpace && makeChildIfLocal)
            {
                spawnedWeapon.transform.SetParent(transform);
            }

            spawnedWeapon.name = tipWeapon.name;
        }

        private Vector3 CalculateSpawnPosition()
        {
            if (useLocalSpace)
            {
                return transform.position +
                       transform.right * spawnOffset.x +
                       transform.up * spawnOffset.y +
                       transform.forward * spawnOffset.z;
            }
            else
            {
                return transform.position + spawnOffset;
            }
        }

        private Quaternion CalculateSpawnRotation()
        {
            return useCustomRotation
                ? transform.rotation * Quaternion.Euler(customRotationEuler)
                : transform.rotation;
        }

        private bool ValidateWeapon()
        {
            if (tipWeapon == null)
            {
                Debug.LogWarning("Weapon prefab is null!");
                return false;
            }
            return true;
        }

        // Для визуализации в редакторе
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(CalculateSpawnPosition(), 0.1f);
        }
    }
}
