using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using Zer0;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject darkBackground;
    [SerializeField] private GameObject mainPause;
    [SerializeField] private GameObject optionsPause;
    
    [SerializeField] private AudioMixer testMixer;

    [SerializeField] private Slider rotationSlider;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    private CharacterAnimatorController _player;
    private bool _paused;
    private bool _optionsBool;

    private void Awake()
    {
        _player = FindObjectOfType<CharacterAnimatorController>();
    }

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

    private void Update()
    {
        if (PlayerInput.PauseMenu())
            TogglePause();
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void SpeedSlider()
    {
        _player.SetRotationSettings(rotationSlider.value);
        PlayerPrefs.SetFloat("RotationSpeed", rotationSlider.value);
    }
    
    public void TogglePause()
    {
        _paused = !_paused;
        
        if (_paused)
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
        _optionsBool = !_optionsBool;

        if (!_optionsBool)
        {
            mainPause.SetActive(true);
            optionsPause.SetActive(false);
        }
        else
        {
            mainPause.SetActive(false);
            optionsPause.SetActive(true);
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
}
