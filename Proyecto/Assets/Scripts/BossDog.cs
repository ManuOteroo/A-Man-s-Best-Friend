using UnityEngine;
using System.Collections;

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

    private Rigidbody2D rb;
    private Collider2D col;

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
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

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

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        animator.SetTrigger("attack");

        yield return new WaitForSeconds(0.4f);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);
            }
        }

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.enabled = true;
            animator.speed = 1f;
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
            animator.SetTrigger("die");

            StartCoroutine(FreezeAtEndOfDeath());
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        if (col != null)
        {
            col.enabled = false;
        }

        this.enabled = false;

        // Trigger final scene AFTER delay to allow gunshot to finish
        StartCoroutine(DelayedCutsceneTrigger());
    }

    IEnumerator FreezeAtEndOfDeath()
    {
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName("Die") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f
        );

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        animator.Play(state.fullPathHash, 0, 0.99f);
        animator.Update(0f);
        animator.enabled = false;
    }

    IEnumerator DelayedCutsceneTrigger()
    {
        yield return new WaitForSeconds(0.5f); // Delay so gunshot plays fully
        FinalSceneController controller = FindObjectOfType<FinalSceneController>();
        if (controller != null)
        {
            controller.TriggerFinalCutscene();
        }
    }
}
