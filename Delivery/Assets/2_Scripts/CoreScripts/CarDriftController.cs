using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DriftControllerSettings
{
    public float Accel = 15.0f;
    public float TopSpeed = 30.0f;
    public float GripX = 12.0f;
    public float GripZ = 3.0f;
    public float RotateAtStart = 190;
    public float RotVel = 0.8f;

    public DriftControllerSettings(float newAccel, float newTopSpeed, float newGripX, float newGripZ, float newRotateAtStart, float newRotVel)
    {
        Accel = newAccel;
        TopSpeed = newTopSpeed;
        GripX = newGripX;
        GripZ = newGripZ;
        RotateAtStart = newRotateAtStart;
        RotVel = newRotVel;    
    }

    public DriftControllerSettings Clone()
    {
        DriftControllerSettings clone = new(Accel, TopSpeed, GripX, GripZ, RotateAtStart, RotVel);
        return clone;
    }
}

public class CarDriftController : MonoBehaviour {

    #region Parameters
    public DriftControllerSettings carSettings;
    [SerializeField] private Transform groundCheck;

    //public float Accel = 15.0f;         // In meters/second2
    //public float TopSpeed = 30.0f;      // In meters/second
    //public float Jump = 3.0f;           // In meters/second2
    //public float GripX = 12.0f;          // In meters/second2
    //public float GripZ = 3.0f;          // In meters/second2
    //public float RotateAtStart = 190;
    //public float RotVel = 0.8f;         // Ratio of forward velocity transfered on rotation

    // Center of mass, fraction of collider boundaries (= half of size)
    // 0 = center, and +/-1 = edge in the pos/neg direction.
    public Vector3 CoM = new Vector3 (0f, .5f, 0f);
                                                
    
    public Transform Target;            // Target for the AI to act upon

    // Ground & air angular drag
    // reduce stumbling time on ground but maintain on-air one
    float AngDragG = 5.0f;
    float AngDragA = 0.05f;

    // Rotational
    float MinRotSpd = 1f;           // Velocity to start rotating
    float MaxRotSpd = 4f;           // Velocity to reach max rotation
    public AnimationCurve SlipL;    // Slip hysteresis static to full (x, input = speed)
    public AnimationCurve SlipU;    // Slip hysteresis full to static (y, output = slip ratio)
    public float SlipMod = 20f;     // Basically widens the slip curve
                                    // (determine the min speed to reach max slip)

    // AI-specific parameters
    [Header("AI Behaviors")]
    public float turnTh = 20f;      // Delta threshold to goal before start turning
    #endregion

    #region Intermediate
    public Rigidbody rigidBody;
    Bounds groupCollider;
    float distToGround;

    // The actual value to be used (modification of parameters)
    private float Rotate = 190;
    float rotate;
    float accel;
    float gripX;
    float gripZ;
    float rotVel;
    float slip;     // The value used based on Slip curves

    // For determining drag direction
    float isRight = 1.0f;
    float isForward = 1.0f;

    bool isRotating = false;
    public bool isGrounded = true;
    bool isStumbling = false;

    // Control signals
    public float inThrottle = 0f;
    public float inTurn = 0f;
    public bool inBoost = false;
    public bool inSlip = false;

    Vector3 spawnP;
    Quaternion spawnR;

    [SerializeField] Vector3 vel = new Vector3(0f, 0f, 0f);
    public Vector3 pvel = new Vector3(0f, 0f, 0f);
    #endregion



    // Use this for initialization
    void Start () {
        Rotate = carSettings.RotateAtStart;
        rigidBody = GetComponent<Rigidbody>();

        // Store start location & rotation
        spawnP = transform.position;
        spawnR = transform.rotation;
        
        groupCollider = GetBounds(gameObject);     // Get the full collider boundary of group
        distToGround = groupCollider.extents.y;    // Pivot to the outermost collider

        // Move the CoM to a fraction of colliders boundaries
        rigidBody.centerOfMass = Vector3.Scale(groupCollider.extents, CoM);

        //distToGround = transform.position.y + 1f;
    }


    // Called once per frame
    void Update() {
        Debug.DrawRay(transform.position, rigidBody.linearVelocity / 2, Color.green);
    }

    // Called once multiple times per frame 
    // (according to physics setting)


    public float raycastMultiplyer = 0.1f;

