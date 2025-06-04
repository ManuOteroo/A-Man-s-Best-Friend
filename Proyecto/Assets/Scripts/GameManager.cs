using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject restartButton_Death; 
    public GameObject restartButton_Pause;
    private bool isPaused = false;

    public AudioSource audioSource; 
    public AudioClip clickSound;

    public AudioClip victorySound; 
   

    void Start()
    {
       
        if (restartButton_Death != null)
        {
            restartButton_Death.SetActive(false);
        }

        if (restartButton_Pause != null)
        {
            restartButton_Pause.SetActive(false);
        }


        audioSource = GetComponent<AudioSource>();

    }


    public void ShowRestartButton_Death()
    {
        if (restartButton_Death != null)
        {
            restartButton_Death.SetActive(true);
        }
        Invoke("PauseGameAfterDeath", 2f); 
    }

    void PauseGameAfterDeath()
    {
        Time.timeScale = 0f; 
    }

    
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
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        if (restartButton_Pause != null)
        {
            restartButton_Pause.SetActive(true);
        }
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; 
        if (restartButton_Pause != null)
        {
            restartButton_Pause.SetActive(false); 
        }
        isPaused = false;
    }

   
    public void RestartGame()
    {



        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}
