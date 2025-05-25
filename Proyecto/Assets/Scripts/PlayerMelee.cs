using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    public Animator anim;
    public float meleeRange = 1f;
    public int meleeDamage = 50;
    public LayerMask zombieLayer;
    public Transform attackPoint;
    public float attackCooldown = 1f;

    private float nextAttackTime = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextAttackTime)
        {
            PerformMelee();
        }
    }

    void PerformMelee()
    {
        nextAttackTime = Time.time + attackCooldown;

        if (anim != null)
            anim.SetTrigger("Melee");

        // Damage nearby zombies
        Collider2D[] hitZombies = Physics2D.OverlapCircleAll(attackPoint.position, meleeRange, zombieLayer);

        foreach (Collider2D zombie in hitZombies)
        {
            ZombieHealth health = zombie.GetComponent<ZombieHealth>();
            if (health != null)
            {
                health.TakeDamage(meleeDamage);
            }
        }
    }

    // Debug circle in scene view
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, meleeRange);
    }
}
