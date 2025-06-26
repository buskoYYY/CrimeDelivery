using UnityEngine;

namespace ArcadeBridge
{
    public class UIHidingAtStart : MonoBehaviour
    {

        [SerializeField] private GameObject[] objects; 

        private void OnEnable()
        {
            for (int i = 0; i < objects.Length; i++)
            {
                Destroy(objects[i], 4);
            }
        }
    }
}
