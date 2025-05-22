using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WinZone : MonoBehaviour
{

    // UI text to display when the player wins
    public GameObject winMessage;

    // Audio clip to play upon victory
    public AudioClip victorySound;

    // AudioSource component for playing sounds
    private AudioSource audioSource;

    public GameObject sceneManager;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Hides the win message at the start of the game
        if (winMessage != null)
        {
            winMessage.SetActive(false); 
        }
    }


    // Triggered when another collider enters this trigger zone
    void OnTriggerEnter2D(Collider2D other)
    {

        // Only activate win logic if the player enters
        if (other.CompareTag("Player"))
        {

            // Show the win message 
            if (winMessage != null)
            {
                winMessage.SetActive(true);
            }

            // Play the victory sound once
            if (victorySound != null && audioSource != null)
            {
                audioSource.PlayOneShot(victorySound);
            }

            // Pause the game
            Time.timeScale = 0f; 


        }
    }

    void StopGame()
    {
        Time.timeScale = 0f; 
        
    }
}
