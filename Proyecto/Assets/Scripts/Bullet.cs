using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // speed at which the bullet travels
    public float speed = 20f;

    // amount of damage the bullet inflicts
    public int damage = 20;

    private Rigidbody2D rb;

    void Start()
    {
       
        rb = GetComponent<Rigidbody2D>();

        
        if (rb == null)
        {
            return;
        }

        rb.velocity = transform.right * speed;
    }

    // Called when the bullet collides with another collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        // if the bulet hits a zombie's head
        if (collision.gameObject.CompareTag("ZombieHead"))
        {
            // access the parent GameObject that holds the ZombieHealth script
            ZombieHealth zombieHealth = collision.transform.parent.GetComponent<ZombieHealth>();

            // Apply damage for a headshot
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(70);
            }

            // Destroy the bullet after impact
            Destroy(gameObject);
        }
        // if the bullet hits the zombie's body
        else if (collision.gameObject.CompareTag("Zombie"))
        {
            ZombieHealth zombieHealth = collision.gameObject.GetComponentInParent<ZombieHealth>();

            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else
        {
            // destroy the bullet if it hits anything else 
            Destroy(gameObject);
        }
    }
}
