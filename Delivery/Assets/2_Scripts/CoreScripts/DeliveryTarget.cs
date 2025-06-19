using System.Collections.Generic;
using UnityEngine;

public class DeliveryTarget : MonoBehaviour
{
    public DeliveryController deliveryController;
    public int deliveryTargetIndex = 0;
    public int deliveryReward = 100;
    public GameObject deliveryTargetVisual;

    [Header("Объекты для погрузки")]
    public List<GameObject> objectsToLoad = new List<GameObject>();
    public Transform unLoadPosition;

    public bool load;
    private bool unLoaded;

    private void Start()
    {
        deliveryController = FindFirstObjectByType<DeliveryController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        CarComponentsController car = other.GetComponentInParent<CarComponentsController>();
        if (car != null)
        {
            if (car.isPlayer && deliveryController.CanDeliver(deliveryTargetIndex))
            {
                deliveryController.Delivered(deliveryTargetIndex, deliveryReward);
                deliveryTargetVisual.SetActive(false);

                TruckLoader loader = other.GetComponentInParent<TruckLoader>();
                if (loader != null)
                {
                    if (load)
                        loader.LoadObjects(objectsToLoad);
                    else
                    {
                        if (!unLoaded)
                        {
                            unLoaded = true;
                            //loader.UnloadObjects(unLoadPosition);
                        }

                    }
                }
            }
        }    
    }
}
