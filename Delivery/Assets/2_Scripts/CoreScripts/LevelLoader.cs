using UnityEngine;
public class LevelLoader : MonoBehaviour
{
    public GameData gameData;
    public int currentLevel;
    [SerializeField] private bool initAtStart;


    private void Start()
    {
        if (initAtStart)
            InitializeScene(currentLevel, gameData);
    }

    public void InitializeScene(int currentLevel, GameData gameData)
    {
        this.gameData = gameData;
        this.currentLevel = currentLevel;
        Instantiate(gameData.sceneConfigs[currentLevel]);
    }
}
