using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wheel_Suspension
{
    public Transform wheel;
    public Transform hardPointTransform;
    public GameObject hardPointGameobject;
    public RaycastHit wheelHit;
    public float springForce = 30000f;
    public float springDamper = 200f;
    public float maxWheelTravel = 0.2f;

    public float maxSpringDistance;
    public float suspensionForce;
    public float offset_Prev;
    public float wheelRadius;
    public bool isGrounded;
}

public class TURBO_Suspension : MonoBehaviour
{
    [Header("Suspension")]
    [Space(10)]

    public Wheel_Suspension[] steeringWheels;
    public Wheel_Suspension[] rearWheels;
    [SerializeField] private List<Wheel_Suspension> allWheels = new List<Wheel_Suspension>();
    public bool localSetupWheel;

    public float setup_springForce = 30000f;
    public float setup_springDamper = 200f;
    public float setup_wheelRadius;
    public float setup_maxWheelTravel = 0.2f;





    //private float MaxSpringDistance;
    //private float[] suspensionForce = new float[4];

    [Header("Visuals")]
    [Space(10)]
    public Transform vehicleBody;
    public Transform wheelsMainObject;

    //public Transform[] HardPoints = new Transform[4];
    //public Transform[] Wheels;

    [Header("Other Things")]
    //private RaycastHit[] wheelHits = new RaycastHit[4];
    private Rigidbody carRigidbody;
    //private float[] offset_Prev = new float[4];

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        allWheels.AddRange(steeringWheels);
        allWheels.AddRange(rearWheels);


