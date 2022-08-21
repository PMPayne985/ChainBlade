using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using Zer0;

public class MainMenu : MonoBehaviour
{
    private bool _optionsBool;
    [SerializeField, Tooltip("The Panel that contains the Main Menu buttons.")]
    private GameObject mainMain;
    [SerializeField, Tooltip("The Panel that contains the Options Menu buttons.")]
    private GameObject optionsMain;
    [SerializeField, Tooltip("Slider that controls the player adjustable mouse rotation speed.")]
    private Slider rotationSlider;
    [SerializeField, Tooltip("Slider to control Music volume.")]
    private Slider musicSlider;
    [SerializeField, Tooltip("Slider to control Sound Effects volume.")]
    private Slider sfxSlider;
    [SerializeField, Tooltip("Slider to control Master volume.")]
    private Slider masterSlider;
    [SerializeField]
    private AudioMixer testMixer;

    private void Start()
    {
        masterSlider.onValueChanged.AddListener(delegate {MasterVolume(); });
        musicSlider.onValueChanged.AddListener(delegate {MusicVolume(); });
        sfxSlider.onValueChanged.AddListener(delegate {SFXVolume(); });
        rotationSlider.onValueChanged.AddListener(delegate {SpeedSlider(); });

        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", .5f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", .5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", .5f);
        rotationSlider.value = PlayerPrefs.GetFloat("RotationSpeed", .5f);
    }

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
        }

    }

    private void MasterVolume()
    {
        testMixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
    }

    private void MusicVolume()
    {
        testMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    private void SFXVolume()
    {
        testMixer.SetFloat("SFX", Mathf.Log10(sfxSlider.value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }

    public void QuitGame()
    {
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif        
    }

    private void SpeedSlider()
    {
        PlayerPrefs.SetFloat("RotationSpeed", rotationSlider.value);
    }
}
