using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{

    // Maximum health the zombie can have
    public int maxHealth = 100;

    //Zombie´s current health

    private int currentHealth;
    private Animator anim;
    public bool isDead = false;
    private Rigidbody2D rb;
    private Collider2D[] colliders;
    private SpriteRenderer spriteRenderer;
    public AudioSource zombieAudio; 
    public AudioClip idleSound; 
    public AudioClip deathSound; 

    void Start()
    {

        // Set initial health
        currentHealth = maxHealth;

        //References
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider2D>();


        // Play idle loop sound 
        if (zombieAudio != null && idleSound != null)
        {
            zombieAudio.clip = idleSound;
            zombieAudio.loop = true; 
            zombieAudio.Play();
        }


    }




    // method for taking damage from external sources
    public void TakeDamage(int damage)
    {

        // Prevent further damage after death
        if (isDead) return; 

        currentHealth -= damage;

        // Trigger death if health reaches zero or below
        if (currentHealth <= 0)
        {
            Die();
        }
    }


    // handles the zombie's death sequence
    void Die()
    {

        // prevent running this method more than once
        if (isDead) return;
        isDead = true;



        // Trigger death animation
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }



        // Reset movement and allow gravity to pull body down
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Dynamic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 1f;
            rb.simulated = true;

            StartCoroutine(WaitToFreezeOnGround());
        }



        // Disable all colliders
        Collider2D[] allCols = GetComponentsInChildren<Collider2D>(includeInactive: true);

        foreach (Collider2D col in allCols)
        {
            col.enabled = false;
        }





        // Disable all scripts except this one and the animator
        foreach (MonoBehaviour component in GetComponents<MonoBehaviour>())
        {
            if (component != this && component != anim)
            {
                component.enabled = false;
            }
        }


        // Stop idle sound and play death sound
        if (zombieAudio != null)
        {
            zombieAudio.Stop();
            if (deathSound != null)
            {
                
                zombieAudio.PlayOneShot(deathSound); 
            }
        }
    }


    // waits until the zombie has landed before freezing its body and disabling colliders
    private IEnumerator WaitToFreezeOnGround()
    {
       
        while (!IsTouchingGround())
        {
            yield return null;
        }

       

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        Collider2D[] allCols = GetComponentsInChildren<Collider2D>(includeInactive: true);
        foreach (Collider2D col in allCols)
        {
            col.enabled = false;
        }

        
    }


    // Checks if zombie is currently touching the ground
    private bool IsTouchingGround()
    {
        
        return Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
    }


    void OnTriggerEnter2D(Collider2D other)
    {
       

        if (other.CompareTag("Spike"))
        {
            
            TakeDamage(999); 
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

}

