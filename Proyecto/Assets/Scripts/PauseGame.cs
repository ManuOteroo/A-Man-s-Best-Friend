using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseMenu;

    
    private bool isPaused = false;

    void Update()
    {

       
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                
                ResumeGame();
            }
            else
            {
                
                Pause();
            }
        }
    }

    void Pause()
    {
        
        Time.timeScale = 0f;

        // Displays the pause menu UI
        pauseMenu.SetActive(true);

        // Sets internal state to paused
        isPaused = true;
       
    }

    public void ResumeGame()
    {
        // Resumes the game by restoring time
        Time.timeScale = 1f;

        // Hides the pause menu UI
        pauseMenu.SetActive(false);

        // Sets state to unpaused
        isPaused = false;
       
    }
}
