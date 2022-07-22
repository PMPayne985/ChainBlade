using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public bool optionsBool;
    public GameObject mainMain;
    public GameObject optionsMain;
    public Slider rotationSpeed;
    public Toggle rotateDirect;
    public Slider music;
    public Slider sfx;
    public Slider master;

    public void GameRun()
    {
        SceneManager.LoadScene("Sample Scene");
    }
    public void Options()
    {
        optionsBool = !optionsBool;
        
        if (!optionsBool)
        {
            mainMain.SetActive(true);
            optionsMain.SetActive(false);
        }
        else
        {
            mainMain.SetActive(false);
            optionsMain.SetActive(true);

            Debug.Log($"Rotation Speed: {rotationSpeed.value}");
            Debug.Log($"Music Sound: {music.value}");
            Debug.Log($"SFX Sound: {sfx.value}");
            Debug.Log($"Master Sound: {master.value}");
        }

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
