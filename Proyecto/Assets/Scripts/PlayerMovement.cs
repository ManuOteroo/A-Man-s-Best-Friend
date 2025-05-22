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
    private float Move;
    public Rigidbody2D rb;
    private Animator anim;
    public bool isJumping;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        Move = Input.GetAxis("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : speed;

        rb.velocity = new Vector2(currentSpeed * Move, rb.velocity.y);

        // Saltar
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jump);
            jumpCount--;

            if (jumpSound != null) jumpSound.Play();
            anim.SetBool("IsJumping", true);
            PlayJumpEffect();
        }

        // Set animation parameters
        anim.SetFloat("Speed", Mathf.Abs(Move));
        anim.SetBool("IsRunning", isRunning);

        // Flip hacia el ratón
        FlipTowardsMouse();

        // Activar bool de disparo con click derecho
        if (Input.GetMouseButtonDown(1))
        {
            anim.SetBool("IsShooting", true);
        }
        if (Input.GetMouseButtonUp(1))
        {
            anim.SetBool("IsShooting", false);
        }
    }



    void FlipTowardsMouse()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        bool isFacingLeft = mousePos.x < transform.position.x;
        spriteRenderer.flipX = isFacingLeft;
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
            jumpEffect.Play();
        }
    }

    void PlayjumpEffect()
    {
        if (jumpEffect != null)
        {
            jumpEffect.transform.position = transform.position - new Vector3(0, 0.5f, 0);
            jumpEffect.Play();
        }
    }
}
