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

        if (hitObject.CompareTag("ZombieHead"))
        {
            ZombieHealth zombie = hitObject.GetComponentInParent<ZombieHealth>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage * 2);
                Destroy(gameObject);
                return;
            }
        }
        else if (hitObject.CompareTag("Zombie"))
        {
            ZombieHealth zombie = hitObject.GetComponentInParent<ZombieHealth>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }
        else if (hitObject.CompareTag("Boss"))
        {
            BossDog dog = hitObject.GetComponent<BossDog>();
            if (dog != null)
            {
                dog.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
