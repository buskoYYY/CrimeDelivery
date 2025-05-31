using UnityEngine;
using UnityEngine.EventSystems;

namespace ArcadeBridge
{
    public class ConstructNextCarButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private FinishConstructionWindow _finishConstructionCarWindow;

        private void ConstructNextCarStage()
        {
            SequenceOfActivities.Instance.ClearGame();
            SequenceOfActivities.Instance.StartGame();

            Destroy(_finishConstructionCarWindow.gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ConstructNextCarStage();
        }
    }
}
