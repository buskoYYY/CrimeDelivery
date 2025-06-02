using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraController : MonoBehaviour
{
        public float cameraDistance = 4f;
        public float cameraHeight = 1.6f;
        public float cameraHorizontalSpeed = 2f;
    public float cameraHorizontalSpeedAtStart = 2f;
    public float cameraHorizontalSpeedInAir = 2f;

    public float cameraZoomFactor = 0.7f;
    public float nitroAdditionalZoom = 0.1f;

    //Graph to setup blur intensity according to car's speed
    private AnimationCurve blurIntensity;
        private float nitroAdditionalBlur;

        //Graph to setup camera shake intensity according to car's speed
         [SerializeField] private AnimationCurve cameraShakeIntensity;
        private Vector3 cameraShakeMultiplier = Vector3.one;
        private float nitroAdditionalShake = 1;

        private float cameraShakeRoughness = 1;

        private float defaultCameraFieldOfView;

        [SerializeField] private Transform cameraPointTransform;
        private Camera cameraComponent;
        [SerializeField] private Rigidbody playerRigidbodyComponent;


        private Vector3 smoothedCameraShakeOffset;
        private float smoothedAcceleration;
        private float velocitySmoothFactor = 0.25f;

    [SerializeField] private CarComponentsController playerCar;

        
    [SerializeField] private bool initializeAtStart;
    private void Start()
    {
        StartCoroutine(LateFixedUpdate());
        cameraComponent = GetComponent<Camera>();
        defaultCameraFieldOfView = cameraComponent.fieldOfView;

        cameraHorizontalSpeedAtStart = cameraHorizontalSpeed;
        if (initializeAtStart)
            Initialize(playerCar);
    }
    public void Initialize(CarComponentsController car)
    {
        playerCar = car;
        cameraPointTransform = playerCar.transform;
        playerRigidbodyComponent = playerCar.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (playerCar != null)
        {
            if (playerCar.vehicle.isGrounded)
                cameraHorizontalSpeed = cameraHorizontalSpeedAtStart;
            else
                cameraHorizontalSpeed = cameraHorizontalSpeedInAir;
        }
    }
    private IEnumerator LateFixedUpdate()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();

                if (cameraPointTransform == null) continue;

                //Calculate delta angle
                //Looks a little bit difficult, because we use LerpAngle to avoid 360, 180 and all other angle-related issues
                float horizontalOffsetAngle = Mathf.LerpAngle(transform.eulerAngles.y, cameraPointTransform.eulerAngles.y, cameraHorizontalSpeed * Time.fixedDeltaTime);

                //Convert angle to offset
                //Note: Quaternion * Vector3 = Vector3. It returns vector in direction of the Quaternion.
                //Example: transform.rotation * Vector3.forward = transform.forward
                Vector3 horizontalOffsetVector = Quaternion.Euler(0, horizontalOffsetAngle, 0) * Vector3.forward * cameraDistance;

                //Smoothed car velocity
                smoothedAcceleration = Mathf.Lerp(smoothedAcceleration, playerRigidbodyComponent.linearVelocity.magnitude, velocitySmoothFactor);
                float currentSpeed = (smoothedAcceleration / 1000f) * 3600;

                //Nitro additions
                float additionalZoom = 0;
                float additionalBlur = 0;
                float additionalShake = 0;


                //Shake offset
                smoothedCameraShakeOffset = Vector3.Lerp(smoothedCameraShakeOffset, new Vector3(
                    Random.Range(-1f, 1f) * cameraShakeMultiplier.x,
                    Random.Range(-1f, 1f) * cameraShakeMultiplier.y,
                    Random.Range(-1f, 1f) * cameraShakeMultiplier.z), cameraShakeRoughness);

                //Shake offset (local space)
                Vector3 relativeShakeOffset = transform.TransformDirection(smoothedCameraShakeOffset) * (cameraShakeIntensity.Evaluate(currentSpeed) + additionalShake) * Time.fixedDeltaTime;

                //Camera position
                Vector3 cameraPosition = cameraPointTransform.position + (Vector3.up * cameraHeight) - horizontalOffsetVector;
                transform.position = cameraPosition;

                //Camera rotation
                transform.rotation = Quaternion.LookRotation((cameraPointTransform.position - cameraPosition) , Vector3.up);

                //Field of view
                cameraComponent.fieldOfView = defaultCameraFieldOfView + smoothedAcceleration * cameraZoomFactor + additionalZoom;

            }
        }
    }
