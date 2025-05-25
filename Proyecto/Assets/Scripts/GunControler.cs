using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 1f;
    private float nextFireTime = 0f;

    private Camera mainCamera;
    private SpriteRenderer playerSprite;
    private SpriteRenderer gunRenderer;
    private Collider2D gunCollider;
    private Vector3 originalPlayerScale;
    private Rigidbody2D playerRb;



    public Animator gunAnimator;      // Animator for the gun 
    public Animator playerAnimator;   
    public AudioSource gunSound;

    private bool isDead = false;

    void Start()
    {
        playerRb = transform.parent.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            float horizontalSpeed = Mathf.Abs(playerRb.velocity.x);

            bool isWalking = horizontalSpeed > 0.1f;

            gunAnimator.SetBool("IsWalking", isWalking);

            Debug.Log("GunController IsWalking: " + isWalking + " (velocity.x = " + horizontalSpeed + ")");
        }


        mainCamera = Camera.main;
        playerSprite = transform.parent.GetComponent<SpriteRenderer>();
        gunRenderer = GetComponent<SpriteRenderer>();
        gunCollider = GetComponent<Collider2D>();
        originalPlayerScale = transform.parent.localScale;

      
        if (playerAnimator == null)
        {
            playerAnimator = transform.parent.GetComponent<Animator>();
        }

        // Hide gun at start
        gunRenderer.enabled = false;
        if (gunCollider != null)
            gunCollider.enabled = false;
    }

    void Update()
    {
        if (isDead) return;

        bool aiming = Input.GetMouseButton(1); // Right-click to aim

        // Show/hide the gun sprite
        gunRenderer.enabled = aiming;
        if (gunCollider != null)
            gunCollider.enabled = aiming;

        if (aiming)
        {
            FlipPlayerToFaceMouse();
            AimGun();

            if (Input.GetMouseButtonDown(0)) // Left-click to shoot
            {
                Shoot();
            }
        }


        bool IsWalking = Mathf.Abs(playerRb.velocity.x) > 0.01f;

        if (gunAnimator != null)
        {
            gunAnimator.SetBool("IsWalking", IsWalking);
        }
        Debug.Log("Gun IsWalking: " + gunAnimator.GetBool("IsWalking"));
        Debug.Log("Current Gun State: " + gunAnimator.GetCurrentAnimatorStateInfo(0).IsName("GunWalking"));


    }

    void FlipPlayerToFaceMouse()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float direction = mousePosition.x < transform.parent.position.x ? -1f : 1f;

        transform.parent.localScale = new Vector3(
            Mathf.Abs(originalPlayerScale.x) * direction,
            originalPlayerScale.y,
            originalPlayerScale.z
        );
    }


    void AimGun()
    {
        if (isDead) return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the gun
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Flip the gun sprite by scaling Y if aiming left
        if (mousePos.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, -1, 1); // flip vertically
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1); // normal
        }
    }




    void Shoot()
    {
        if (isDead || bulletPrefab == null || firePoint == null) return;
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 shootDirection = firePoint.up.normalized;
            rb.velocity = shootDirection * bulletSpeed;
        }

        if (gunSound != null)
        {
            gunSound.Play();
        }

        if (gunAnimator != null)
        {
            gunAnimator.SetTrigger("Shoot");
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Shoot"); 
        }
    }

    public void PlayerDied()
    {
        isDead = true;
        playerSprite.flipX = false;

        gunRenderer.enabled = false;
        if (gunCollider != null)
            gunCollider.enabled = false;
    }
}
