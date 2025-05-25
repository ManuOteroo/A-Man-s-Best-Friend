using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    
    public AudioSource musicSource;

    
    public AudioClip backgroundMusic;

    void Start()
    {
        
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        
        if (backgroundMusic != null)
        {
           
            musicSource.clip = backgroundMusic;

            
            musicSource.loop = true;

            
            musicSource.Play();
        }
    }
}
