using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vehicle : CarComponent
{
    public float InTurn { get; private set; }
    public float InThrottle { get; private set; }
    public bool isGrounded;

    public delegate void OnNitro(bool onNitro);
    public event OnNitro OnNitroEvent;

    public virtual void ChangeTurn(float turn)
    {
        InTurn = turn;
    }

    public virtual void ChangeThrottle(float gass)
    {
        InThrottle = gass;
    }

    public virtual void ActivateBoost()
    {
        OnNitroEvent?.Invoke(true);
    }

    public virtual void DeactivateBoost()
    {
        OnNitroEvent?.Invoke(false);
    }

    public virtual void StrongHit()
    {

    }


}
