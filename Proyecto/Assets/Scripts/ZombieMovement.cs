using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    public float speed = 2f;
    public float followDistance = 5f;
    public int attackDamage = 1;
    public float attackCooldown = 1.5f;

    private float nextAttackTime = 0f;

    public Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isDead = false;
    private Vector3 originalScale;

    [Header("Walk Animation Loop Settings")]
    public string walkStateName = "Walk";
    public float walkTotalDuration = 1.75f;
    public int loopStartFrame = 10;
    public int loopEndFrame = 21;

    private bool playedIntro = false;
    private float loopTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        originalScale = transform.localScale;
    }

    void FixedUpdate()
    {
        if (isDead || isAttacking || player == null) return;
        FollowPlayer();
    }

    void FollowPlayer()
    {
        if (isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float diffX = player.position.x - transform.position.x;
        int moveDirection = 0;

        if (Mathf.Abs(diffX) > 0.1f) // evitar flip constante
        {
            moveDirection = (diffX > 0) ? 1 : -1;
        }

        if (distanceToPlayer < followDistance)
        {
            rb.velocity = new Vector2(speed * moveDirection, rb.velocity.y);

            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            float loopStartNorm = (float)(loopStartFrame - 1) / loopEndFrame;

            if (!playedIntro)
            {
                if (!isMoving || !state.IsName(walkStateName))
                {
                    anim.speed = 1f;
                    anim.Play(walkStateName, 0, 0f);
                }

                if (state.IsName(walkStateName) && state.normalizedTime >= loopStartNorm)
                {
                    playedIntro = true;
                    loopTimer = 0f;
                    anim.speed = 0f;
                }
            }
            else
            {
                if (!state.IsName(walkStateName))
                {
                    anim.Play(walkStateName, 0, (float)(loopStartFrame - 1) / loopEndFrame);
                    anim.Update(0f);
                    loopTimer = 0f;
                }

                loopTimer += Time.fixedDeltaTime;

                float loopStart = (float)(loopStartFrame - 1) / loopEndFrame;
                float loopLength = (float)(loopEndFrame - loopStartFrame + 1) / loopEndFrame;
                float loopNormalizedTime = loopStart + (loopTimer % (walkTotalDuration * loopLength)) / (walkTotalDuration * loopLength);

                anim.Play(walkStateName, 0, loopNormalizedTime);
                anim.Update(0f);
            }

            isMoving = true;

            if (moveDirection != 0)
            {
                FlipZombie(moveDirection);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            isMoving = false;
            playedIntro = false;
            loopTimer = 0f;
            anim.speed = 1f;
        }
    }

    void FlipZombie(int moveDirection)
    {
        if (moveDirection > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (moveDirection < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player") && Time.time >= nextAttackTime && !isAttacking)
        {
            rb.velocity = Vector2.zero;
            isMoving = false;
            playedIntro = false;
            loopTimer = 0f;
            anim.speed = 1f;
            AttackPlayer(other.GetComponent<Health>());
        }
    }

    void AttackPlayer(Health playerHealth)
    {
        if (isDead || isAttacking || anim == null) return;

        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        Debug.Log("🧟 Lanzando ataque al jugador");
        anim.ResetTrigger("Attack");
        anim.SetTrigger("Attack");

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }

        Invoke(nameof(ResetAttack), 1f);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    public void OnDeath()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        anim.speed = 1f;
    }
}
