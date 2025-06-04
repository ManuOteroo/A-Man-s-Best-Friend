using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void LoadForest()
    {
        SceneManager.LoadScene("Forest");
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit!"); 
        Application.Quit(); 
    }

    public void LoadIntro()
    {
        SceneManager.LoadScene("IntroCutscene");
    }
}
