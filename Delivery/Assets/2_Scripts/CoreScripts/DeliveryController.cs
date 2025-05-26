using System.Collections.Generic;
using UnityEngine;
public class DeliveryController : MonoBehaviour
{
    public List<DeliveryTarget> deliveryTargets = new List<DeliveryTarget>();
    public int activeTarget = 0;

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

    public void Delivered(int deliviriedIndex)
    {
        for (int i = 0; i < deliveryTargets.Count; i++)
        {
            if (i == deliviriedIndex + 1)
                deliveryTargets[i].gameObject.SetActive(true);
            else
                deliveryTargets[i].gameObject.SetActive(false);
        }
    }
}