        for (int i = 0; i < allWheels.Count; i++)
        {
            SetupWheel(allWheels[i], i);
        }
    }

    private void SetupWheel(Wheel_Suspension wheel_suspension, int wheelNumber)
    {
        if (!localSetupWheel)
        {
            SetupWheelsConfigs(wheel_suspension);
        }

        wheel_suspension.hardPointGameobject = Instantiate(new GameObject(), wheelsMainObject.position, wheelsMainObject.rotation, wheelsMainObject);
        wheel_suspension.hardPointGameobject.name = $"HardPoint({wheelNumber})";
        wheel_suspension.hardPointTransform = wheel_suspension.hardPointGameobject.transform;
        wheel_suspension.hardPointTransform.localPosition =  new Vector3(wheel_suspension.wheel.localPosition.x, 0, wheel_suspension.wheel.localPosition.z);
        
        wheel_suspension.maxSpringDistance = Mathf.Abs(wheel_suspension.wheel.localPosition.y - wheel_suspension.hardPointTransform.localPosition.y) + 0.1f + wheel_suspension.wheelRadius;

    }

    private void OnValidate()
    {
        foreach (Wheel_Suspension wheel in allWheels)
        {
            SetupWheelsConfigs(wheel);
        }
    }

    private void SetupWheelsConfigs(Wheel_Suspension wheel_suspension)
    {
        wheel_suspension.wheelRadius = setup_wheelRadius;
        wheel_suspension.springForce = setup_springForce;
        wheel_suspension.springDamper = setup_springDamper;
        wheel_suspension.maxWheelTravel = setup_maxWheelTravel;
    }

    public bool IsGroundedForAllCar(int wheelsToBeGrounded)
    {
        int groundedWheelsCount = 0;
        for (int i = 0; i < allWheels.Count; i++)
        {
            if (allWheels[i].isGrounded)
                groundedWheelsCount++;
        }
        if (groundedWheelsCount >= wheelsToBeGrounded)
            return true;
        else
            return false;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < allWheels.Count; i++)
        {
            allWheels[i].suspensionForce = 0;
            AddSuspensionForce_2(allWheels[i], i);

            TireVisual(allWheels[i]);
        }

        float suspensionForce_hackSum = (allWheels[0].suspensionForce + allWheels[1].suspensionForce + allWheels[2].suspensionForce + allWheels[3].suspensionForce) / 4;

        allWheels[0].suspensionForce = suspensionForce_hackSum;
        allWheels[1].suspensionForce = suspensionForce_hackSum;
        allWheels[2].suspensionForce = suspensionForce_hackSum;
        allWheels[3].suspensionForce = suspensionForce_hackSum;
    }

    private void AddSuspensionForce_2(Wheel_Suspension wheel_Suspension, int wheelNum)
    {
        var direction = -transform.up;

        if (Physics.SphereCast(wheel_Suspension.hardPointTransform.position + (transform.up * wheel_Suspension.wheelRadius), wheel_Suspension.wheelRadius, direction, 
            out wheel_Suspension.wheelHit, wheel_Suspension.maxSpringDistance, 1 << 6, QueryTriggerInteraction.Ignore))
        {
            wheel_Suspension.isGrounded = true;
        }
        else
        {
            wheel_Suspension.isGrounded = false;
        }

        // suspension spring force
        if (wheel_Suspension.isGrounded)
        {
            Vector3 springDir = wheel_Suspension.wheelHit.normal;
            //springDir = transform.up;
            float offset = (wheel_Suspension.maxSpringDistance + 0.1f - wheel_Suspension.wheelHit.distance) / (wheel_Suspension.maxSpringDistance - wheel_Suspension.wheelRadius - 0.1f);
            offset = Mathf.Clamp01(offset);

            float vel = -((offset - wheel_Suspension.offset_Prev) / Time.fixedDeltaTime);

            Vector3 wheelWorldVel = carRigidbody.GetPointVelocity(wheel_Suspension.hardPointTransform.position);
            float WheelVel = Vector3.Dot(transform.up, wheelWorldVel);



            wheel_Suspension.offset_Prev = offset;
            if (offset < 0.3f)
            {
                vel = 0;
            }
            else if (vel < 0 && offset > 0.6f && WheelVel < 10)
            {
                vel *= 10;
            }

            float TotalSpringForce = offset * offset * wheel_Suspension.springForce;
            float totalDampingForce = Mathf.Clamp(-(vel * wheel_Suspension.springDamper),
                -0.25f * carRigidbody.mass * Mathf.Abs(WheelVel) / Time.fixedDeltaTime, 0.25f * carRigidbody.mass * Mathf.Abs(WheelVel) / Time.fixedDeltaTime);
            if ((wheel_Suspension.maxSpringDistance + 0.1f - wheel_Suspension.wheelHit.distance) < 0.1f)
            {
                totalDampingForce = 0;
            }
            float force = TotalSpringForce + totalDampingForce;


            wheel_Suspension.suspensionForce = force;

            Vector3 suspensionForce_vector = Vector3.Project(springDir, transform.up) * force;

            carRigidbody.AddForceAtPosition(suspensionForce_vector, wheel_Suspension.hardPointTransform.position);

            //if (offset > 0.5f && WheelVel > 5)
            //{
            //    rb.velocity -= WheelVel * springDir / 4;
            //}

        }
        else
        {
            wheel_Suspension.suspensionForce = 0;
        }

    }

    private void TireVisual(Wheel_Suspension wheel_Suspension)
    {
        if (wheel_Suspension.isGrounded)
        {
            Vector3 wheelPos = wheel_Suspension.wheel.localPosition;
            if (wheel_Suspension.offset_Prev > 0.3f)
            {
                wheelPos = wheel_Suspension.hardPointTransform.localPosition + (Vector3.up * wheel_Suspension.wheelRadius) - Vector3.up * (wheel_Suspension.wheelHit.distance);
            }
            else
            {
                wheelPos = Vector3.Lerp(new Vector3(wheel_Suspension.hardPointTransform.localPosition.x, wheel_Suspension.wheel.localPosition.y, wheel_Suspension.hardPointTransform.localPosition.z), 
                    wheel_Suspension.hardPointTransform.localPosition + (Vector3.up * wheel_Suspension.wheelRadius) - Vector3.up * (wheel_Suspension.wheelHit.distance), 0.1f);
            }

            if (wheelPos.y > wheel_Suspension.hardPointTransform.localPosition.y + wheel_Suspension.wheelRadius + wheel_Suspension.maxWheelTravel - wheel_Suspension.maxSpringDistance)
            {
                wheelPos.y = wheel_Suspension.hardPointTransform.localPosition.y + wheel_Suspension.wheelRadius + wheel_Suspension.maxWheelTravel - wheel_Suspension.maxSpringDistance;
            }


            wheel_Suspension.wheel.localPosition = wheelPos;

        }
        else
        {
            wheel_Suspension.wheel.localPosition = Vector3.Lerp(new Vector3(wheel_Suspension.hardPointTransform.localPosition.x, wheel_Suspension.wheel.localPosition.y, wheel_Suspension.hardPointTransform.localPosition.z),
                wheel_Suspension.hardPointTransform.localPosition + (Vector3.up * wheel_Suspension.wheelRadius) - Vector3.up * wheel_Suspension.maxSpringDistance, 0.05f);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < allWheels.Count; i++)
        {
            Gizmos.DrawLine(allWheels[i].hardPointTransform.position + (transform.up * allWheels[i].wheelRadius), allWheels[i].wheel.position);
            Gizmos.DrawWireSphere(allWheels[i].wheel.position, allWheels[i].wheelRadius);
            Gizmos.DrawSphere(allWheels[i].hardPointTransform.position + (transform.up * allWheels[i].wheelRadius), 0.05f);

            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.ArrowHandleCap(0, allWheels[i].wheel.position + transform.up * allWheels[i].wheelRadius, allWheels[i].wheel.rotation * Quaternion.LookRotation(Vector3.up),
                allWheels[i].maxWheelTravel, EventType.Repaint);
        }

    }
#endif
}
