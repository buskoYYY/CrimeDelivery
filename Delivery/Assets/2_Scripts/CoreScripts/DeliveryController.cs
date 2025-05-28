using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeliveryController : MonoBehaviour
{
    public List<DeliveryTarget> deliveryTargets = new List<DeliveryTarget>();
    public int activeTarget = 0;
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
                deliveryTargets[i].gameObject.SetActive(true);
            else
                deliveryTargets[i].gameObject.SetActive(false);
        }
    }

    public void Delivered(int deliviriedIndex, int reward)
    {
        OnDeliveredEvent?.Invoke(reward);

        for (int i = 0; i < deliveryTargets.Count; i++)
        {
            if (i == deliviriedIndex + 1)
            {
                deliveryTargets[i].gameObject.SetActive(true);
            }
            else
            {
                deliveryTargets[i].gameObject.SetActive(false);
                summReward += reward;
            }
        }

        if (deliviriedIndex == deliveryTargets.Count - 1)
        {
            SceneManager.LoadScene(0); 
        }
    }
}
