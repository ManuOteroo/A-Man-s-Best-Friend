using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{

    public float distanciaActivacion = 30f;
    public float velocidad = 5f;

    private Transform zombieTransform;
    private bool activarMovimiento = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject zombie = GameObject.Find("ZombieCutscene");
        if (zombie != null)
        {
            zombieTransform = zombie.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (zombieTransform == null) return;

        float distancia = Vector2.Distance(transform.position, zombieTransform.position);

        if (distancia <= distanciaActivacion)
        {
            activarMovimiento = true;
        }

        if (activarMovimiento)
        {
            transform.Translate(Vector2.right * velocidad * Time.deltaTime);
        }
    }
}
