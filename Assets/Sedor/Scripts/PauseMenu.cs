using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using Zer0;

public class PauseMenu : MonoBehaviour
{
    public bool optionsBool;
    public GameObject mainPause;
    public GameObject optionsPause;
    public Slider rotationSpeed;
    public Toggle rotateDirect;
    public Slider music;
    public Slider sfx;
    public Slider master;
    public bool rotationSpeedFlip;
    public AudioMixer testMixer;
    public CharacterAnimatorController cacScript;
    public bool paused;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            TogglePause();
    }

    public void GameLeave()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SpeedSlider(Slider speed)
    {
        cacScript.SetRotationSettings(speed.value);
    }

    public void RotationFlip()
    {
        rotationSpeedFlip = !rotationSpeedFlip;
    }

    public void TogglePause()
    {
        paused = !paused;
        
        if (paused)
        {
            mainPause.SetActive(true);
            optionsPause.SetActive(false);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            mainPause.SetActive(false);
            optionsPause.SetActive(false);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void Options()
    {
        optionsBool = !optionsBool;

        if (!optionsBool)
        {
            mainPause.SetActive(true);
            optionsPause.SetActive(false);
        }
        else
        {
            mainPause.SetActive(false);
            optionsPause.SetActive(true);

            Debug.Log($"Rotation Speed: {rotationSpeed.value}");
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
