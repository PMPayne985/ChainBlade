using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

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
<<<<<<< Updated upstream
    public int rotationSpeedFinal;
=======
<<<<<<< HEAD
    public int RotationSpeedFinal;
    public AudioMixer TestMixer;
=======
    public int rotationSpeedFinal;
>>>>>>> 5ce1e750c9a3e1983458bc34086ff874e860b0a2
>>>>>>> Stashed changes

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
       rotationSpeedFinal = rotationSpeedFinal * -1;
    }
}
