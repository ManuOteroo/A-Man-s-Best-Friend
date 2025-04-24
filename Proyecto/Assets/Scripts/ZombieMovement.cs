using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    public float speed = 2f;
    public float followDistance = 5f;
    public int attackDamage = 20;
    public float attackCooldown = 1.5f;

    private float nextAttackTime = 0f;

    public Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isMoving = false;
    private bool isAttacking = false;
    private Vector3 originalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        originalScale = transform.localScale;
    }

    void FixedUpdate()
    {
        if (!isAttacking && player != null)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        int moveDirection = (player.position.x > transform.position.x) ? 1 : -1;

        if (distanceToPlayer < followDistance)
        {
            rb.velocity = new Vector2(speed * moveDirection, rb.velocity.y);
            isMoving = true;
            anim.SetBool("IsWalking", true);
            FlipZombie(moveDirection);
        }
        else
        {
            rb.velocity = Vector2.zero;
            isMoving = false;
            anim.SetBool("IsWalking", false);
        }
    }

    void FlipZombie(int moveDirection)
    {
        if (moveDirection > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (moveDirection < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && Time.time >= nextAttackTime && !isAttacking)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("IsWalking", false);
            AttackPlayer(other.GetComponent<Health>());
        }
    }

    void AttackPlayer(Health playerHealth)
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        nextAttackTime = Time.time + attackCooldown;

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }

        Invoke(nameof(ResetAttack), 1f);
    }

    void ResetAttack()
    {
        isAttacking = false;
        Debug.Log("Ataque terminado");
    }
}
