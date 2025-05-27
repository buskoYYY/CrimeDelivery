using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleCarDriftController : Vehicle
{
    private CarDriftController car;
    private TURBO_Suspension suspension;

    public DriftControllerSettings carSettingsAtStart;

    public float boostValue = 3;
    public float reduceRotationInBoost = 2;

    private float boostGripZ;

    private float boostGripX;

    private bool stun;


    public override void SetupComponent(CarComponentsController carComponentsController)
    {
        base.SetupComponent(carComponentsController);
        car = GetComponent<CarDriftController>();
        suspension = GetComponent<TURBO_Suspension>();
        car.isGroundedSelf = false;
        UpdateCarStats(car.carSettings);
        initialized = true;
    }

    [SerializeField] private int wheelsIsGrounded;
    private bool onTwoWeels = false;
    private void FixedUpdate()
    {
        if (initialized)
        {
            isGrounded = suspension.IsGroundedForAllCar(2);
            car.isGrounded = isGrounded;
        }
    }

    public override void CarDestroy()
    {
        base.CarDestroy();
        car.isGroundedSelf = false;
        car.isGrounded = false;
    }

    public void UpdateCarStats(DriftControllerSettings newSettings)
    {
        carSettingsAtStart = newSettings.Clone();

    }

    public override void StartRace()
    {
    }

    public override void FinishRace()
    {

    }



    public override void ChangeThrottle(float gass)
    {
        base.ChangeThrottle(gass);
        car.inThrottle = InThrottle;
    }

    public override void ChangeTurn(float turn)
    {
        base.ChangeTurn(turn);
        car.inTurn = InTurn;
    }

    public override void ActivateBoost()
    {
        base.ActivateBoost();
        car.carSettings.Accel = carSettingsAtStart.Accel * boostValue;
        car.carSettings.RotateAtStart = carSettingsAtStart.RotateAtStart / reduceRotationInBoost;
    }

    public override void DeactivateBoost()
    {
        base.DeactivateBoost();
        car.carSettings.Accel = carSettingsAtStart.Accel;
        car.carSettings.RotateAtStart = carSettingsAtStart.RotateAtStart;
    }

    public override void StrongHit()
    {
        if (stunCorutine == null)
            stunCorutine = StartCoroutine(StunCorutine());
        else
        {
            StopCoroutine(stunCorutine);
            stunCorutine = StartCoroutine(StunCorutine());
        }
    }

    private Coroutine stunCorutine;
    private IEnumerator StunCorutine()
    {
        stun = true;
        yield return new WaitForSeconds(0.7f);
        stun = false;
    }
}
