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
    [SerializeField] private ChainUpgrade enhancementMenu;
    
    [SerializeField] private AudioMixer testMixer;

    [SerializeField] private Slider rotationSlider;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    
    public static bool Paused { get; private set; }
    private bool _optionsBool;

    private void Awake()
    {
        OpenAllMenus();
    }

    private void Start()
    {
        masterSlider.onValueChanged.AddListener(delegate {MasterVolume(); });
        musicSlider.onValueChanged.AddListener(delegate {MusicVolume(); });
        sfxSlider.onValueChanged.AddListener(delegate {SFXVolume(); });

        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", .5f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", .5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", .5f);
        rotationSlider.value = PlayerPrefs.GetFloat("RotationSpeed", .5f);
        
        CloseAllMenus();
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

    public void TogglePause()
    {
        if (ChainUpgrade.EnhancementOpen) return;
        if (DebugMenu.Instance.MenuOn) return;
        
        Paused = !Paused;
        
        if (Paused)
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
            _optionsBool = false;
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

    public void OpenAllMenus()
    {
        mainPause.SetActive(true);
        optionsPause.SetActive(true);
    }

    public void CloseAllMenus()
    {
        mainPause.SetActive(false);
        optionsPause.SetActive(false);
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
