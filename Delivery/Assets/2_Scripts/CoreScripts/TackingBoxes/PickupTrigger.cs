using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PickupTrigger : MonoBehaviour
{

    [Header("Объекты для погрузки")]
    public List<GameObject> objectsToLoad;
    public Transform unLoadPosition;

    public bool load;
    private bool unLoaded;

    private void OnTriggerEnter(Collider other)
    {
        TruckLoader loader = other.GetComponentInParent<TruckLoader>();
        if (loader != null)
        {
            if (load)
                loader.LoadObjects(objectsToLoad);
            else
            {
                if (!unLoaded)
                {
                    unLoaded = true;
                    loader.UnloadObjects(unLoadPosition);
                }
                
            }
        }
    }
}