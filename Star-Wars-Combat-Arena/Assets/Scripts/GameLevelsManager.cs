using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameLevelsManager : MonoBehaviour
{
    public static GameLevelsManager Instance { get; private set; }

    public bool IsLevelPaused { get; private set; }

    private IEnumerable<GameLevelData> GameLevelsData { get; set; }

    private int CurrentLevelNumber { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);

            IsLevelPaused = false;

            CurrentLevelNumber = 1;

            GameLevelsData = new List<GameLevelData>()
            {
                new GameLevelData.Builder()
                    .SetPlayerConfigurationData(
                        new PlayerData.Builder()
                            .SetLightsaberDamage(25)
                            .SetStaminaRegenSpeed(80)
                            .SetStaminaPenaltyForAttack(40)
                            .SetStaminaMaxLimit(400)
                            .SetHealthMaxLimit(500)
                            .SetLightsaberColor(LightsaberColor.BLUE)
                            .Build()
                    )
                    .SetEnemyConfigurationData(
                        new EnemyData.Builder()
                            .SetLightsaberDamage(25)
                            .SetStaminaRegenSpeed(100)
                            .SetStaminaPenaltyForAttack(40)
                            .SetStaminaMaxLimit(400)
                            .SetHealthMaxLimit(500)
                            .SetLightsaberColor(LightsaberColor.RED)
                            .Build()
                    )
                    .SetLevelMusicName("gameLevelOneMusic")
                    .SetLevelNumber(1)
                    .Build(),

                new GameLevelData.Builder()
                    .SetPlayerConfigurationData(
                        new PlayerData.Builder()
                            .SetLightsaberDamage(25)
                            .SetStaminaRegenSpeed(80)
                            .SetStaminaPenaltyForAttack(40)
                            .SetStaminaMaxLimit(400)
                            .SetHealthMaxLimit(500)
                            .SetLightsaberColor(LightsaberColor.GREEN)
                            .Build()
                    )
                    .SetEnemyConfigurationData(
                        new EnemyData.Builder()
                            .SetLightsaberDamage(40)
                            .SetStaminaRegenSpeed(120)
                            .SetStaminaPenaltyForAttack(65)
                            .SetStaminaMaxLimit(500)
                            .SetHealthMaxLimit(550)
                            .SetLightsaberColor(LightsaberColor.RED)
                            .Build()
                    )
                    .SetLevelMusicName("gameLevelTwoMusic")
                    .SetLevelNumber(2)
                    .Build(),

                new GameLevelData.Builder()
                    .SetPlayerConfigurationData(
                        new PlayerData.Builder()
                            .SetLightsaberDamage(25)
                            .SetStaminaRegenSpeed(80)
                            .SetStaminaPenaltyForAttack(40)
                            .SetStaminaMaxLimit(400)
                            .SetHealthMaxLimit(500)
                            .SetLightsaberColor(LightsaberColor.PURPLE)
                            .Build()
                    )
                    .SetEnemyConfigurationData(
                        new EnemyData.Builder()
                            .SetLightsaberDamage(50)
                            .SetStaminaRegenSpeed(133)
                            .SetStaminaPenaltyForAttack(75)
                            .SetStaminaMaxLimit(550)
                            .SetHealthMaxLimit(550)
                            .SetLightsaberColor(LightsaberColor.RED)
                            .Build()
                    )
                    .SetLevelMusicName("gameLevelThreeMusic")
                    .SetLevelNumber(3)
                    .Build()
            };

            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void ReloadGame()
    {
        ResumeLevel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public GameLevelData GetCurrentGameLevelData() 
    {
        return Instance.GameLevelsData.Where(x => x.LevelNumber == Instance.CurrentLevelNumber).SingleOrDefault();
    }

    public int GetLastLevelNumber() 
    {
        return Instance.GameLevelsData.Count();
    }

    public void GoToNextLevel()
    {
        Instance.CurrentLevelNumber++;

        if (Instance.GameLevelsData.Where(x => x.LevelNumber == Instance.CurrentLevelNumber).SingleOrDefault() == null) 
        {
            EndGame();
        }
        else 
        {
            ReloadGame();
        }
    }

    public void GoToFirstLevel()
    {
        Instance.CurrentLevelNumber = 1;
        ReloadGame();
    }

    public void EndGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        //Application.Quit();
    }

    public void PauseLevel() 
    {
        Time.timeScale = 0f;
        Instance.IsLevelPaused = true;
    }

    public void ResumeLevel() 
    {
        Time.timeScale = 1f;
        Instance.IsLevelPaused = false;
    }
}
