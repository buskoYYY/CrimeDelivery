using UnityEngine;

public class AIDriftController : CarComponent
{
    [SerializeField] private float distanceToTarget;
    [SerializeField] private VehicleCarDriftController vehicleCarDriftController;
    [SerializeField] private CarDriftController carDriftController;
    [SerializeField] private Driver driver;

    [SerializeField] private float accelChangerMin = 1f;
    [SerializeField] private float accelChangerMax = 20f;
    [SerializeField] private float accelChanger;
    [SerializeField] private float rotVelChanger = 0.3f;
    [SerializeField] private float newTargetOffcetAtStart;

    [SerializeField] private float newTargetOffcetMin = 1;
    [SerializeField] private float newTargetOffcetMax = 10;
    [SerializeField] private float newTargetOffcet;

    [SerializeField] private float maxDistance = 5;

    public bool autoDestroy = false;


    public override void CarDestroy()
    {
        base.CarDestroy();
    }

    public override void SetupComponent(CarComponentsController carComponents)
    {
        base.SetupComponent(carComponents);
        vehicleCarDriftController = GetComponent<VehicleCarDriftController>();
        carDriftController = GetComponent<CarDriftController>();
        driver = GetComponent<Driver>();
        accelChanger = Random.Range(accelChangerMin, accelChangerMax);
        newTargetOffcetAtStart = driver.targetOffset;
        newTargetOffcet = Random.Range(newTargetOffcetMin, newTargetOffcetMax);
    }

    private void FixedUpdate()
    {
        distanceToTarget = Vector3.Distance(transform.position, driver.TargetObjectTransform.position);
        if (distanceToTarget < maxDistance)
        {
            carDriftController.carSettings.Accel = vehicleCarDriftController.carSettingsAtStart.Accel - accelChanger;
            carDriftController.carSettings.RotVel = vehicleCarDriftController.carSettingsAtStart.RotVel;
            driver.targetOffset = newTargetOffcet;
        }
        else
        {
            driver.targetOffset = newTargetOffcetAtStart;
            carDriftController.carSettings.Accel = vehicleCarDriftController.carSettingsAtStart.Accel + (accelChanger * 2);
            carDriftController.carSettings.RotVel = vehicleCarDriftController.carSettingsAtStart.RotVel + rotVelChanger;
        }

        if (autoDestroy)
        {
            DestroyByDistance(distanceToTarget);
            DestroyByStuck();
        }
    }

    [SerializeField] private float distanceToDestroy = 30;
    private void DestroyByDistance(float distanceToPlayer)
    {
        if (distanceToPlayer > distanceToDestroy)
        {
            carComponents.carDamageHandler.ApplyDamage(999999999);
        }
    }

    [SerializeField] private float stuckDestroyTime = 5;
    private float stuckTimer;
    private void DestroyByStuck()
    {
        if (carComponents.carRigidbody.linearVelocity.magnitude < 2)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckDestroyTime)
            {
                carComponents.carDamageHandler.ApplyDamage(999999999);
            }
        }
        else
        {
            stuckTimer = 0;
        }
    }
}
