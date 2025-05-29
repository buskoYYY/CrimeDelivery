using UnityEngine;

public class DeliveryTarget : MonoBehaviour
{
    public DeliveryController deliveryController;
    public int deliveryTargetIndex = 0;
    public int deliveryReward = 100;
    public GameObject deliveryTargetVisual;
    [SerializeField] private bool delivered;

    private void OnTriggerEnter(Collider other)
    {
        CarComponentsController car = other.GetComponentInParent<CarComponentsController>();
        if (car != null)
        {
            if (car.isPlayer && !delivered)
            {
                delivered = true;
                deliveryController.Delivered(deliveryTargetIndex, deliveryReward);
                deliveryTargetVisual.SetActive(false);
            }
        }    
    }
}
