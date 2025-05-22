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
    private Vector3 originalScale;

    // Animation control
    public string walkStateName = "Walk"; // Name of the full walk animation
    public float walkTotalDuration = 1f;  // Total duration of walk animation (in seconds)
    public int totalFrames = 10;          // Total number of frames in the animation
    public int loopStartFrame = 4;        // Start looping from this frame

    private bool playedIntro = false;
    private float loopTimer = 0f;

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

            if (!isMoving)
            {
                playedIntro = false;
                loopTimer = 0f;
                anim.Play(walkStateName, 0, 0f); // Start from beginning
                anim.speed = 1f; // Allow intro to play
            }

            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

            if (!playedIntro)
            {
                // Wait until the intro section (frames 1–3) finishes
                float loopStartNormalizedTime = (float)loopStartFrame / totalFrames;
                if (state.IsName(walkStateName) && state.normalizedTime >= loopStartNormalizedTime)
                {
                    playedIntro = true;
                    loopTimer = 0f;
                }
            }
            else
            {
                // Loop frames 4+ manually
                anim.speed = 0f; // Stop Animator from progressing automatically
                loopTimer += Time.fixedDeltaTime;

                float loopStart = (float)loopStartFrame / totalFrames;
                float loopLength = 1f - loopStart;
                float loopTime = loopStart + (loopTimer % (walkTotalDuration * loopLength)) / (walkTotalDuration * loopLength);

                anim.Play(walkStateName, 0, loopTime);
                anim.Update(0f); // Apply frame immediately
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
        else if (moveDirection < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z); // Face left
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && Time.time >= nextAttackTime && !isAttacking)
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
        isAttacking = true;
        anim.SetTrigger("Attack");
        nextAttackTime = Time.time + attackCooldown;

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10);
        }

        Invoke(nameof(ResetAttack), 1f);
    }

    void ResetAttack()
    {
        isAttacking = false;
        Debug.Log("Ataque terminado");
    }
}
