using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // prefab for the bullet to apear when firing
    public GameObject bulletPrefab;

    // the point where bullets are spawned from
    public Transform firePoint;

    // speed of bullet
    public float bulletSpeed = 20f;

    // delay between consecutive shots
    public float fireRate = 0.2f;

    // Timestamp of when the next shot can occur
    private float nextFireTime = 0f;

    private Camera mainCamera;

    // Reference to the player's sprite for flipping
    private SpriteRenderer playerSprite;

    // Reference to the gun's Animator component
    public Animator gunAnimator;

    // audio source used to play the gunshot sound
    public AudioSource gunSound;

    private bool isDead = false;

    void Start()
    {
        
        mainCamera = Camera.main;

        // Get the player's SpriteRenderer from the parent GameObject
        playerSprite = transform.parent.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // if player is dead, do nothing
        if (isDead) return;

        // Fire when left mouse button is clicked
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        // continuously aim the gun toward the mouse
        AimGun();
    }

    void AimGun()
    {
        // Prevent aiming if player is dead
        if (isDead) return;

        // Get mouse position in world space
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // Calculate direction and rotation angle to face the mouse
        Vector3 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Determine if player should be flipped horizontally
        bool isFacingLeft = mousePosition.x < transform.position.x;

        // Flip the player's sprite based on aim direction
        playerSprite.flipX = isFacingLeft;

        // Rotate the gun to point at the mouse
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Flip the gun's vertical scale based on aim direction
        transform.localScale = new Vector3(1, isFacingLeft ? -1 : 1, 1);
    }

    

    public Animator armAnimator;

    void Shoot()
    {
        
        if (isDead) return;

        // Prevent shooting if prefab or firePoint is not set
        if (bulletPrefab == null || firePoint == null)
        {
            return;
        }

        // Spawn a new bullet at the fire point
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        
        if (rb != null)
        {
            Vector2 shootDirection = firePoint.up.normalized;
            rb.velocity = shootDirection * bulletSpeed;
        }

        // Play the gunshot sound if available
        if (gunSound != null)
        {
            gunSound.Play();
        }
    }

   
    public void PlayerDied()
    {
        isDead = true;

        // Reset sprite flip to avoid rotation issues
        playerSprite.flipX = false;

        // Disable the gun GameObject
        gameObject.SetActive(false);
    }
}
