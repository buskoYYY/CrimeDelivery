using TMPro;
using UnityEngine;

namespace ArcadeBridge
{
    public class ViewMoney : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        private void Start()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            _text.text = SaveLoadService.instance.PlayerProgress.money.ToString();
        }
    }
}
