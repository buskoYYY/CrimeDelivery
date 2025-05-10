using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wheel_Friction
{
    public Transform wheelTransform;
    public float turnAngle = 30;
    public float frictionCoefficient = 9000;
    public Wheel_Suspension wheel_Suspension;
}

public class TURBO_WheelFriction : Vehicle
{
    public enum Drive
    {
        FWD, RWD, AWD
    }

    public Drive driveMode = Drive.AWD;
    public TURBO_Suspension suspension;
    public List<Wheel_Friction> steeringWheels = new List<Wheel_Friction>();
    public List<Wheel_Friction> rearWheels = new List<Wheel_Friction>();
    [SerializeField] private List<Wheel_Friction> allWheels = new List<Wheel_Friction>();
    public bool localSetupWheel;

    [Header("Grip")]
    public float setup_frictionCoefficient = 9000;
    public float setup_turnAngle = 30;
    public AnimationCurve slipCurve;
    public float maxSpeed = 50f;

    [Header("Accel")]
    public NitroSystem nitroSystem;
    public AnimationCurve accelerationCurve;
    public float accel;
    public float nitroAccelMultiplier = 1.5f;
    public float nitroGainRate = 10;

    [Header("Other")]
    private float accelInput;
    private float turnInput;


    private Rigidbody carRigidbody;
    
    private void Awake()
    {
        carRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (!localSetupWheel)
        {
            for (int i = 0; i < suspension.steeringWheels.Length; i++)
            {
                steeringWheels.Add(SetupWheel(suspension.steeringWheels[i]));
            }

            for (int i = 0; i < suspension.rearWheels.Length; i++)
            {
                rearWheels.Add(SetupWheel(suspension.rearWheels[i]));
            }

            allWheels.AddRange(steeringWheels);
            allWheels.AddRange(rearWheels);
            SetupAllWheels();
        }
    }

    private void OnValidate()
    {
        SetupAllWheels();
    }

    public void SetupAllWheels()
    {
        for (int i = 0; i < allWheels.Count; i++)
        {
            allWheels[i].frictionCoefficient = setup_frictionCoefficient;
            allWheels[i].turnAngle = setup_turnAngle;
        }
    }

    private Wheel_Friction SetupWheel(Wheel_Suspension wheelSuspension)
    {
        Wheel_Friction wheelToSetup = new Wheel_Friction();
        wheelToSetup.wheelTransform = wheelSuspension.wheel;
        wheelToSetup.wheel_Suspension = wheelSuspension;
        return wheelToSetup;
    }

    private void FixedUpdate()
    {
        CarInput();
        for (int i = 0; i < allWheels.Count; i++)
        {
            AddLateralFriction(allWheels[i], allWheels[i].wheel_Suspension.isGrounded, i);
        }
        TiresTurn();

        switch (driveMode)
        {
            case Drive.AWD:
                foreach (Wheel_Friction wheel in allWheels)
                {
                    Accel(wheel.wheelTransform);
                }
                break;

            case Drive.FWD:
                foreach (Wheel_Friction wheel in steeringWheels)
                {
                    Accel(wheel.wheelTransform);
                }
                break;

            case Drive.RWD:
                foreach (Wheel_Friction wheel in rearWheels)
                {
                    Accel(wheel.wheelTransform);
                }
                break;
        }
    }
    public void AddLateralFriction(Wheel_Friction wheel, bool wheelIsGrounded, int wheelID)
    {
        if (wheelIsGrounded)
        {
            //Vector3 surfaceNormal = wheelHit.normal;
            Vector3 sideVelocity = (wheel.wheelTransform.InverseTransformDirection(carRigidbody.GetPointVelocity(wheel.wheelTransform.position)).x) * wheel.wheelTransform.right;
            Vector3 forwardVelocity = (wheel.wheelTransform.InverseTransformDirection(carRigidbody.GetPointVelocity(wheel.wheelTransform.position)).z) * wheel.wheelTransform.forward;
            Vector3 localVelocity = wheel.wheelTransform.InverseTransformDirection(carRigidbody.GetPointVelocity(wheel.wheelTransform.position));

            float angle = Vector3.Angle(carRigidbody.GetPointVelocity(wheel.wheelTransform.position), wheel.wheelTransform.right);
            float angleDifference = Mathf.Abs(angle - 90) / 90;
            float slipCoef = slipCurve.Evaluate(angleDifference);
            Vector3 frictionForce = -sideVelocity.normalized * wheel.frictionCoefficient* slipCoef;

            if (sideVelocity.magnitude > forwardVelocity.magnitude)
            {
                AddNitro(nitroGainRate * angleDifference);
            }

            carRigidbody.AddForceAtPosition(frictionForce, wheel.wheelTransform.position);

        }

    }

    public float angleDifferenceNew;
    public float angleNew;
    public float steerSpeed = 100f;
    public void TiresTurn()
    {
        angleNew = Vector3.Angle(carRigidbody.GetPointVelocity(transform.position), transform.right);
        angleDifferenceNew = 1 - Mathf.Abs(angleNew - 90) / 90;
        if (turnInput < 0 && angleNew < 90)
        {
            angleDifferenceNew = 1 - Mathf.Abs(angleNew - 90) / 90;
        }
        else if (turnInput > 0 && angleNew < 90)
        {
            angleDifferenceNew = 1;
        }
        else if (turnInput > 0 && angleNew > 90)
        {
            angleDifferenceNew = 1 - Mathf.Abs(angleNew - 90) / 90;
        }
        else if (turnInput < 0 && angleNew > 90)
        {
            angleDifferenceNew = 1;
        }
        else
        {
            angleDifferenceNew = 99;
        }

        for (int i = 0; i < steeringWheels.Count; i++)
        {
            // Целевой угол колеса
            float targetAngle = steeringWheels[i].turnAngle * turnInput * angleDifferenceNew;

            // Текущий поворот колеса
            Quaternion currentRotation = steeringWheels[i].wheelTransform.localRotation;

            // Желаемый поворот
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            steeringWheels[i].wheelTransform.localRotation = Quaternion.RotateTowards(
                        currentRotation,
                        targetRotation,
                        steerSpeed * Time.fixedDeltaTime
            );  //Quaternion.Euler(0, steeringWheels[i].turnAngle * turnInput * angleDifferenceNew, 0);
        }
    }

    public void Accel(Transform wheel)
    {
        Vector3 forwardVelocity = (wheel.InverseTransformDirection(carRigidbody.GetPointVelocity(wheel.position)).z) * wheel.forward;

        float acceleration = accel;

        if (nitroSystem != null && nitroSystem.IsUsingNitro)
        {
            acceleration *= nitroAccelMultiplier;
        }
       
        float speedRatio = Mathf.Clamp01(forwardVelocity.magnitude / maxSpeed);
        float accelMultiplier = accelerationCurve.Evaluate(speedRatio);
        carRigidbody.AddForceAtPosition(wheel.forward * accelInput * acceleration * accelMultiplier, wheel.position, ForceMode.Acceleration);
    }

    public bool selfInput;
    public void CarInput()
    {
        if (selfInput)
        {
            turnInput = Input.GetAxis("Sideways");
            accelInput = Input.GetAxis("Throttle");
        }
    }

    public override void ChangeTurn(float turn)
    {
        base.ChangeTurn(turn);
        turnInput = turn; // 
    }

    public override void ChangeThrottle(float gass)
    {
        base.ChangeThrottle(gass);
        accelInput = gass;
    }

    public void AddNitro(float nitroGain)
    {
        nitroSystem.AddNitro(nitroGain * Time.deltaTime);
    }
}
