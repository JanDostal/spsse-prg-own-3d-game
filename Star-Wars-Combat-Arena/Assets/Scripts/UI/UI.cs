using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

public class UI : MonoBehaviour
{
    private AudioSource music;

    private GameObject player;

    private GameObject enemy;

    private GameObject menu;

    private GameLevelData currentGameLevelData;


    public GameObject EnemyHealthBar { get; private set; }

    public GameObject EnemyStaminaBar { get; private set; }

    public GameObject PlayerHealthBar { get; private set; }

    public GameObject PlayerStaminaBar { get; private set; }


    private TMP_Text gameStatusInfoBar;

    private TMP_Text currentGameLevelInfoBar;

    private TMP_Text musicButton;


    private bool wasGameIntroMessageShown = false;

    private bool wasMusicInitialized = false;

    private float gameIntroMessageVisibilityDuration = 6f;

    private float timeWhenGameIntroMessageDisappears;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        currentGameLevelData = GameLevelsManager.Instance.GetCurrentGameLevelData();
        player = GameObject.FindWithTag("Player");
        enemy = GameObject.FindWithTag("Enemy");

        menu = transform.Find("Menu").gameObject;
        menu.SetActive(false);

        PlayerHealthBar = transform.Find("PlayerHealthBar").gameObject;
        PlayerStaminaBar = transform.Find("PlayerStaminaBar").gameObject;
        EnemyStaminaBar = transform.Find("EnemyStaminaBar").gameObject;
        EnemyHealthBar = transform.Find("EnemyHealthBar").gameObject;

        InitializeCurrentGameLevelInfoBars(currentGameLevelData, menu);

        InitializeCurrentGameLevelMusic(currentGameLevelData, menu);
    }

    private void Update()
    {
        if (wasGameIntroMessageShown == false && Time.time >= timeWhenGameIntroMessageDisappears)
        {
            gameStatusInfoBar.text = "";
            wasGameIntroMessageShown = true;
        }

        if (player.GetComponent<Player>().GetIsDeadStatus() == true) 
        {
            gameStatusInfoBar.text = "You lost";
        }

        if (enemy.GetComponent<Enemy>().GetIsDeadStatus() == true && currentGameLevelData.LevelNumber == GameLevelsManager.Instance.GetLastLevelNumber()) 
        {
            gameStatusInfoBar.text = "Game over";
        }
        else if (enemy.GetComponent<Enemy>().GetIsDeadStatus() == true && currentGameLevelData.LevelNumber != GameLevelsManager.Instance.GetLastLevelNumber()) 
        {
            gameStatusInfoBar.text = "You won";
        }
    }

    public void OnPause(InputAction.CallbackContext context) 
    {
        if (context.performed == true) 
        {
            if (GameLevelsManager.Instance.IsLevelPaused == true)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void QuitGame() 
    {
        GameLevelsManager.Instance.EndGame();
    }

    public void RestartGame() 
    {
        GameLevelsManager.Instance.GoToFirstLevel();
    }

    public void ChangeMusicVolume() 
    {
        if (wasMusicInitialized == true) 
        {
            music.mute = music.mute == true ? false : true;

            musicButton.text = GetMusicButtonText();
        }
    }

    private void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;

        GameLevelsManager.Instance.ResumeLevel();

        player.GetComponent<PlayerInput>().actions.Enable();

        menu.SetActive(false);
        gameStatusInfoBar.enabled = true;

        PlayerStaminaBar.SetActive(true);
        PlayerHealthBar.SetActive(true);
        EnemyHealthBar.SetActive(true);
        EnemyStaminaBar.SetActive(true);

        enemy.GetComponent<Enemy>().Lightsaber.UnmuteLightsaber();
        player.GetComponent<Player>().Lightsaber.UnmuteLightsaber();
    }

    private void Pause()
    {
        Cursor.lockState = CursorLockMode.None;

        GameLevelsManager.Instance.PauseLevel();

        player.GetComponent<PlayerInput>().actions.Disable();

        menu.SetActive(true);
        gameStatusInfoBar.enabled = false;

        PlayerStaminaBar.SetActive(false);
        PlayerHealthBar.SetActive(false);
        EnemyHealthBar.SetActive(false);
        EnemyStaminaBar.SetActive(false);

        enemy.GetComponent<Enemy>().Lightsaber.MuteLightsaber();
        player.GetComponent<Player>().Lightsaber.MuteLightsaber();
    }

    private void InitializeCurrentGameLevelInfoBars(GameLevelData currentGameLevelData, GameObject menu) 
    {
        currentGameLevelInfoBar = menu.transform.Find("CurrentGameLevelInfoBar").gameObject.GetComponent<TMP_Text>();
        gameStatusInfoBar = transform.Find("GameStatusInfoBar").gameObject.GetComponent<TMP_Text>();

        gameStatusInfoBar.text = $"Level {currentGameLevelData.LevelNumber}";
        currentGameLevelInfoBar.text = $"Level {currentGameLevelData.LevelNumber}";

        timeWhenGameIntroMessageDisappears = Time.time + gameIntroMessageVisibilityDuration;
    }

    private async void InitializeCurrentGameLevelMusic(GameLevelData currentGameLevelData, GameObject menu)
    {
        music = GetComponent<AudioSource>();
        music.clip = await LoadMusic(currentGameLevelData);
        music.Play();

        musicButton = menu.transform.Find("MusicButton").gameObject.transform.Find("Text").gameObject.GetComponent<TMP_Text>();
        musicButton.text = GetMusicButtonText();

        wasMusicInitialized = true;
    }

    private string GetMusicButtonText() 
    {
        return music.mute == false ? "Mute music" : "Unmute music";
    }

    private async Task<AudioClip> LoadMusic(GameLevelData gameLevelData) 
    {
        var path = Path.Combine(Application.dataPath, "PostProcessingAssets", "Sound", "Music", $"{gameLevelData.LevelMusicName}.wav");

        AudioClip music = null;

        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            uwr.SendWebRequest();

            try
            {
                while (uwr.isDone == false) await Task.Delay(5);

                if (uwr.result == UnityWebRequest.Result.ConnectionError ||
                    uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log($"{uwr.error}");
                }
                else
                {
                    music = DownloadHandlerAudioClip.GetContent(uwr);
                }
            }
            catch (Exception err)
            {
                Debug.Log($"{err.Message}, {err.StackTrace}");
            }
        }

        return music;
    }
}
