using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private float m_Distance = 10.0f;
    [SerializeField] private float m_Height = 5.0f;
    [SerializeField] private float m_HeightDamping = 2.0f;
    [SerializeField] private float m_RotationDamping = 3.0f;
    [SerializeField] private float m_MoveSpeed = 1.0f;
    [SerializeField] private float m_NormalFov = 60.0f;
    [SerializeField] private float m_FastFov = 90.0f;
    [SerializeField] private float m_FovDamping = 0.25f;

    private Transform m_Transform;
    private Camera m_Camera;

    public Transform FollowTarget;
    public float SpeedRatio { get; set; }

    private void Awake()
    {
        m_Transform = transform;
        //m_Camera = GetComponent<Camera>();
    }

    private void Start()
    {
        StartCoroutine(LateFixedUpdate());
    }

    private Vector3 velocity = Vector3.zero;

    [SerializeField] private float radius = 10;
    
    public void LateUpdate()
    {
        /*
        if (FollowTarget == null)
        {
            return;
        }

        float wantedRotationAngle = FollowTarget.eulerAngles.y;
        float wantedHeight = FollowTarget.position.y + m_Height;
        float currentRotationAngle = m_Transform.eulerAngles.y;
        float currentHeight = m_Transform.position.y;

        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, m_RotationDamping * Time.deltaTime);

        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, m_HeightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0.0f, currentRotationAngle, 0.0f);

        Vector3 desiredPosition = FollowTarget.position;
        desiredPosition -= currentRotation * Vector3.forward * m_Distance;
        desiredPosition.y = currentHeight;
        //m_Transform.position = Vector3.RotateTowards(m_Transform.position, desiredPosition, Time.deltaTime * m_MoveSpeed,0.0f);
        m_Transform.position = Vector3.MoveTowards(m_Transform.position, desiredPosition, Time.deltaTime * m_MoveSpeed);
        //m_Transform.position = Vector3.Lerp(m_Transform.position, FollowTarget.position, Time.deltaTime * m_MoveSpeed); //Vector3.SmoothDamp(m_Transform.position, desiredPosition, ref velocity, m_MoveSpeed); //Vector3.MoveTowards(m_Transform.position, desiredPosition, Time.deltaTime * m_MoveSpeed);
        //m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, FollowTarget.rotation, Time.deltaTime * m_RotationDamping);


        m_Transform.LookAt(FollowTarget);

        /*
        const float FAST_SPEED_RATIO = 0.9f;
        float targetFov = SpeedRatio > FAST_SPEED_RATIO ? m_FastFov : m_NormalFov;
        m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, targetFov, Time.deltaTime * m_FovDamping);
        
         
         */
        
    }

    private IEnumerator LateFixedUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (FollowTarget == null) continue;

            float wantedRotationAngle = FollowTarget.eulerAngles.y;
            float wantedHeight = FollowTarget.position.y + m_Height;
            float currentRotationAngle = m_Transform.eulerAngles.y;
            float currentHeight = m_Transform.position.y;

            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, m_RotationDamping * Time.deltaTime);

            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, m_HeightDamping * Time.deltaTime);

            Quaternion currentRotation = Quaternion.Euler(0.0f, currentRotationAngle, 0.0f);

            Vector3 desiredPosition = FollowTarget.position;
            desiredPosition -= currentRotation * Vector3.forward * m_Distance;
            desiredPosition.y = currentHeight;
            //m_Transform.position = Vector3.RotateTowards(m_Transform.position, desiredPosition, Time.deltaTime * m_MoveSpeed,0.0f);
            m_Transform.position = Vector3.MoveTowards(m_Transform.position, desiredPosition, Time.deltaTime * m_MoveSpeed);
            //m_Transform.position = Vector3.Lerp(m_Transform.position, FollowTarget.position, Time.deltaTime * m_MoveSpeed); //Vector3.SmoothDamp(m_Transform.position, desiredPosition, ref velocity, m_MoveSpeed); //Vector3.MoveTowards(m_Transform.position, desiredPosition, Time.deltaTime * m_MoveSpeed);
            //m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, FollowTarget.rotation, Time.deltaTime * m_RotationDamping);


            m_Transform.LookAt(FollowTarget);

        }
    }
}
