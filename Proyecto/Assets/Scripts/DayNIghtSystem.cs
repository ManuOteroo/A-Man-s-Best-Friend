using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    // Reference to the background object's Renderer component
    public Renderer backgroundRenderer;

    // Material to use for daytime
    public Material dayMaterial;

    // Material to use for nighttime
    public Material nightMaterial; 

    void Start()
    {
        // Randomly choose between 0 (day) and 1 (night) at the start of the game
        int timeOfDay = Random.Range(0, 2);


        // Apply the corresponding material based on the random result
        if (timeOfDay == 0)
        {
            SetDay();
        }
        else
        {
            SetNight();
        }
    }


    // Applies the day material to the background
    void SetDay()
    {
        if (backgroundRenderer != null && dayMaterial != null)
        {
            backgroundRenderer.material = dayMaterial;
            
        }
    }


    // Applies the night material to the background
    void SetNight()
    {
        if (backgroundRenderer != null && nightMaterial != null)
        {
            backgroundRenderer.material = nightMaterial;
            
        }
    }
}
