using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float jump;
    public float runSpeed;
    public int maxJumps = 2;
    public AudioSource jumpSound;
    public ParticleSystem jumpEffect;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private int jumpCount;
    private bool isRunning;
    private float moveInput;
    private bool isGrounded;

    public Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private Vector3 originalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        originalScale = transform.localScale;
    }

    void Update()
    {
        // --- Movement Input ---
        moveInput = Input.GetAxis("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : speed;
        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);

        // --- Ground Check ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            jumpCount = maxJumps;
            anim.SetBool("IsJumping", false);
        }

        // --- Jumping ---
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jump);
            jumpCount--;

            anim.SetBool("IsJumping", true);
            if (jumpSound != null) jumpSound.Play();
            if (jumpEffect != null)
            {
                jumpEffect.transform.position = transform.position - new Vector3(0, 0.5f, 0);
                jumpEffect.Play();
            }
        }

        // --- Flip based on mouse ---
        FlipTowardsMouse();

        // Aiming — completely disabled if jumping
        bool isJumping = anim.GetBool("IsJumping");
        bool isAiming = Input.GetMouseButton(1) && !isJumping;

        anim.SetBool("isAiming", isAiming);



        // --- Walking logic ---
        bool isWalking = Mathf.Abs(moveInput) > 0.1f && isGrounded;
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("IsRunning", isRunning);
        anim.SetFloat("Speed", Mathf.Abs(moveInput));



    }

    void FlipTowardsMouse()
    {
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        bool isFacingLeft = mouseWorld.x < transform.position.x;

        float direction = isFacingLeft ? -1f : 1f;

        transform.localScale = new Vector3(
            Mathf.Abs(originalScale.x) * direction,
            originalScale.y,
            originalScale.z
        );
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
