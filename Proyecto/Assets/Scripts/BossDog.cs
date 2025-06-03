using UnityEngine;

public class BossDog : MonoBehaviour
{
    [Header("Comportamiento")]
    public float detectionRange = 20f;
    public float attackRange = 2f;
    public float moveSpeed = 3f;
    public float damage = 20f;
    public float attackCooldown = 2f;

    [Header("Vida")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Referencias")]
    public Animator animator;

    private Transform player;
    private float lastAttackTime;
    private bool isDead = false;
    private bool isAttacking = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("No se encontró el jugador con la etiqueta 'Player'");
            enabled = false;
            return;
        }

        if (animator == null) animator = GetComponent<Animator>();

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            animator.SetBool("isRunning", !isAttacking);

            if (!isAttacking)
            {
                MoveTowardsPlayer();
            }

            if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
            {
                StartCoroutine(PerformAttack());
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    System.Collections.IEnumerator PerformAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        animator.SetTrigger("attack");

        yield return new WaitForSeconds(0.4f); // espera antes del daño

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);
            }
        }

        yield return new WaitForSeconds(0.4f); // espera tras el golpe

        isAttacking = false;
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        Debug.Log("Perro recibió daño: " + dmg + " | Vida actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("die");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}
