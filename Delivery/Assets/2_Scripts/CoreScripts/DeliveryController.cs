using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeliveryController : MonoBehaviour
{
    public List<DeliveryTarget> deliveryTargets = new List<DeliveryTarget>();
    
    public int summReward = 0;
    public int activeTarget { get; private set; }  = -1;

    public delegate void OnDelivered(int reward);
    public event OnDelivered OnDeliveredEvent;

    public delegate void OnDeliveredAll(CarComponentsController playerCar, RaceData.CompleteType completeType);
    public event OnDeliveredAll OnDeliveredAllEvent;

    private CarComponentsController playerCar;
    public void Initialize(CarComponentsController playerCar)
    {
        this.playerCar = playerCar;
    }

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

    public bool CanDeliver(int deliviriedIndex)
    {
        if (deliviriedIndex == activeTarget + 1)
            return true;
        else
            return false;
    }

    public void Delivered(int deliviriedIndex, int reward)
    {
        if (CanDeliver(deliviriedIndex))
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
                OnDeliveredAllEvent?.Invoke(playerCar, RaceData.CompleteType.FINISHED);
            }
        }
    }
}
