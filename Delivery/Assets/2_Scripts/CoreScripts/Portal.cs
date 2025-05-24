using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform spawnPosition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Car")
        {
            CarComponentsController carToTranslate = other.GetComponentInParent<CarComponentsController>();
            carToTranslate.carTrasform.position = spawnPosition.position;
            carToTranslate.carTrasform.rotation = spawnPosition.rotation;
        }
    }
}
