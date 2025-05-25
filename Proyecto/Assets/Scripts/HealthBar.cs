using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Health targetHealth;       
    public Image healthBarImage;     

    void Update()
    {
        if (targetHealth != null && healthBarImage != null)
        {
            float fillAmount = (float)targetHealth.GetCurrentHealth() / targetHealth.maxHealth;
            healthBarImage.fillAmount = fillAmount;
        }
    }
}
