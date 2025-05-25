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

        
        if (rb == null)
        {
            return;
        }

        rb.velocity = transform.right * speed;
    }

    
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("ZombieHead"))
        {
            
            ZombieHealth zombieHealth = collision.transform.parent.GetComponent<ZombieHealth>();

            
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(70);
            }

            
            Destroy(gameObject);
        }
        // if the bullet hits the zombie's body
        else if (collision.gameObject.CompareTag("Zombie"))
        {
            ZombieHealth zombieHealth = collision.gameObject.GetComponentInParent<ZombieHealth>();

            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(30);
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
