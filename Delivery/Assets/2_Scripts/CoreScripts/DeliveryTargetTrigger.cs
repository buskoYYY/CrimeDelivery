using UnityEngine;
public class DeliveryTargetTrigger : MonoBehaviour
{
    private DeliveryTarget deliveryTarget;

    private void Awake()
    {
        deliveryTarget = GetComponentInParent<DeliveryTarget>();
    }

    private void OnTriggerEnter(Collider other)
    {
        deliveryTarget.Deliver(other);
    }
}
