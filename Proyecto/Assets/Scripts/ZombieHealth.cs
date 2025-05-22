using System.Collections;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public int maxHealth = 1000;
    private int currentHealth;
    private Animator anim;
    public bool isDead = false;

    public AudioSource zombieAudio;
    public AudioClip idleSound;
    public AudioClip deathSound;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();

        if (zombieAudio != null && idleSound != null)
        {
            zombieAudio.clip = idleSound;
            zombieAudio.loop = true;
            zombieAudio.Play();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public IEnumerator Die()
    {
        isDead = true;

        // Stop AI movement
        ZombieMovement zm = GetComponent<ZombieMovement>();
        if (zm != null)
        {
            zm.enabled = false;
            zm.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        // Play death sound
        if (zombieAudio != null)
        {
            zombieAudio.Stop();
            if (deathSound != null)
            {
                zombieAudio.PlayOneShot(deathSound);
            }
        }

        // Play death animation
        if (anim != null)
        {
            anim.SetTrigger("Die");

            yield return null;

            // Wait for "Die" to start
            while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            {
                yield return null;
            }

            // Wait for it to fully finish
            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }

            // ✅ Snap to last frame and freeze it
            anim.Play("Die", 0, 0.99f); // Stay on last frame
            anim.Update(0f); // Apply pose immediately
            anim.enabled = false; // Freeze the animation here
        }

        // Freeze physics
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        // Disable collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Disable this script
        this.enabled = false;
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
