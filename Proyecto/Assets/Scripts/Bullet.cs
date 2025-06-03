using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 20;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = transform.right * speed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hitObject = collision.gameObject;

        // 🎯 Hit zombie head
        if (hitObject.CompareTag("ZombieHead"))
        {
            ZombieHealth zombieHealth = hitObject.transform.parent.GetComponent<ZombieHealth>();
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(70);
            }
        }
        // 💀 Hit zombie body
        else if (hitObject.CompareTag("Zombie"))
        {
            ZombieHealth zombieHealth = hitObject.GetComponentInParent<ZombieHealth>();
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(30);
            }
        }
        // 🐦 Hit bird enemy
        else if (hitObject.CompareTag("Bird"))
        {
            BirdEnemy bird = hitObject.GetComponent<BirdEnemy>();
            if (bird != null)
            {
                bird.TakeDamage(damage); // Use bullet damage value
            }
        }

        // Destroy bullet on any hit
        Destroy(gameObject);
    }
}
