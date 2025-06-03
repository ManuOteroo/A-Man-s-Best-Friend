using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdEnemy : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 15f;
    public float attackSpeed = 10f;
    public float returnSpeed = 5f;
    public float attackCooldown = 5f;
    public Vector3 idleOffset = new Vector3(0, 5f, 0);
    public int health = 100;

    private Vector3 startPosition;
    private bool isAttacking = false;
    private bool isDead = false;
    private float lastAttackTime;
    private Animator animator;
    private Collider2D birdCollider;

    void Start()
    {
        transform.localScale = Vector3.one;
        startPosition = transform.position + idleOffset;
        animator = GetComponent<Animator>();
        birdCollider = GetComponent<Collider2D>();
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown && distanceToPlayer < detectionRange)
        {
            StartCoroutine(AttackPlayer());
        }
        else if (!isAttacking)
        {
            PatrolHover();
        }
    }

    void PatrolHover()
    {
        transform.position = startPosition + new Vector3(0, Mathf.Sin(Time.time * 2f) * 0.5f, 0);
        Vector3 direction = player.position - transform.position;
        transform.right = direction.normalized;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        StopAllCoroutines();

        if (animator != null)
            animator.SetTrigger("Die");

        if (birdCollider != null)
            birdCollider.isTrigger = false; // Allow collision with ground

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1f;
            rb.velocity = Vector2.zero;
        }

        Destroy(gameObject, 5f);
    }

    System.Collections.IEnumerator AttackPlayer()
    {
        isAttacking = true;

        if (animator != null)
            animator.SetBool("IsAttacking", true);

        Vector3 attackTarget = player.position + Vector3.up * 0.5f;

        while (Vector3.Distance(transform.position, attackTarget) > 0.3f && !isDead)
        {
            transform.position = Vector3.MoveTowards(transform.position, attackTarget, attackSpeed * Time.deltaTime);
            Vector3 direction = player.position - transform.position;
            transform.right = direction.normalized;
            yield return null;
        }

        // Damage the player directly
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null && !playerHealth.isDead)
        {
            playerHealth.TakeDamage(20);
        }

        yield return new WaitForSeconds(0.2f);

        while (Vector3.Distance(transform.position, startPosition) > 0.3f && !isDead)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, returnSpeed * Time.deltaTime);
            Vector3 direction = player.position - transform.position;
            transform.right = direction.normalized;
            yield return null;
        }

        if (animator != null)
            animator.SetBool("IsAttacking", false);

        lastAttackTime = Time.time;
        isAttacking = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null && !playerHealth.isDead)
            {
                playerHealth.TakeDamage(20);
            }
        }
    }
}
