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
    [SerializeField] private float targetOffcetFarFromPlayer;

    [SerializeField] private float targetOffcetMin = 1;
    [SerializeField] private float targetOffcetMax = 10;
    [SerializeField] private float targetOffcet;

    [SerializeField] private float playerShowRadius = 5;

    public bool autoDestroy = false;


    public override void CarDestroy()
    {
        base.CarDestroy();
    }

    [SerializeField] private bool setupAtSetupComponent;


    public override void StartRace()
    {
        base.StartRace();
    }

    public override void SetupComponent(CarComponentsController carComponents)
    {
        base.SetupComponent(carComponents);
        vehicleCarDriftController = GetComponent<VehicleCarDriftController>();
        carDriftController = GetComponent<CarDriftController>();
        driver = GetComponent<Driver>();
        if (setupAtSetupComponent)
            SetupCarAIConfig(accelChangerMin, accelChangerMax, rotVelChanger, targetOffcetMin, targetOffcetMax, targetOffcetFarFromPlayer);
    }

    private void FixedUpdate()
    {
        distanceToTarget = Vector3.Distance(transform.position, driver.TargetObjectTransform.position);
        if (distanceToTarget < playerShowRadius)
        {
            carDriftController.carSettings.Accel = vehicleCarDriftController.carSettingsAtStart.Accel - accelChanger;
            carDriftController.carSettings.RotVel = vehicleCarDriftController.carSettingsAtStart.RotVel;
            driver.targetOffset = targetOffcet;
        }
        else
        {
            carDriftController.carSettings.Accel = vehicleCarDriftController.carSettingsAtStart.Accel + (accelChanger * 2);
            carDriftController.carSettings.RotVel = vehicleCarDriftController.carSettingsAtStart.RotVel + rotVelChanger;
            driver.targetOffset = targetOffcetFarFromPlayer;
        }

        if (autoDestroy)
        {
            DestroyByDistance(distanceToTarget);
            DestroyByStuck();
        }
    }

    public void SetupCarAIConfig(float newAccelChagerMin, float newAccelChangerMax, float newRotVelChanger, float newTargetOffcetMin, 
        float newTargetOffcetMax, float newTargetOffcetFarFromPlayer)
    {
        accelChangerMin = newAccelChagerMin;
        accelChangerMax = newAccelChangerMax;
        accelChanger = Random.Range(accelChangerMin, accelChangerMax);

        rotVelChanger = newRotVelChanger;

        targetOffcetFarFromPlayer = newTargetOffcetFarFromPlayer;
        targetOffcetMin = newTargetOffcetMin;
        targetOffcetMax = newTargetOffcetMax;
        targetOffcet = Random.Range(targetOffcetMin, targetOffcetMax);
    }

    public float distanceToDestroy = 30;
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
