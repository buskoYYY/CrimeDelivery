using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBySpline : MonoBehaviour
{
    [SerializeField] private bool autoInit;

    public SplineComputer aiPath;

    public SplinePositioner targetObjectPositioner;
    //public Transform targetObjectTransform;


    public SplineProjector thisObjectProjector;
    public Transform thisObjectTransform;

    public float inTurn;
    public float targetOffset = 10;
    public float deltaForTurn; //насколько дельта больше оффсета

    [SerializeField] private Driver car;

    private void Start()
    {
        if (autoInit)
        {
            Init(aiPath);
        }
    }

    public void Init(SplineComputer newAiPath)
    {
        aiPath = newAiPath;
        thisObjectTransform = transform;
        thisObjectProjector = gameObject.AddComponent<SplineProjector>();
        thisObjectProjector.projectTarget = transform;
        thisObjectProjector.spline = newAiPath;
        targetObjectPositioner = Instantiate(new GameObject()).AddComponent<SplinePositioner>();
        targetObjectPositioner.spline = newAiPath;

        StartCoroutine(SetTargetDistance());
    }

    float lastPosition;
    float currentPosition;
    float diffPosition;

    public float objectProjecorDistance = 20;
    private IEnumerator SetTargetDistance()
    {
        car = GetComponent<Driver>();
        //car.isAI = true;
        
        car.targetObjectTransform = targetObjectPositioner.transform;
        targetObjectPositioner.spline = aiPath;
        thisObjectProjector.spline = aiPath;


        while (gameObject.activeSelf)
        {


            currentPosition = aiPath.CalculateLength(0, thisObjectProjector.GetPercent()) + objectProjecorDistance;
            diffPosition = Mathf.Abs(currentPosition - lastPosition);

            if (thisObjectProjector.GetPercent() >= 0.99f)
            {
                currentPosition = 10;
                targetObjectPositioner.SetDistance(currentPosition);
                lastPosition = currentPosition;
            }
            else if (diffPosition < 100)
            {
                targetObjectPositioner.SetDistance(currentPosition);
                lastPosition = currentPosition;
            }

            //InputAI();
            yield return new WaitForSeconds(0.01f);
        }

    }
    
    private void InputAI()
    {
        /*
        // Turn by facing player
        // Get the angle between the points (absolute goal) = right (target) - left
        float angle = AngleOffset(Angle2Points(thisObjectTransform.position, targetObjectTransform.position), 0f);

        Vector3 rot = transform.eulerAngles;
        float delta = Mathf.DeltaAngle(rot.y, angle);


        //inTurn = delta > 0f ? 1f : -1f;

        if (delta > targetOffset)
        {
            deltaForTurn = delta - targetOffset;
            inTurn = 1f;
        }
        else if (delta < -targetOffset)
        {
            deltaForTurn = delta + targetOffset;
            inTurn = -1f;
        }
        else inTurn = 0f;


        //car.CarInput(inTurn, true);

        /* InThrottle Logic
         *     public float stopAngle;
    public float releaseThrottle;
                     if (delta > releaseThrottle && delta < stopAngle && pvel.z > 30)
            {
                inThrottle = 0;
            }
            else if (delta < -releaseThrottle && delta < -stopAngle && pvel.z > 30)
            {
                inThrottle = 0;
            }
            else if (delta > stopAngle && pvel.z > 20)
            {
                inThrottle = -1;
            }
            else if (delta < -stopAngle && pvel.z > 20)
            {
                inThrottle = -1;
            }
            else
            {
                inThrottle = 1;
            }
         */
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
