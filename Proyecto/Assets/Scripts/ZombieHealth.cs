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

        // Stop all movement and behavior
        ZombieMovement zm = GetComponent<ZombieMovement>();
        if (zm != null)
        {
            zm.enabled = false;
        }

        // Stop sound and play death sound
        if (zombieAudio != null)
        {
            zombieAudio.Stop();
            if (deathSound != null)
            {
                zombieAudio.PlayOneShot(deathSound);
            }
        }

        // Force stop Rigidbody motion and physics
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        // Disable collisions so corpse isn't pushed
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Play death animation
        if (anim != null)
        {
            anim.enabled = true;
            anim.speed = 1f;
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Die");

            yield return null;

            // Wait for the "Die" animation to start
            while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            {
                yield return null;
            }

            // Wait until it's finished
            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }

            // Freeze on last frame
            anim.Play("Die", 0, 0.99f);
            anim.Update(0f);
            anim.enabled = false;
        }

        // Disable this script after death
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
