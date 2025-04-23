using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherButton : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

   

    // Asigna aquí el nombre del layer que quieres detectar (en este caso, "Player")
    public string targetLayerName = "Player";
    private int targetLayer;

    public ZombieHealth zombie;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // Desactiva al inicio
        }
        targetLayer = LayerMask.NameToLayer(targetLayerName);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == targetLayer)
        {
            spriteRenderer.enabled = true;
            
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == targetLayer)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
        }
    }

   



}
