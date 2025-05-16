using System.Collections.Generic;
using UnityEngine;

public class PoliceActivator : MonoBehaviour
{
    public List<Driver> policeCars = new List<Driver>();
    private void OnTriggerEnter(Collider other)
    {
        CarComponentsController playerGameObject = other.GetComponentInParent<CarComponentsController>();
        if (playerGameObject != null)
        {
            if (playerGameObject.isPlayer)
            {
                foreach (Driver police in policeCars)
                {
                    police.ChangeTarget(playerGameObject.carTrasform);
                    police.Throttle(1);
                }
            }

        }
    }

}
