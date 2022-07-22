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
    public int RotationSpeedFinal;

    public void GameRun()
    {
        SceneManager.LoadScene("SampleScene");
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
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif        
    }
    public void InverseRotate()
    {
       RotationSpeedFinal = RotationSpeedFinal * -1;
    }
}
