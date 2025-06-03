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
        if (collision.gameObject.CompareTag("Boss")) // Asegúrate de que el perro tenga este tag
        {
            BossDog dog = collision.gameObject.GetComponent<BossDog>();
            if (dog != null)
            {
                dog.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
