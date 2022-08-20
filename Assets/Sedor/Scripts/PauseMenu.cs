using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using Zer0;

public class PauseMenu : MonoBehaviour
{
    public bool optionsBool;
    public GameObject darkBackground;
    public GameObject mainPause;
    public GameObject optionsPause;
    public Slider rotationSpeed;
    public Slider music;
    public Slider sfx;
    public Slider master;
    public AudioMixer testMixer;
    private CharacterAnimatorController _player;
    public bool paused;

    private void Awake()
    {
        _player = FindObjectOfType<CharacterAnimatorController>();
    }

    private void Update()
    {
        if (PlayerInput.PauseMenu())
            TogglePause();
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SpeedSlider(Slider speed)
    {
        _player.SetRotationSettings(speed.value);
    }
    
    public void TogglePause()
    {
        paused = !paused;
        
        if (paused)
        {
            darkBackground.SetActive(true);
            mainPause.SetActive(true);
            optionsPause.SetActive(false);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            darkBackground.SetActive(false);
            mainPause.SetActive(false);
            optionsPause.SetActive(false);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ToggleOptions()
    {
        optionsBool = !optionsBool;

        if (!optionsBool)
        {
            Cursor.lockState = CursorLockMode.None;
            mainPause.SetActive(true);
            optionsPause.SetActive(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            mainPause.SetActive(false);
            optionsPause.SetActive(true);
        }

    }

    public void MasterVolume(Slider volume)
    {
        testMixer.SetFloat("MasterTest", master.value);
        Debug.Log($"Master Sound: {volume.value}");
    }

    public void MusicVolume(Slider volume)
    {
        testMixer.SetFloat("MusicTest", music.value);
        Debug.Log($"Music Sound: {volume.value}");
    }

    public void SFXVolume(Slider volume)
    {
        testMixer.SetFloat("SFXTest", sfx.value);
        Debug.Log($"SFX Sound: {volume.value}");
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif        
    }
}
