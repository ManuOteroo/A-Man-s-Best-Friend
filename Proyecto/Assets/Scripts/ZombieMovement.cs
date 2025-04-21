using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    // Speed of Zombie
    public float speed = 2f;

    //Distance for zombie to follow player
    public float followDistance = 5f;

    //Distance for zombie attack on player
    public float attackDistance = 0.5f;

    //Damage Given to player by zombie
    public int attackDamage = 20; 

    //Cooldown of attack
    
    public float attackCooldown = 1.5f;

    // Tracks when the zombie can attack next
    private float nextAttackTime = 0f;

    public Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isMoving = false;
    private bool isAttacking = false;
    private Vector3 originalScale; 

    void Start()
    {
        //References to rigidbody and animator
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        //Helps find player by tag
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

       
        originalScale = transform.localScale; 
    }

    void Update()
    {

        // Only follow player if not in the middle of an attack
        if (!isAttacking) 
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {

        // Abort if player isn't found
        if (player == null)
        {
            return;
        }


        // Calculate distance between zombie and player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Determine which direction to move in
        int moveDirection = (player.position.x > transform.position.x) ? 1 : -1;



        if (distanceToPlayer < followDistance && distanceToPlayer > attackDistance)
        {

            // Move toward player if within follow range but not close enough to attack
            rb.velocity = new Vector2(speed * moveDirection, rb.velocity.y);
            isMoving = true;
            anim.SetBool("IsWalking", true);

            // Flip the zombie's sprite to face the direction of movement
            FlipZombie(moveDirection);
        }


        else if (distanceToPlayer <= attackDistance && Time.time >= nextAttackTime)
        {

            // Stop moving and attack if within attack range and cooldown has passed
            rb.velocity = Vector2.zero;
            isMoving = false;
            anim.SetBool("IsWalking", false);
            AttackPlayer();
        }
        else
        {
            // Idle state
            rb.velocity = Vector2.zero;
            isMoving = false;
            anim.SetBool("IsWalking", false);
        }
    }


    void FlipZombie(int moveDirection)
    {

        // Moving Right
        if (moveDirection > 0) 
        {
            // Face Right
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z); 
        }

        // Moving Left
        else if (moveDirection < 0) 
        {
            // Face Left
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z); 
        }
    }

    void AttackPlayer()
    {

        // Start attack sequence
        isAttacking = true;

        // Play attack animation
        anim.SetTrigger("Attack");

        // Set cooldown
        nextAttackTime = Time.time + attackCooldown;

        // Apply damage to player
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
           
        }

        // Reset attack after 1 second
        Invoke(nameof(ResetAttack), 1f);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }
}
