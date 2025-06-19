using System;
using TMPro;
using UnityEngine;

namespace ArcadeBridge
{
    public class RewardMoneyView : MonoBehaviour
    {
        [SerializeField] private bool _xReward;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private GameoverController _gameoverController;
        
        private void OnEnable()
        {
            if (_xReward)
            {
                _text.text = _gameoverController.gameoverData.xSummReward.ToString();
            }
            else
            {
                _text.text = _gameoverController.gameoverData.rewardSumm.ToString();
            }
        }
    }
}
