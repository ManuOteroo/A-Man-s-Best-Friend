using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using UnityEngine;


public class Health : MonoBehaviour
{
    // Max HP for the player
    public int maxHealth = 100;

    // Tracks current Health
    private int currentHealth;

    // Reference to Animator for death animation
    private Animator anim;

    // Flag to prevent repeated death
    public bool isDead = false;

    // Sound player makes when dead
    public AudioSource deathSound;

    private SpriteRenderer spriteRenderer;


    void Start()
    {
        // Set current HP to max at start
        currentHealth = maxHealth;

        // Get Animator component
        anim = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        // If there is no deathSound assigned in Inspector, try to grab AudioSource attached to player
        if (deathSound == null)
        {
            deathSound = GetComponent<AudioSource>();
        }

    }

    // Call this to apply damage to the player
    public void TakeDamage(int damage)
    {
        // Don't apply damage if already dead
        if (isDead) return;

        // Subtract damage from health
        currentHealth -= damage;

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            // Trigger death if health reaches zero 
            Die();
        }
    }

    void Die()
    {


        // Prevent this from running more than once
        if (isDead) return;
        isDead = true;

      
        if (deathSound != null)
        {
            deathSound.Play();
        }

        if (anim != null)
        {
            // Play death animation
            anim.Play("Die");
            anim.SetTrigger("Die");
            Invoke(nameof(FreezeAnimation), anim.GetCurrentAnimatorStateInfo(0).length);
        }






        // Disable gun if player has one
        GunController gun = GetComponentInChildren<GunController>();
        if (gun != null)
        {
            gun.PlayerDied();
        }

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            // Show the restart button via GameManager
            gameManager.ShowRestartButton_Death();
        }


        // Disable movement script
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }


    }

    //  Freeze the animation on the last frame
    void FreezeAnimation()
    {
        if (anim != null)
        {
            // Stops animator to keep the death pose
            anim.enabled = false; 
           
        }
    }


    // Function to access current HP
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        // Check if the zombie is dead already
        if (collision.gameObject.CompareTag("Zombie"))
        {
           
            ZombieHealth zombieHealth = collision.gameObject.GetComponent<ZombieHealth>();
            if (zombieHealth != null && zombieHealth.isDead)
            {
              
                return; 
            }

            // Take damage when colliding with a live zombie
            TakeDamage(20); 
        }
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = new Color(0.9f, 0.25f, 0.05f);
        yield return new WaitForSeconds(0.5f); // medio segundo
        spriteRenderer.color = Color.white; // color original del sprite
    }


}
