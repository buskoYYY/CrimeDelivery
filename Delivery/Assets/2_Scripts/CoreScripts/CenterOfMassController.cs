using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CenterOfMassController : MonoBehaviour
{
    [SerializeField] private Transform carCenterOfMassOnWheels;
    [SerializeField] private Transform carCenterOfMassInAir;
    private Rigidbody carRigidbody;
    private CarDriftController carController;
    public float localGravity = 10;
    public float carStabilityInTurn = 1.5f;

    public float localRotation;


    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carController = GetComponent<CarDriftController>();
        carRigidbody.centerOfMass = Vector3.Scale(carCenterOfMassOnWheels.localPosition,transform.localScale);
    }

    private bool onWheels = false;
    private bool triggerToRotate = false;
    private bool inTrigger = false;

    private float rotationForce = 10;
   
    private void FixedUpdate()
    {
        localRotation = carRigidbody.transform.localEulerAngles.x;


        if (carController.isGrounded == true)
        {
            carRigidbody.AddForce(Vector3.down * localGravity * 1.5f, ForceMode.Acceleration);
            carRigidbody.centerOfMass = Vector3.Scale(new Vector3(carCenterOfMassOnWheels.localPosition.x - (carController.inTurn / carStabilityInTurn), carCenterOfMassOnWheels.localPosition.y, carCenterOfMassOnWheels.localPosition.z), transform.localScale);
        }
        else
        {
            carRigidbody.AddForce(Vector3.down * localGravity, ForceMode.Acceleration);
            carRigidbody.centerOfMass = Vector3.Scale(carCenterOfMassOnWheels.localPosition, transform.localScale);
        }



        /*
        if (inTrigger == false)
        {
            carRigidbody.centerOfMass = Vector3.Scale(carCenterOfMassOnWheels.localPosition, transform.localScale);
            onWheels = true;
            inAir = false;
        }
        

        if (carController.isGrounded == true)
            carRigidbody.centerOfMass = Vector3.Scale(carCenterOfMassOnWheels.localPosition, transform.localScale);
        */
    }





    private Coroutine centerMassHelper;
    public float rotationForceLocalShower;
    public float rotationMultiplyer = 100;
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 6 && carController.isGrounded == false)
        {
            carRigidbody.centerOfMass = Vector3.Scale(carCenterOfMassInAir.localPosition, transform.localScale);
            if (localRotation <= 25 || localRotation >= 320)
            {
                float rotationForceLocal = 10;
                rotationForceLocalShower = rotationForceLocal;
                if (carController.inTurn != 0)
                {
                    if (rotationForceLocal < rotationForce)
                        rotationForceLocal += Time.deltaTime * rotationMultiplyer;
                }
                carRigidbody.AddRelativeTorque(new Vector3(carRigidbody.transform.localRotation.x, carRigidbody.transform.localRotation.y, carRigidbody.transform.localRotation.z + rotationForceLocal * (carController.inTurn * -1)), ForceMode.Acceleration);
            }

        } else
            carRigidbody.centerOfMass = Vector3.Scale(carCenterOfMassOnWheels.localPosition, transform.localScale);

    }

    /*

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6)
            inTrigger = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            inTrigger = false;
            carRigidbody.centerOfMass = Vector3.Scale(carCenterOfMassOnWheels.localPosition, transform.localScale);
        }
    }
    */

}
