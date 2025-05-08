using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherButton : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public string targetLayerName = "Player";
    private int targetLayer;

    public ZombieHealth zombie;

    private bool playerIsNearby = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // Desactiva al inicio
        }
        targetLayer = LayerMask.NameToLayer(targetLayerName);
    }

    void Update()
    {
        bool shouldShow = playerIsNearby && zombie != null && zombie.GetCurrentHealth() < 30 && !zombie.isDead;

        spriteRenderer.enabled = shouldShow;

        if (shouldShow && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(zombie.Die());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == targetLayer)
        {
            playerIsNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == targetLayer)
        {
            playerIsNearby = false;
        }
    }
}