    public bool isGroundedSelf = true;
    void FixedUpdate() {
        #region Situational Checks
        GearBox();
        accel = carSettings.Accel * gearbox;
        
        rotate = Rotate * gearbox;
        gripX = carSettings.GripX;
        gripZ = carSettings.GripZ;
        rotVel = carSettings.RotVel;
        rigidBody.angularDamping = AngDragG;

        // Adjustment in slope
        accel = accel * Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad);
        accel = accel > 0f ? accel : 0f;
        gripZ = gripZ * Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad);
        gripZ = gripZ > 0f ? gripZ : 0f;
        gripX = gripX * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad);
        gripX = gripX > 0f ? gripX : 0f;

        // A short raycast to check below
        if(isGroundedSelf)
            isGrounded = Physics.Raycast(transform.position, -transform.up, raycastMultiplyer, LayerMask.GetMask("Ground"));
        
        if (!isGrounded) {
            rotate = 0f;
            accel = 0f;
            gripX = 0f;
            gripZ = 0f;
            rigidBody.angularDamping = AngDragA;
        }

        // Prevent the rotational input intervenes with physics angular velocity 
        isStumbling = rigidBody.angularVelocity.magnitude > 0.1f * Rotate * Time.deltaTime;
        if (isStumbling) {
            //rotate = 0f;
        }
        /*
        // Start turning only if there's velocity
        if (pvel.magnitude < MinRotSpd) {
            rotate = 0f;
        } else {
            rotate = pvel.magnitude / MaxRotSpd * rotate;
        }
        */

        rotate = pvel.magnitude / MaxRotSpd * rotate;

        if (rotate > Rotate) rotate = Rotate;

        // Calculate grip based on sideway velocity in hysteresis curve
        if (!inSlip) {
            // Normal => slip
            slip = this.SlipL.Evaluate(Mathf.Abs(pvel.x)/SlipMod);
            if (slip == 1f) inSlip = true;
        } else {
            // Slip => Normal
            slip = this.SlipU.Evaluate(Mathf.Abs(pvel.x)/SlipMod);
            if (slip == 0f) inSlip = false;
        }


        //rotate *= (1f + 0.5f * slip);   // Overall rotation, (body + vector)
        rotate *= (1f - 0.3f * slip);   // Overall rotation, (body + vector)
        rotVel *= (1f - slip);          // The vector modifier (just vector)

        /* Should be:
         * 1. Moving fast       : local forward, world forward.
         * 2. Swerve left       : instantly rotate left, local sideways, world forward.
         * 3. Wheels turn a little : small adjustments to the drifting arc.
         * 3. Wheels turn right : everything the same, traction still gone.
         * 4. Slowing down      : instantly rotate right, local forward, world left.
         * 
         * Update, solution: Hysteresis, gradual loss but snappy return.
         */

        #endregion

        #region Logics
        // Get command from keyboard or simple AI (conditional rulesets)

        // Execute the commands
        Controller();   // pvel assigment in here
        #endregion

        #region Passives
        // Get the local-axis velocity after rotation
        vel = transform.InverseTransformDirection(rigidBody.linearVelocity);

        // Rotate the velocity vector
        // vel = pvel => Transfer all (full grip)
        if (isRotating) {
            vel = vel * (1 - rotVel) + pvel * rotVel; // Partial transfer
            //vel = vel.normalized * speed;
        }

        // Sideway grip
        isRight = vel.x > 0f ? 1f : -1f;
        vel.x -= isRight * gripX * Time.deltaTime;  // Accelerate in opposing direction
        if (vel.x * isRight < 0f) vel.x = 0f;       // Check if changed polarity

        // Straight grip
        isForward = vel.z > 0f ? 1f : -1f;
        vel.z -= isForward * gripZ * Time.deltaTime;
        if (vel.z * isForward < 0f) vel.z = 0f;

        // Top speed
        if (vel.z > carSettings.TopSpeed) vel.z = carSettings.TopSpeed;
        else if (vel.z < -carSettings.TopSpeed) vel.z = -carSettings.TopSpeed;

        rigidBody.linearVelocity = transform.TransformDirection(vel);
        #endregion

        //cameraPlayer.PlayerUpdate();
    }



    #region Controllers
    // Get input values from keyboard

    // Executing the queued inputs
    void Controller() {

        if (inThrottle > 0.5f || inThrottle < -0.5f) {
            rigidBody.linearVelocity += transform.forward * inThrottle * accel * Time.deltaTime;
            gripZ = 0f;     // Remove straight grip if wheel is rotating
        }

        isRotating = false;

        // Get the local-axis velocity before new input (+x, +y, and +z = right, up, and forward)
        pvel = transform.InverseTransformDirection(rigidBody.linearVelocity);

        // Turn statically
        if (inTurn > 0.1f || inTurn < -0.1f) {
            if (pvel.z < 0)
                Rotate = carSettings.RotateAtStart * 1.2f;
            else
                Rotate = carSettings.RotateAtStart;
            RotateGradConst(inTurn );
        }
    }
    #endregion

    private float gearbox;
    private void GearBox()
    {
        float currentGearSpeed = pvel.z / carSettings.TopSpeed;

        if (currentGearSpeed < 0.2f)
        {
            gearbox = 1;
        }
        else if (currentGearSpeed >= 0.2f && currentGearSpeed < 0.4f)
        {
            gearbox = 0.6f; //0.8
        }
        else if (currentGearSpeed >= 0.4f && currentGearSpeed < 0.6f)
        {
            gearbox = 0.4f; //0.8
        }
        else if (currentGearSpeed >= 0.6f && currentGearSpeed < 0.8f) // ������ � �����
        {
            gearbox = 0.2f; //0.3f;
        }
        else if (currentGearSpeed >= 0.8f) // ������ � ������
        {
            gearbox = 0.1f;
        }
    }


    #region Rotation Methods

    Vector3 drot = new Vector3(0f, 0f, 0f);

    void RotateInstant(float angle) {
        if (rotate > 0f) {
            Vector3 rot = transform.eulerAngles;
            rot.y = angle;
            transform.eulerAngles = rot;
            isRotating = true;
        }
    }

    void RotateGradConst(float isCW) {
        // Delta = right(taget) - left(current)
        drot.y = isCW * rotate * Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(drot.y, transform.up);
        isRotating = true;
    }

    void RotateGradAbsolute(float angle) {
        // Delta = right(taget) - left(current)
        Vector3 rot = transform.eulerAngles;
        rot.y = AngleOffset(rot.y, 0f);

        float delta = Mathf.DeltaAngle(rot.y, angle);
        float isCW = delta > 0f ? 1f : -1f;
        rot.y += isCW * rotate * Time.deltaTime;
        rot.y = AngleOffset(rot.y, 0f);

        delta = Mathf.DeltaAngle(AngleOffset(rot.y, 0f), angle);
        if (delta * isCW < 0f) rot.y = angle;       // Check if changed polarity
        else isRotating = true;

        // You can't set them directly as it'll set x & z to zero
        // if you're not using eulerAngles.x & z.
        transform.eulerAngles = rot;
        //transform.rotation = Quaternion.AngleAxis(rot.y, Vector3.up);
        //rigidBody.MoveRotation(Quaternion.Euler(rot));
    }

    void RotateGradRelative(float angle) {
        // Delta = right(taget) - left(current)
        Vector3 rot = transform.eulerAngles;
        rot.y = AngleOffset(rot.y, 0f);

        float delta = Mathf.DeltaAngle(rot.y, angle);
        float isCW = delta > 0f ? 1f : -1f;

        // Value add to transform.eulerAngles
        drot.y = isCW * rotate * Time.deltaTime;
        drot.y = AngleOffset(rot.y, 0f);

        delta = Mathf.DeltaAngle(AngleOffset(rot.y, drot.y), angle);
        if (delta * isCW < 0f) drot.y = 0;       // Check if changed polarity
        else isRotating = true;

        // Add the drot to current rotation
        transform.rotation *= Quaternion.AngleAxis(drot.y, transform.up);
        //rigidBody.rotation *= Quaternion.AngleAxis(drot.y, transform.up);
        //transform.Rotate(drot, Space.Self);
        //rigidBody.AddTorque(drot);
        //rigidBody.MoveRotation(rigidBody.rotation * Quaternion.Euler(drot));
    }
    #endregion



    #region Utilities

    float Angle2Points(Vector3 a, Vector3 b) {
        //return Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;
        return Mathf.Atan2(b.x - a.x, b.z - a.z) * Mathf.Rad2Deg;
    }

    float AngleOffset(float raw, float offset) {
        raw = (raw + offset) % 360;             // Mod by 360, to not exceed 360
        if (raw > 180.0f) raw -= 360.0f;
        if (raw < -180.0f) raw += 360.0f;
        return raw;
    }

    // Get bound of a large 
    public static Bounds GetBounds(GameObject obj) {

        // Switch every collider to renderer for more accurate result
        Bounds bounds = new Bounds();
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        if (colliders.Length > 0) {

            //Find first enabled renderer to start encapsulate from it
            foreach (Collider collider in colliders) {

                if (collider.enabled) {
                    bounds = collider.bounds;
                    break;
                }
            }

            //Encapsulate (grow bounds to include another) for all collider
            foreach (Collider collider in colliders) {
                if (collider.enabled) {
                    bounds.Encapsulate(collider.bounds);
                }
            }
        }
        return bounds;
    }
    #endregion
}
