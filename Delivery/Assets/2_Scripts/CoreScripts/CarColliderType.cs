using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderType { Front, Back, Side, Roof };

public class CarColliderType : MonoBehaviour
{
    public ColliderType colliderType;
    public CarComponentsController carComponentsController;

    private void Start()
    {
        carComponentsController = GetComponentInParent<CarComponentsController>();
    }

    public void HitCarCollider(HitInfo hitInfo)
    {
        carComponentsController.carDamageHandler.HitCar(hitInfo);
    }
}
