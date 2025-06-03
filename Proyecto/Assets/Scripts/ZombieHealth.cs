using System.Collections;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public bool isDead = false;

    private Animator anim;

    public AudioClip deathSoundClip; 
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (deathSoundClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSoundClip);
        }

        if (anim != null)
        {
            anim.SetTrigger("Die");
            StartCoroutine(HoldFinalDeathFrame());
        }

        ZombieMovement movement = GetComponent<ZombieMovement>();
        if (movement != null)
        {
            movement.OnDeath();
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        foreach (MonoBehaviour script in GetComponents<MonoBehaviour>())
        {
            if (script != this && !(script is Animator))
            {
                script.enabled = false;
            }
        }
    }

    private IEnumerator HoldFinalDeathFrame()
    {
        yield return new WaitForSeconds(0.1f);

        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            yield return null;

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.98f)
            yield return null;

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        anim.Play(state.fullPathHash, 0, 0.99f);
        anim.Update(0f);
        anim.enabled = false;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
