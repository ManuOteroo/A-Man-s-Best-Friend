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

        if (zombieAudio != null)
        {
            zombieAudio.Stop();
            if (deathSound != null)
            {
                zombieAudio.PlayOneShot(deathSound);
            }
        }

        if (anim != null)
        {
            anim.SetTrigger("Die");

            // Espera un frame para que el Animator actualice el estado
            yield return null;

            // Espera a que comience la animación "Die"
            while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            {
                yield return null;
            }

            // Espera a que se reproduzca por completo
            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
        }

        Destroy(gameObject);
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
