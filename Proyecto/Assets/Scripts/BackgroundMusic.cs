using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    // Reference to the AudioSource component 
    public AudioSource musicSource;

    // The audio clip to be used as background music
    public AudioClip backgroundMusic;

    void Start()
    {
        
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        
        if (backgroundMusic != null)
        {
            // Assign the clip to the AudioSource
            musicSource.clip = backgroundMusic;

            // Enable looping so the music continues indefinitely
            musicSource.loop = true;

            // Start playing the background music
            musicSource.Play();
        }
    }
}
