using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeliveryController : MonoBehaviour
{
    public List<DeliveryTarget> deliveryTargets = new List<DeliveryTarget>();
    
    public int summReward = 0;
    private int activeTarget = -1;

    public delegate void OnDelivered(int reward);
    public event OnDelivered OnDeliveredEvent;

    public delegate void OnDeliveredAll(CarComponentsController playerCar, RaceData.CompleteType completeType);
    public event OnDeliveredAll OnDeliveredAllEvent;

    private CarComponentsController playerCar;

    [SerializeField] private DeliveryTarget startLoader;
    public void Initialize(CarComponentsController playerCar)
    {
        this.playerCar = playerCar;
        if (startLoader != null)
        {
            startLoader.LoadUnload(this.playerCar);
            int itemsToEachTarget = startLoader.objectsToLoad.Count / deliveryCount;
            int remainder = startLoader.objectsToLoad.Count % deliveryCount;

            for (int i = 0; i < deliveryCount; i++)
            {
                if (i == deliveryCount - 1)
                    deliveryTargets[i].unloadCount = itemsToEachTarget + remainder;
                else
                    deliveryTargets[i].unloadCount = itemsToEachTarget;
            }
        }
    }

    [SerializeField] private int deliveryCount = 6;

    private void Awake()
    {
        GardikUtilities.Shuffle(deliveryTargets);

        deliveryCount = Mathf.Min(Random.Range(3, 6), deliveryTargets.Count);
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

            for (int i = 0; i < deliveryCount; i++)
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

            if (deliviriedIndex == deliveryCount - 1)
            {
                OnDeliveredAllEvent?.Invoke(playerCar, RaceData.CompleteType.FINISHED);
            }
        }
    }
}
