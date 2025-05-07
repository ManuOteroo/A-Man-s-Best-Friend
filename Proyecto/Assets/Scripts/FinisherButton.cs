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
        // Solo mostramos el botón si el jugador está cerca Y la vida del zombi es baja
        if (playerIsNearby && zombie != null && zombie.GetCurrentHealth() < 30 && !zombie.isDead)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
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
