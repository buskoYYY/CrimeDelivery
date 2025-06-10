using ArcadeBridge;
using UnityEngine;

[System.Serializable]

public class GameoverData
{
    public int rewardSumm;
    public int xSummReward;
    public RaceData raceData;
}

public class GameoverController : MonoBehaviour
{
    public GameoverData gameoverData;
    public GameoverUI gameoverUI;
    public RaceLogic raceLogic;

    public void Initialize()
    {

    }

    public void Gameover(RaceData raceData)
    {
        gameoverData.raceData = raceData;

        SetupReward(raceData);

        if (SaveLoadService.instance != null)
        {
            SaveLoadService.instance.PlayerProgress.money += gameoverData.rewardSumm;
            SaveLoadService.instance.PlayerProgress.deliveriesCount++;
            SaveLoadService.instance.DelayedSaveProgress();
        }
        else
        {
            Debug.LogError("SaveLoadService is null");
        }

        gameoverUI.GameoverUIStart(gameoverData);
    }

    public void SetupReward(RaceData raceData)
    {
        gameoverData.rewardSumm += raceData.deliveryReward;
        
        gameoverData.xSummReward = gameoverData.rewardSumm * 2;
    }
}
