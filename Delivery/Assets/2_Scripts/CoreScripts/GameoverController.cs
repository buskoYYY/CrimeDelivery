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
    
        gameoverUI.GameoverUIStart(gameoverData);
    }

    public void SetupReward(RaceData raceData)
    {
        gameoverData.rewardSumm += raceData.deliveryReward;
        
        gameoverData.xSummReward = gameoverData.rewardSumm * 2;
    }
}
