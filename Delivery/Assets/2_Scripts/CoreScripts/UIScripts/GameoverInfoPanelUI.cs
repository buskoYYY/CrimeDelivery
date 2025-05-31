using UnityEngine;
using TMPro;
public class GameoverInfoPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text rewardType;
    [SerializeField] private TMP_Text rewardCount;
    [SerializeField] private TMP_Text creditsEarned;

    public void Setup(string rewardType, string rewardCount, int creditsEarned)
    {
        this.rewardType.text = rewardType;
        this.rewardCount.text = rewardCount;
        this.creditsEarned.text = creditsEarned.ToString();
    }
}
