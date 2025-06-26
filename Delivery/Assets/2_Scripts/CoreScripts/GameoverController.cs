using ArcadeBridge;
using System;
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
            SaveLoadService.instance.PlayerProgress.deliveriesCount++;
            SaveLoadService.instance.DelayedSaveProgress();
        }
        else
        {
            Debug.LogError("SaveLoadService is null");
        }

        gameoverUI.GameoverUIStart(gameoverData);

        gameoverUI.OnPlayerGetSimpleReward = OnPlayerGetSimpleReward;
        gameoverUI.OnPlayerXReward = OnPlayerXReward;

        AnalyticsEvents.RaceCompleteEvent(SaveLoadService.instance.PlayerProgress.deliveriesCount, raceData.completeType.ToString(), raceData.compleatedDeliveries, raceData.maxDeliveries, raceData.raceTime);
    }

    private void OnPlayerGetSimpleReward()
    {
        SaveLoadService.instance.PlayerProgress.money += gameoverData.rewardSumm;
    }
    private void OnPlayerXReward()
    {
        SaveLoadService.instance.PlayerProgress.money += gameoverData.xSummReward;
    }

    public void SetupReward(RaceData raceData)
    {
        gameoverData.rewardSumm += raceData.deliveryReward;
        
        gameoverData.xSummReward = gameoverData.rewardSumm * 2;
    }
}
