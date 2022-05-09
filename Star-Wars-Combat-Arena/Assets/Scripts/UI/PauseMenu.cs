using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;

    private AudioSource music;

    private GameObject player;

    private AudioSource lightsaber;

    private AudioSource enemyLightsaber;

    public GameObject PauseMenuUI;

    private GameObject HealthBar;

    private GameObject StaminaBar;

    private TMP_Text SceneHeader;

    public TMP_Text MusicButton;

    private GameObject EnemyHealthBar;

    private GameObject EnemyStaminaBar;

    private void Start() {
        music = GameObject.FindWithTag("BackgroundMusic").GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player");
        lightsaber = GameObject.FindWithTag("LightsaberSound").GetComponent<AudioSource>();
        enemyLightsaber = GameObject.FindWithTag("EnemyLightsaberSound").GetComponent<AudioSource>();
        HealthBar = GameObject.FindWithTag("HealthBar");
        StaminaBar = GameObject.FindWithTag("StaminaBar");
        SceneHeader = GameObject.FindWithTag("SceneHeader").GetComponent<TMP_Text>();
        EnemyStaminaBar = GameObject.FindWithTag("EnemyStaminaBar");
        EnemyHealthBar = GameObject.FindWithTag("EnemyHealthBar");
    }

    public void OnPause (InputAction.CallbackContext context) 
    {
        if (GameIsPaused) 
        {
            Resume();
        }
        else 
        {
            Pause();
        }
    }

    private void Resume() 
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        player.GetComponent<PlayerInput>().actions.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        PauseMenuUI.SetActive(false);
        SceneHeader.enabled = true;
        StaminaBar.SetActive(true);
        HealthBar.SetActive(true);
        EnemyHealthBar.SetActive(true);
        EnemyStaminaBar.SetActive(true);
        lightsaber.mute = false;
        enemyLightsaber.mute = false;
    }

    private void Pause() 
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        player.GetComponent<PlayerInput>().actions.Disable();
        Cursor.lockState = CursorLockMode.None;
        SceneHeader.enabled = false;
        PauseMenuUI.SetActive(true);
        StaminaBar.SetActive(false);
        HealthBar.SetActive(false);
        EnemyHealthBar.SetActive(false);
        EnemyStaminaBar.SetActive(false);
        lightsaber.mute = true;
        enemyLightsaber.mute = true;
        if (music.mute == false) 
        {
            MusicButton.text = "Mute music";
        }
        else 
        {
            MusicButton.text = "Unmute music";
        }
    }

    public void Quit() 
    {
        UnityEditor.EditorApplication.isPlaying = false;
        //Application.Quit();
    }

    public void Restart() 
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("FirstLevel");
    }

    public void ChangeMusic() 
    {
        if (music.mute == true) 
        {
            music.mute = false;
            MusicButton.text = "Mute music";

        }
        else 
        {
            music.mute = true;
            MusicButton.text = "Unmute music";
        }
    }
}
