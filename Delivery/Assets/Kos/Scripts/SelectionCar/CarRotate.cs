using UnityEngine;

namespace ArcadeBridge
{
    public class CarRotate : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 90f; // Скорость вращения в градусах/секунду
        [SerializeField] private bool clockwise = true; // Направление вращения
        
        void Update()
        {
            // Вычисляем направление вращения (1 или -1)
            float direction = clockwise ? 1f : -1f;
            
            // Вращаем объект по оси Y
            transform.Rotate(Vector3.up, rotationSpeed * direction * Time.deltaTime);
        }
    }
}
