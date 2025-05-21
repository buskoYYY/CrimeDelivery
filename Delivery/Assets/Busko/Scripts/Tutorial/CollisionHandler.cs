using System;
using System.Collections;
using UnityEngine;

namespace ArcadeBridge
{
    public class CollisionHandler : MonoBehaviour
    {
        public event Action LeftTutorialOpend;
        public event Action RightTutorialOpend;

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.TryGetComponent(out SteeringLeftTrigger leftSteering))
            {
                Destroy(leftSteering.gameObject);
                LeftTutorialOpend?.Invoke();         
            }

            if (collision.TryGetComponent(out SteeringRightTrigger rightSteering))
            {
                Destroy(rightSteering.gameObject);
                RightTutorialOpend?.Invoke();
            }
        }
    }
}
