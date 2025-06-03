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
    private Collider birdCollider;

    void Start()
    {
        startPosition = transform.position + idleOffset;
        animator = GetComponent<Animator>();
        birdCollider = GetComponent<Collider>();
        lastAttackTime = -attackCooldown;
        transform.localScale = Vector3.one; 

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
        transform.LookAt(player);
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
        animator.SetTrigger("Die");
        StopAllCoroutines();

        // Disable collision and movement
        if (birdCollider != null)
            birdCollider.enabled = false;

        // Optional: fall down
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Destroy after animation or leave corpse
        Destroy(gameObject, 5f);
    }

    System.Collections.IEnumerator AttackPlayer()
    {
        isAttacking = true;
        animator.SetBool("IsAttacking", true);

        Vector3 attackTarget = player.position + Vector3.up * 1f;
        while (Vector3.Distance(transform.position, attackTarget) > 0.5f && !isDead)
        {
            transform.position = Vector3.MoveTowards(transform.position, attackTarget, attackSpeed * Time.deltaTime);
            transform.LookAt(player);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        while (Vector3.Distance(transform.position, startPosition) > 0.5f && !isDead)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, returnSpeed * Time.deltaTime);
            transform.LookAt(player);
            yield return null;
        }

        animator.SetBool("IsAttacking", false);
        lastAttackTime = Time.time;
        isAttacking = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
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
