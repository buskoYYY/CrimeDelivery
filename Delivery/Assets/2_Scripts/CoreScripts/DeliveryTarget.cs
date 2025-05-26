using UnityEngine;

public class DeliveryTarget : MonoBehaviour
{
    public DeliveryController deliveryController;
    public int deliveryTargetIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        CarComponentsController car = other.GetComponentInParent<CarComponentsController>();
        if (car != null)
        {
            if (car.isPlayer)
            {
                deliveryController.Delivered(deliveryTargetIndex);
            }
        }    
    }
}
