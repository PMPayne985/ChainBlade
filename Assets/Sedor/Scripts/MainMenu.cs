using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    private bool _optionsBool;
    [SerializeField, Tooltip("The Panel that contains the Main Menu buttons.")]
    private GameObject mainMain;
    [SerializeField, Tooltip("The Panel that contains the Options Menu buttons.")]
    private GameObject optionsMain;
    [SerializeField, Tooltip("Slider that controls the player adjustable mouse rotation speed.")]
    private Slider rotationSpeed;
    [SerializeField, Tooltip("Toggle to invert mouse rotation direction.")]
    private Toggle rotateDirect;
    [SerializeField, Tooltip("Slider to control Music volume.")]
    private Slider music;
    [SerializeField, Tooltip("Slider to control Sound Effects volume.")]
    private Slider sfx;
    [SerializeField, Tooltip("Slider to control Master volume.")]
    private Slider master;
    private int _rotationSpeedFinal;
    [SerializeField]
    private AudioMixer testMixer;

    public void GameRun()
    {
        SceneManager.LoadScene(1);
    }
    
    public void Options()
    {
        _optionsBool = !_optionsBool;
        
        if (!_optionsBool)
        {
            mainMain.SetActive(true);
            optionsMain.SetActive(false);
        }
        else
        {
            mainMain.SetActive(false);
            optionsMain.SetActive(true);

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
    
    public void InverseRotate()
    {
       _rotationSpeedFinal = _rotationSpeedFinal * -1;
    }
}
