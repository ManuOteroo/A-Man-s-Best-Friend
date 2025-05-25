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

    private int jumpCount;
    private bool isRunning;
    private float moveInput;
    public Rigidbody2D rb;
    private Animator anim;
    public bool isJumping;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
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
        // --- Movement ---
        moveInput = Input.GetAxis("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : speed;

        rb.velocity = new Vector2(currentSpeed * moveInput, rb.velocity.y);

        // --- Jumping ---
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jump);
            jumpCount--;

            if (jumpSound != null) jumpSound.Play();
            anim.SetBool("IsJumping", true);
            PlayJumpEffect();
        }

        // --- Aiming ---
        bool isAiming = Input.GetMouseButton(1); // Right-click to aim
        anim.SetBool("IsShooting", isAiming);

        // --- Flipping towards mouse ---
        FlipTowardsMouse();

        // --- Animation states ---
        bool isWalking = Mathf.Abs(moveInput) > 0.1f && !isJumping;

        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("IsRunning", isRunning);
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isAiming", isAiming);
    }

    void FlipTowardsMouse()
    {
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        bool isFacingLeft = mouseWorld.x < transform.position.x;

        float direction = isFacingLeft ? -1f : 1f;

        // Flip entire player (including gun and firepoint)
        transform.localScale = new Vector3(
            Mathf.Abs(originalScale.x) * direction,
            originalScale.y,
            originalScale.z
        );
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor") && rb.velocity.y <= 0)
        {
            jumpCount = maxJumps;
            anim.SetBool("IsJumping", false);
        }
    }

    void PlayJumpEffect()
    {
        if (jumpEffect != null)
        {
            jumpEffect.transform.position = transform.position - new Vector3(0, 0.5f, 0);
            jumpEffect.Play();
        }
    }
}
