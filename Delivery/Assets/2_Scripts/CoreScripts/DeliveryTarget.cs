using System.Collections.Generic;
using UnityEngine;

public class DeliveryTarget : MonoBehaviour
{
    public DeliveryController deliveryController;
    public int deliveryTargetIndex = 0;
    public int unloadCount = 0;
    public int deliveryReward = 100;
    public GameObject deliveryTargetVisual;

    [Header("������� ��� ��������")]
    public List<GameObject> objectsToLoad = new List<GameObject>();
    public Transform unLoadPosition;

    public bool load;
    private bool unLoaded;

    private void Start()
    {
        deliveryController = FindFirstObjectByType<DeliveryController>();
    }

    public void LoadUnload(CarComponentsController target)
    {
        deliveryTargetVisual.SetActive(false);

        TruckLoader loader = target.GetComponentInParent<TruckLoader>();
        if (loader != null)
        {
            if (load)
                loader.LoadObjects(objectsToLoad);
            else
            {
                if (!unLoaded)
                {
                    unLoaded = true;
                    loader.UnloadObjects(unLoadPosition, unloadCount);
                }

            }
        }
    }

    public void Deliver(Collider other)
    {
        CarComponentsController car = other.GetComponentInParent<CarComponentsController>();
        if (car != null)
        {
            if (car.isPlayer && deliveryController.CanDeliver(deliveryTargetIndex))
            {
                LoadUnload(car);
                deliveryController.Delivered(deliveryTargetIndex, deliveryReward);
            }
        }
    }
}
