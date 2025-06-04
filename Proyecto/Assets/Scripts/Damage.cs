using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DamageScript : MonoBehaviour
{

    // Amount of damage object inflicts on the player
    public int damageAmount = 100;




    private void OnTriggerEnter2D(Collider2D other)
    {


        if (other.CompareTag("Player"))
        {

            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {


                playerHealth.TakeDamage(damageAmount);
            }
        }

    }


}
 