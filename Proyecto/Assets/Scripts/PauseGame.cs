using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    // Reference to the UI element that serves as the pause menu
    public GameObject pauseMenu;

    // Tracks whether the game is currently paused
    private bool isPaused = false;

    void Update()
    {

        // Checks for the escape key to toggle pause state
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                // If already paused, resume the game
                ResumeGame();
            }
            else
            {
                // If not paused, pause the game
                Pause();
            }
        }
    }

    void Pause()
    {
        // Freezes the game by stopping time progression
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
