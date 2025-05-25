using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private Animator anim;
    public bool isDead = false;
    public AudioSource deathSound;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (deathSound == null)
        {
            deathSound = GetComponent<AudioSource>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (deathSound != null)
        {
            deathSound.Play();
        }

        if (anim != null)
        {
            anim.ResetTrigger("Die");
            anim.Play("Die", 0, 0f);
            anim.SetTrigger("Die");
            StartCoroutine(FreezeAnimatorAfterDeath());
        }

        GunController gun = GetComponentInChildren<GunController>();
        if (gun != null)
        {
            gun.PlayerDied();
        }

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ShowRestartButton_Death();
        }

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;

           
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

    }

    IEnumerator FreezeAnimatorAfterDeath()
    {
     
        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorStateInfo(0).IsName("Die") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f
        );

        
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        anim.Play(state.fullPathHash, 0, 0.999f);
        anim.Update(0f); 

        
        anim.enabled = false;
    }



    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            ZombieHealth zombieHealth = collision.gameObject.GetComponent<ZombieHealth>();
            if (zombieHealth != null && zombieHealth.isDead) return;

            TakeDamage(20);
        }
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = new Color(0.9f, 0.25f, 0.05f);
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.white;
    }
}
