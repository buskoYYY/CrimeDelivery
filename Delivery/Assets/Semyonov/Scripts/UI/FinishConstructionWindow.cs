using DG.Tweening;
using UnityEngine;

namespace ArcadeBridge
{
    public class FinishConstructionWindow: Window
    {
        private Sequence _sequence;

        private void Start()
        {
            transform.localScale = new Vector3(.4f, .4f, .4f);

            _sequence = DOTween.Sequence().SetUpdate(true);

            _sequence.Append(transform.DOScale(1f, .5f));

            ViewCunstructedCar view = GetComponent<ViewCunstructedCar>();

            view.PreviewCar();
            view.ShowCarPreview(SaveLoadService.instance.GetLastOpenedIndexCar());
            view.UpdateCarStatsUI();
        }

    }
}
