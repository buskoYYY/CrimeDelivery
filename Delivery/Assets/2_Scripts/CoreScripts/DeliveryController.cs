using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeliveryController : MonoBehaviour
{
    public List<DeliveryTarget> deliveryTargets = new List<DeliveryTarget>();
    
    public int summReward = 0;
    public int activeTarget { get; private set; } = -1;

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
                /*
                if (i == 0)
                {
                    deliveryTargets[i].deliveryReward = 50;
                }
                else if (i == 1)
                {
                    deliveryTargets[i].deliveryReward = 70;
                }
                else if (i == 2)
                {
                    deliveryTargets[i].deliveryReward = 120;
                }
                else if (i == 3)
                {
                    deliveryTargets[i].deliveryReward = 150;
                }
                else if (i == 4)
                {
                    deliveryTargets[i].deliveryReward = 200;
                }
                else
                {
                    deliveryTargets[i].deliveryReward = 250;
                }
                */

                deliveryTargets[i].deliveryReward = i switch
                {
                    0 => 50,
                    1 => 70,
                    2 => 120,
                    3 => 150,
                    4 => 200,
                    _ => 250,
                };

                if (i == deliveryCount - 1)
                    deliveryTargets[i].unloadCount = itemsToEachTarget + remainder;
                else
                    deliveryTargets[i].unloadCount = itemsToEachTarget;
            }
        }
    }

    public int deliveryCount = 6;

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
            summReward += reward;

            OnDeliveredEvent?.Invoke(reward);

            for (int i = 0; i < deliveryCount; i++)
            {
                if (i == deliviriedIndex + 1)
                {
                    deliveryTargets[i].deliveryTargetVisual.SetActive(true);

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
