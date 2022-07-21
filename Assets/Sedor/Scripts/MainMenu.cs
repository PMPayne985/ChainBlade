using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public bool Optionsbool;
    // Start is called before the first frame update
    void Start()
    {
       bool Optionsbool = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameRun()
    {
        SceneManager.LoadScene("Sample Scene");
    }
    public void Options()
    {
        if (Optionsbool==false)
        {
            bool Optionsbool = true;
        }
        
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
