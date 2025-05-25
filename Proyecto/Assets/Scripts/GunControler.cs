using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class GunController : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 1f;
    private float nextFireTime = 0f;

    [Header("Ammo Settings")]
    public int maxAmmo = 5;
    private int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;

    [Header("UI Ammo Display")]
    public List<Image> bulletIcons;         // Assign 5 UI bullet icons
    public Sprite bulletFullSprite;         // Bullet visible
    public Sprite bulletEmptySprite;        // Bullet empty

    [Header("References")]
    private Camera mainCamera;
    private SpriteRenderer playerSprite;
    private SpriteRenderer gunRenderer;
    private Collider2D gunCollider;
    private Vector3 originalPlayerScale;
    private Rigidbody2D playerRb;

    public Animator gunAnimator;
    public Animator playerAnimator;
    public AudioSource gunSound;
    public AudioSource reloadSound;
    public AudioSource emptyClickSound; 


    private bool isDead = false;

    void Start()
    {
        playerRb = transform.parent.GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        playerSprite = transform.parent.GetComponent<SpriteRenderer>();
        gunRenderer = GetComponent<SpriteRenderer>();
        gunCollider = GetComponent<Collider2D>();
        originalPlayerScale = transform.parent.localScale;

        if (playerAnimator == null)
        {
            playerAnimator = transform.parent.GetComponent<Animator>();
        }

        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        // Hide gun initially
        gunRenderer.enabled = false;
        if (gunCollider != null)
            gunCollider.enabled = false;
    }

    void Update()
    {
        if (isDead || isReloading) return;

        bool aiming = Input.GetMouseButton(1);

        gunRenderer.enabled = aiming;
        if (gunCollider != null)
            gunCollider.enabled = aiming;

        if (aiming)
        {
            FlipPlayerToFaceMouse();
            AimGun();

            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }

        // Optional: Gun walk anim
        bool isWalking = Mathf.Abs(playerRb.velocity.x) > 0.01f;
        if (gunAnimator != null)
        {
            gunAnimator.SetBool("IsWalking", isWalking);
        }
    }


    void Shoot()
    {
        if (isDead || isReloading) return;

        if (Time.time < nextFireTime) return;

        if (currentAmmo <= 0)
        {
            // ✅ Play empty click sound
            if (emptyClickSound != null)
            {
                emptyClickSound.Play();
            }

            Debug.Log("Click! Out of ammo.");
            return;
        }

        nextFireTime = Time.time + fireRate;
        currentAmmo--;
        UpdateAmmoUI();

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 shootDirection = firePoint.up.normalized;
            rb.velocity = shootDirection * bulletSpeed;
        }

        if (gunSound != null) gunSound.Play();
        if (gunAnimator != null) gunAnimator.SetTrigger("Shoot");
        if (playerAnimator != null) playerAnimator.SetTrigger("Shoot");
    }




    IEnumerator Reload()
    {
        isReloading = true;

        // 🔊 Reload sound
        if (reloadSound != null) reloadSound.Play();

        // ✅ Tell Animator if player is walking or not
        bool isWalking = Mathf.Abs(playerRb.velocity.x) > 0.1f;
        gunAnimator.SetBool("IsWalking", isWalking);

        // 🎞️ Trigger the correct reload animation
        gunAnimator.SetTrigger("Reload");

        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        isReloading = false;

        Debug.Log("Reload complete.");
    }




    void UpdateAmmoUI()
    {
        for (int i = 0; i < bulletIcons.Count; i++)
        {
            if (i < currentAmmo)
            {
                bulletIcons[i].enabled = true;  // Show bullet
            }
            else
            {
                bulletIcons[i].enabled = false; //  Hide bullet completely
            }
        }
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

        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (gunRenderer != null)
        {
            gunRenderer.flipY = (mousePos.x < transform.position.x);
        }
    }

    public void PlayerDied()
    {
        isDead = true;
        gunRenderer.enabled = false;
        if (gunCollider != null)
            gunCollider.enabled = false;
    }
}
