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

    public Animator gunAnimator;
    public AudioSource gunSound;

    private bool isDead = false;

    void Start()
    {
        mainCamera = Camera.main;
        playerSprite = transform.parent.GetComponent<SpriteRenderer>();
        gunRenderer = GetComponent<SpriteRenderer>();
        gunCollider = GetComponent<Collider2D>();

        // Ocultar el arma al inicio
        gunRenderer.enabled = false;
        if (gunCollider != null)
            gunCollider.enabled = false;
    }

    void Update()
    {
        if (isDead) return;

        bool aiming = Input.GetMouseButton(1); // botón derecho para apuntar

        // Mostrar u ocultar el arma visualmente
        gunRenderer.enabled = aiming;
        if (gunCollider != null)
            gunCollider.enabled = aiming;

        if (aiming)
        {
            AimGun();

            if (Input.GetMouseButtonDown(0)) // botón izquierdo para disparar
            {
                Shoot();
            }
        }
    }

    void AimGun()
    {
        if (isDead) return;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        bool isFacingLeft = mousePosition.x < transform.position.x;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.localScale = new Vector3(1, isFacingLeft ? -1 : 1, 1);
    }

    void Shoot()
    {
        if (isDead || bulletPrefab == null || firePoint == null) return;

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
    }

    public void PlayerDied()
    {
        isDead = true;
        playerSprite.flipX = false;

        // Ocultar el arma completamente si el jugador muere
        gunRenderer.enabled = false;
        if (gunCollider != null)
            gunCollider.enabled = false;
    }
}
