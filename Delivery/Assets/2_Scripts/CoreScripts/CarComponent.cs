using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CarComponent: MonoBehaviour
{
    public bool isActiveComponent = true;
    public CarComponentsController carComponents;

    public virtual void SetupComponent(CarComponentsController carComponentsController)
    {
        carComponents = carComponentsController;
    }

    public virtual void StartRace()
    { }
    public virtual void FinishRace()
    { }

    public virtual void UpdateHealth(float carHeath, float maxHealth)
    { }

    public virtual void CarDestroy()
    { }

    public virtual void CarReturnToLive()
    {
        
    }
}
