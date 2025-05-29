using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeliveryController : MonoBehaviour
{
    public List<DeliveryTarget> deliveryTargets = new List<DeliveryTarget>();
    
    public int summReward = 0;

    public delegate void OnDelivered(int reward);
    public event OnDelivered OnDeliveredEvent;

    public delegate void OnDeliveredAll();
    public event OnDeliveredAll OnDeliveredAllEvent;

    private void Start()
    {
        for (int i = 0; i < deliveryTargets.Count; i++) 
        {
            deliveryTargets[i].deliveryTargetIndex = i;
            deliveryTargets[i].deliveryController = this;
            if (i == 0)
                deliveryTargets[i].deliveryTargetVisual.SetActive(true);
            else
                deliveryTargets[i].deliveryTargetVisual.SetActive(false);
        }
    }

    [SerializeField] private int activeTarget = -1;

    private float deliverDelay = 1;
    private float timeBetweenDeliver;
    private void FixedUpdate()
    {
        if (timeBetweenDeliver < deliverDelay)
            timeBetweenDeliver += Time.deltaTime;
    }

    public void Delivered(int deliviriedIndex, int reward)
    {
        if (timeBetweenDeliver >= deliverDelay)
        {
            if (deliviriedIndex == activeTarget + 1)
            {
                activeTarget = deliviriedIndex;

                OnDeliveredEvent?.Invoke(reward);

                for (int i = 0; i < deliveryTargets.Count; i++)
                {
                    if (i == deliviriedIndex + 1)
                    {
                        deliveryTargets[i].deliveryTargetVisual.SetActive(true);
                        summReward += reward;
                    }
                    else
                    {
                        deliveryTargets[i].deliveryTargetVisual.SetActive(false);
                    }
                }

                if (deliviriedIndex == deliveryTargets.Count - 1)
                {
                    SceneManager.LoadScene(0);
                }
            }
        }

    }
}
