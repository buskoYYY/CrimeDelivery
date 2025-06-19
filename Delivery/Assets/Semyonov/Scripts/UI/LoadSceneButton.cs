using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ArcadeBridge
{
    public class LoadSceneButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string SceneName = "Garage";

        [SerializeField] private Canvas _canvas;

        [SerializeField] private GameObject _loadingWindowPrefab;

        private GameObject _loadingWindow;

        protected Sequence _sequence;

        //[SerializeField] private MenuWindow _menu;
        public void Init(Canvas canvas)
        {
            _canvas = canvas;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_canvas)
            {
                _canvas = ConstructionCanvas.Instance.GetComponent<Canvas>();

                if (!_canvas) Debug.LogError("Needs init canvas");
            }

            if (SceneManager.GetActiveScene().name.Equals(SceneName))
            {
                //if (_menu)
                  //  Destroy(_menu.gameObject);
            }
            else
            {
                _loadingWindow = Instantiate(_loadingWindowPrefab, _canvas.transform);

                _loadingWindow.transform.localScale = new Vector3(.4f, .4f, .4f) ;

                _sequence = DOTween.Sequence().SetUpdate(true);

                _sequence.Append(_loadingWindow.transform.DOScale(1f, .5f));

                _sequence.onComplete += StartLoadScene;
            }
        }

        protected void StartLoadScene()
        {
            SceneManager.LoadSceneAsync(SceneName);
        }
    }
}
