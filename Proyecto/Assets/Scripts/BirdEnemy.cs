using System.Collections;
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
    private Rigidbody2D rb;

    void Start()
    {
        startPosition = transform.position + idleOffset;
        animator = GetComponent<Animator>();
        birdCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }

        if (birdCollider != null)
        {
            birdCollider.isTrigger = false; // Use collision for bullets
        }

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
        transform.right = -direction.normalized; // for left-facing sprite
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
        {
            animator.SetTrigger("Die");
            StartCoroutine(FreezeAnimatorAfterDeath());
        }

        if (birdCollider != null)
            birdCollider.isTrigger = true; // allow player to walk through

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1f;
            rb.velocity = Vector2.zero;
        }

        StartCoroutine(SnapToGroundAfterFall());
        StartCoroutine(FreezeBirdAfterFall());

        Destroy(gameObject, 5f);
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        if (animator != null)
            animator.SetBool("IsAttacking", true);

        while (Vector3.Distance(transform.position, player.position + Vector3.up * 0.5f) > 0.3f && !isDead)
        {
            Vector3 dynamicTarget = player.position + Vector3.up * 0.5f;
            transform.position = Vector3.MoveTowards(transform.position, dynamicTarget, attackSpeed * Time.deltaTime);
            Vector3 direction = dynamicTarget - transform.position;
            transform.right = -direction.normalized;
            yield return null;
        }

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
            transform.right = -direction.normalized;
            yield return null;
        }

        if (animator != null)
            animator.SetBool("IsAttacking", false);

        lastAttackTime = Time.time;
        isAttacking = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null && !playerHealth.isDead)
            {
                playerHealth.TakeDamage(20);
            }
        }
    }

    IEnumerator FreezeAnimatorAfterDeath()
    {
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName("Die") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f
        );

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        animator.Play(state.fullPathHash, 0, 0.999f);
        animator.Update(0f);
        animator.enabled = false;
    }

    IEnumerator SnapToGroundAfterFall()
    {
        yield return new WaitForSeconds(0.3f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 5f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            Bounds bounds = birdCollider.bounds;
            Vector3 newPos = transform.position;
            newPos.y = hit.point.y + bounds.extents.y;
            transform.position = newPos;
        }
    }

    IEnumerator FreezeBirdAfterFall()
    {
        yield return new WaitForSeconds(0.4f);

        if (rb != null)
            rb.bodyType = RigidbodyType2D.Static;
    }
}
