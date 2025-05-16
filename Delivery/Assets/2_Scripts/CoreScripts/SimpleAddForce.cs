using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAddForce : MonoBehaviour
{


    [Header("Accel")]
    [Space(5)]
    public float accel = 50;
    public float maxSpeed = 50;
    public float counterForceSpeedInDirection;
    public AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.1f);
    [Space(5)]
    public float speedInDirection;
    public float forwardSpeed;

    [Header("Steering")]
    [Space(5)]
    public float torqueForce = 100f;
    public float maxAngularVelocity = 3f; // ������������ �������� �������� ��������
    public float counterTorqueFactor = 50f; // ��������� ������ �������� ��� ����������

    [Header("Other")]
    [Space(5)]
    public bool selfInput;
    public float nitroGainRate = 1;
    public AnimationCurve slipCurve;
    [SerializeField] private float angularVelosity;
    [SerializeField] private float force;

    [SerializeField] private bool isGrounded;
    [SerializeField] private float distToGround = 2;
    [SerializeField] private float angle;
    [SerializeField] private float angleDifference;

    private Rigidbody carRigidbody;
    private Transform forcePosition;
    private NitroSystem nitroSystem;
    private float accelInput;
    private float turnInput;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        forcePosition = GetComponent<Transform>();
        nitroSystem = GetComponent<NitroSystem>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, -transform.up, distToGround, LayerMask.GetMask("Ground"));
        /*
        if (Mathf.Abs(currentAngularVelocity) > maxAngularVelocity)
        {
            // ���������� ����������� ��������������� torque
            float counterTorque = -Mathf.Sign(currentAngularVelocity) * (Mathf.Abs(currentAngularVelocity) - maxAngularVelocity) * counterTorqueFactor;
            carRigidbody.AddTorque(Vector3.up * counterTorque, ForceMode.Acceleration);
        }
        */

        Vector3 sideVelocity = (transform.InverseTransformDirection(carRigidbody.GetPointVelocity(transform.position)).x) * transform.right;
        Vector3 forwardVelocity = (transform.InverseTransformDirection(carRigidbody.GetPointVelocity(transform.position)).z) * transform.forward;
        angle = Vector3.Angle(carRigidbody.GetPointVelocity(transform.position), transform.right);
        angleDifference = Mathf.Abs(angle - 90) / 90;
        float slipCoef = slipCurve.Evaluate(angleDifference);
        if (nitroSystem != null)
        {
            if (sideVelocity.magnitude > forwardVelocity.magnitude)
            {
                AddNitro(nitroGainRate * angleDifference);
            }
        }



        CarInput();
        Accel();
        Turn();
    }

    private void Accel()
    {
        if (isGrounded)
        {
            speedInDirection = carRigidbody.linearVelocity.magnitude;
            forwardSpeed = Vector3.Dot(carRigidbody.linearVelocity, transform.forward);

            carRigidbody.AddForce(-carRigidbody.linearVelocity.normalized * (counterForceSpeedInDirection * angleDifference), ForceMode.Acceleration);

            float speedRatio = Mathf.Clamp01(forwardSpeed / maxSpeed);
            float accelMultiplier = accelerationCurve.Evaluate(speedRatio);

            carRigidbody.AddForceAtPosition(forcePosition.forward * accelInput * accel * accelMultiplier, forcePosition.position, ForceMode.Acceleration);
        }

    }

    private void Turn()
    {
        if (isGrounded)
        {
            //angle = Vector3.Angle(transform.forward, carRigidbody.velocity) / 180;
            angularVelosity = carRigidbody.angularVelocity.y;
            if (turnInput != 0)
                force = Mathf.Clamp((turnInput * torqueForce) - angularVelosity, -torqueForce, torqueForce);
            else
                force = 0;

            carRigidbody.AddTorque(Vector3.up * force, ForceMode.Acceleration);

            // ���������, �� ��������� �� ������ ����� ��������
            float currentAngularVelocity = carRigidbody.angularVelocity.y;

            float counterTorque = -Mathf.Sign(currentAngularVelocity) * (angleDifference * counterTorqueFactor);
            carRigidbody.AddTorque(Vector3.up * counterTorque, ForceMode.Acceleration);
        }

    }

    private void CarInput()
    {
        if (selfInput)
        {
            accelInput = Input.GetAxis("Vertical"); 
             turnInput = Input.GetAxis("Horizontal");
        }
    }

    public void AddNitro(float nitroGain)
    {
        nitroSystem.AddNitro(nitroGain * Time.deltaTime);
    }
}
