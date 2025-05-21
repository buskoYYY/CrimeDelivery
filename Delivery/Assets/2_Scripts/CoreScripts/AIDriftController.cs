using UnityEngine;

public class AIDriftController : CarComponent
{
    [SerializeField] private float distanceToTarget;
    [SerializeField] private CarComponentsController carComponents;
    [SerializeField] private VehicleCarDriftController vehicleCarDriftController;
    [SerializeField] private CarDriftController carDriftController;
    [SerializeField] private Driver driver;

    [SerializeField] private float accelChanger;
    [SerializeField] private float rotVelChanger = 0.3f;
    [SerializeField] private float newTargetOffcetAtStart;
    [SerializeField] private float newTargetOffcet;

    [SerializeField] private float maxDistance = 5;



    public override void SetupComponent()
    {
        vehicleCarDriftController = GetComponent<VehicleCarDriftController>();
        carDriftController = GetComponent<CarDriftController>();
        carComponents = GetComponent<CarComponentsController>();
        driver = GetComponent<Driver>();

        newTargetOffcetAtStart = driver.targetOffset;
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

        DestroyByDistance(distanceToTarget);
        DestroyByStuck();
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
