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

    // Walk animation control
    [Header("Walk Animation Loop Settings")]
    public string walkStateName = "Walk";  // Must match Animator state name
    public float walkTotalDuration = 1f;   // Total length of walk animation in seconds
    public int totalFrames = 10;           // Total frame count in the animation
    public int loopStartFrame = 4;         // Frame to start looping from

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
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        int moveDirection = (player.position.x > transform.position.x) ? 1 : -1;

        if (distanceToPlayer < followDistance)
        {
            rb.velocity = new Vector2(speed * moveDirection, rb.velocity.y);

            if (!isMoving)
            {
                // Play full walk animation from the beginning
                playedIntro = false;
                loopTimer = 0f;
                anim.Play(walkStateName, 0, 0f);
                anim.speed = 1f;
            }

            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

            if (!playedIntro)
            {
                float loopStartNormalizedTime = (float)loopStartFrame / totalFrames;
                if (state.IsName(walkStateName) && state.normalizedTime >= loopStartNormalizedTime)
                {
                    playedIntro = true;
                    loopTimer = 0f;
                }
            }
            else
            {
                // Manually loop only frames 4 to 10
                anim.speed = 0f;
                loopTimer += Time.fixedDeltaTime;

                float loopStart = (float)loopStartFrame / totalFrames;
                float loopLength = 1f - loopStart;
                float loopTime = loopStart + (loopTimer % (walkTotalDuration * loopLength)) / (walkTotalDuration * loopLength);

                anim.Play(walkStateName, 0, loopTime);
                anim.Update(0f);
            }

            isMoving = true;
            FlipZombie(moveDirection);
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
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z); // Face right
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z); // Face left
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
        if (isDead) return;

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
    }

    // Called by ZombieHealth when zombie dies
    public void OnDeath()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        anim.speed = 1f;
    }
}
