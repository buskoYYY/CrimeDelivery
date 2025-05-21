using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : CarComponent
{
    public bool isAI = false;

    public float targetOffset = 5;
    [SerializeField] private float deltaForTurn; //насколько дельта больше оффсета
    private Transform thisObjectTransform;
    public Transform TargetObjectTransform { get; private set; }
    private float inTurn;
    private float throttle;
    [SerializeField] private CarComponentsController vehicle;
    [SerializeField] private LayerMask layerMask;
    public bool testPC;

    private void Start()
    {
        vehicle = gameObject.GetComponent<CarComponentsController>();
        thisObjectTransform = transform;

        if (TargetObjectTransform == null)
            TargetObjectTransform = transform;
    }

    private void FixedUpdate()
    {
        if (!vehicle.carDamageHandler.carAlive)
        {
            inTurn = 0;
            throttle = 0;
        }
        else
        {
            if (isAI)
            {
                InputAI();
                ActivateBoostDriver();
            }
            else if (testPC)
                InputPlayer();
        }


        vehicle.ChangeTurnCarComponent(inTurn);
        vehicle.ChangeThrottleCarComponent(throttle);
    }

    public void ActivateBoostDriver()
    {
        vehicle.ActivateBoostCarComponent();
    }

    public void ChangeTarget(Transform newTarget)
    {
        TargetObjectTransform = newTarget;
    }
    public float boost;

    public void Turn(int turnDir)
    {
        inTurn = turnDir;
    }

    public void Throttle(int throttleDir)
    {
        throttle = throttleDir;
    }

    private void InputPlayer()
    {

        inTurn = Input.GetAxisRaw("Horizontal");
        throttle = Input.GetAxisRaw("Vertical");
        /*
        inTurn = ControlFreak2.CF2Input.GetAxis("Horizontal");
        if (testPC)
            throttle = ControlFreak2.CF2Input.GetAxis("Vertical");
        else
            throttle = 1;
        boost = ControlFreak2.CF2Input.GetAxis("Boost");
        if (boost > 0)
        {
            ActivateBoostDriver();
        }
        */
    }

    private void InputAI()
    {
        // Turn by facing player
        // Get the angle between the points (absolute goal) = right (target) - left
        float angle = AngleOffset(Angle2Points(thisObjectTransform.position, TargetObjectTransform.position), 0f);

        Vector3 rot = transform.eulerAngles;
        float delta = Mathf.DeltaAngle(rot.y, angle);


        //inTurn = delta > 0f ? 1f : -1f;

        if (delta > targetOffset)
        {
            deltaForTurn = delta - targetOffset;

            if (Mathf.Abs(deltaForTurn) <= 50)
                inTurn = 0.5f;
            else if (Mathf.Abs(deltaForTurn) > 50 && Mathf.Abs(deltaForTurn) < 70)
                inTurn = 0.75f;
            else if (Mathf.Abs(deltaForTurn) > 70)
                inTurn = 1f;

        }
        else if (delta < -targetOffset)
        {
            deltaForTurn = delta + targetOffset;

            if (Mathf.Abs(deltaForTurn) <= 50)
                inTurn = -0.5f;
            else if (Mathf.Abs(deltaForTurn) > 50 && Mathf.Abs(deltaForTurn) < 70)
                inTurn = -0.75f;
            else if (Mathf.Abs(deltaForTurn) > 70)
                inTurn = -1f;
            inTurn = -1f;
        }
        else inTurn = 0f;
    }

    float AngleOffset(float raw, float offset)
    {
        raw = (raw + offset) % 360;             // Mod by 360, to not exceed 360
        if (raw > 180.0f) raw -= 360.0f;
        if (raw < -180.0f) raw += 360.0f;
        return raw;
    }

    float Angle2Points(Vector3 a, Vector3 b)
    {
        //return Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;
        return Mathf.Atan2(b.x - a.x, b.z - a.z) * Mathf.Rad2Deg;
    }
}
