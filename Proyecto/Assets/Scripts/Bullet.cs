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

    // 💥 Detect collisions with zombies
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hit = collision.gameObject;

        if (hit.CompareTag("ZombieHead"))
        {
            ZombieHealth zombieHealth = hit.transform.parent.GetComponent<ZombieHealth>();
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(70);
            }
        }
        else if (hit.CompareTag("Zombie"))
        {
            ZombieHealth zombieHealth = hit.GetComponentInParent<ZombieHealth>();
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(30);
            }
        }

        Destroy(gameObject);
    }

    // 🐦 Detect trigger hits with birds
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bird"))
        {
            BirdEnemy bird = other.GetComponent<BirdEnemy>();
            if (bird != null)
            {
                bird.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }


}
