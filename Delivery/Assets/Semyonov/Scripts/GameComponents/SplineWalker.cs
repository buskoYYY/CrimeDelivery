using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace ArcadeBridge
{
    public class SplineWalker : MonoBehaviour
    {
        private const string CarTag = "Car";
        public SplineContainer splineContainer;
        public float speed = .01f;
        public float progress = 0f;
        [SerializeField] private float3 _offset = new Vector3(0f, .8f ,0f);
        [SerializeField] private float rotationSmoothness = 5f;

        private Rigidbody _rigidbody;
        private void Start()
        {
            progress = GetProgressFromWorldPosition(transform.position);
            _rigidbody = GetComponent<Rigidbody>();
        }
        private void FixedUpdate()
        {
            progress += speed * Time.deltaTime;
            if (progress > 1f)
            {
                if(!splineContainer.Splines[0].Closed)
                {
                    return;
                }
                progress = 0f; // Или другой способ обработки конца сплайна
            }
            Vector3 position = splineContainer.EvaluatePosition(progress) + _offset;

            //transform.position = position;

            Vector3 tangent = splineContainer.EvaluateTangent(progress);

            Quaternion lefpRotation = transform.rotation;

            if (tangent != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(tangent);
                lefpRotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSmoothness * Time.deltaTime
                );
                //transform.forward = tangent.normalized;
            }
            _rigidbody.Move(position, lefpRotation);
        }

        private float GetProgressFromWorldPosition(Vector3 worldPosition)
        {
            Spline spline = splineContainer.Spline;
            float closestT = 0f;
            float minDistance = float.MaxValue;

            // Sample points along the spline to find the closest segment
            const int samples = 100; // Increase for better accuracy
            for (int i = 0; i <= samples; i++)
            {
                float t = i / (float)samples;
                Vector3 pointOnSpline = splineContainer.EvaluatePosition(t);
                float distance = Vector3.Distance(worldPosition, pointOnSpline);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestT = t;
                }
            }

            return closestT;
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.collider.tag.Equals(CarTag))
            {
                enabled = false;
            }
        }
    }
}
