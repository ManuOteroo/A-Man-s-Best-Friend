using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider healthSlider; 
    public Health playerHealth; 



    void Start()
    {
        if (healthSlider != null && playerHealth != null)
        {
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.GetCurrentHealth();
        }
    }

    void Update()
    {
        if (healthSlider != null && playerHealth != null)
        {
            healthSlider.value = playerHealth.GetCurrentHealth();
        }
    }
}