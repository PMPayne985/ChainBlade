using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public bool Optionsbool;
    public GameObject MainMain;
    public GameObject OptionsMain;
    public Slider RotationSpeed;
    public Toggle RotateDirect;
    public Slider Music;
    public Slider SFX;
    public Slider Master;
    // Start is called before the first frame update
    void Start()
    {
        Optionsbool = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Optionsbool == false)
        {
            MainMain.SetActive(true);
            OptionsMain.SetActive(false);
        }
        else
        {
            MainMain.SetActive(false);
            OptionsMain.SetActive(true);
            Debug.Log("Rotation Speed: " + RotationSpeed.value);
            Debug.Log("Music Sound: " + Music.value);
            Debug.Log("SFX Sound: " + SFX.value);
            Debug.Log("Master Sound: " + Master.value);
        }
    }

    public void GameRun()
    {
        SceneManager.LoadScene("Sample Scene");
    }
    public void Options()
    {
        if (Optionsbool == false)
        {
            Optionsbool = true;
        }
        else
        {
            Debug.LogWarning("Options Button Clickable after deactivation");
        }

    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void UnOptions()
    {
        if (Optionsbool == true)
        {
            Optionsbool = false;
        }
        else
        {
            Debug.LogWarning("Return Button Clickable after deactivation");
        }
    }
}
