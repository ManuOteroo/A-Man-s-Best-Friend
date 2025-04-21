using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // walking speed
    public float speed;

    // jump force applied when jumping
    public float jump;

    // Speed when running (holding shift)
    public float runSpeed;

    // maximum number of jumps allowed for double jump
    public int maxJumps = 2;

    // Tracks how many jumps the player has left
    private int jumpCount;

    // Audio source for jump sound effect
    public AudioSource jumpSound;

    
    private bool isRunning;
    private float Move;
    public Rigidbody2D rb;
    private Animator anim;

    // Particle effect played when jumping
    public ParticleSystem jumpEffect;

    // Tracks whether the player is in the air
    public bool isJumping;

    void Start()
    {
        // get Rigidbody2D and Animator components at startup
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move = Input.GetAxis("Horizontal");

        // check if the player is holding the run key (Left Shift)
        isRunning = Input.GetKey(KeyCode.LeftShift);

        
        float currentSpeed = isRunning ? runSpeed : speed;

        // Move the player horizontally based on input
        rb.velocity = new Vector2(currentSpeed * Move, rb.velocity.y);

       
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            // Apply vertical velocity to jump
            rb.velocity = new Vector2(rb.velocity.x, jump);

            // Decrease jump count
            jumpCount--;

            // play jump sound if assigned
            if (jumpSound != null)
            {
                jumpSound.Play();
            }

            // Trigger jump animation
            anim.SetBool("IsJumping", true);

            // Play jump particle effect
            PlayJumpEffect();
        }

        // Update movement animations
        anim.SetFloat("Speed", Mathf.Abs(Move));
        anim.SetBool("IsRunning", isRunning);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Reset jump count when touching the floor and falling or grounded
        if (collision.gameObject.CompareTag("Floor") && rb.velocity.y <= 0)
        {
            jumpCount = maxJumps;
            anim.SetBool("IsJumping", false);
        }
    }

    // plays the jump particle effect
    void PlayJumpEffect()
    {
        if (jumpEffect != null)
        {
            jumpEffect.Play();
        }
    }

    
    void PlayjumpEffect()
    {
        if (jumpEffect != null)
        {
            jumpEffect.transform.position = transform.position - new Vector3(0, 0.5f, 0);
            jumpEffect.Play();
        }
    }
}

