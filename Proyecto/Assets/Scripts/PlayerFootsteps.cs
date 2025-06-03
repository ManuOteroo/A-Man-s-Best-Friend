using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioClip[] footstepClips;
    public float stepRate = 0.4f; 
    public float minSpeed = 0.1f; 

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private float stepTimer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rb == null || audioSource == null || footstepClips.Length == 0)
            return;

        float speed = rb.velocity.magnitude;

        if (speed > minSpeed)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepRate;
            }
        }
        else
        {
            stepTimer = 0f;

            
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    void PlayFootstep()
    {
        int index = Random.Range(0, footstepClips.Length);
        audioSource.PlayOneShot(footstepClips[index]);
    }
}
