using UnityEngine;
using System.Collections.Generic;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int damage = 5;
    public float knockbackPower = 10f;
    public bool isFullyCharged = false; // Added: Full charge check

    private List<Collider2D> alreadyHitEnemies = new List<Collider2D>();
    private Transform playerTransform;

    private void Awake()
    {
        playerTransform = GetComponentInParent<PlayerController>().transform;
    }

    public void ClearHitList()
    {
        alreadyHitEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Enemy Hit Logic
        if (collision.CompareTag("Enemy"))
        {
            if (alreadyHitEnemies.Contains(collision)) return;

            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                Vector2 knockbackDir = (collision.transform.position - playerTransform.position).normalized;
                enemyHealth.TakeDamage(damage, knockbackDir, knockbackPower);
                alreadyHitEnemies.Add(collision);
            }
        }
        // 2. Rock Hit Logic
        else if (collision.CompareTag("Rock"))
        {
            if (alreadyHitEnemies.Contains(collision)) return;

            BreakableRock rock = collision.GetComponent<BreakableRock>();

            if (rock != null)
            {
                // Only damage the rock if the attack is fully charged
                if (isFullyCharged)
                {
                    rock.TakeDamage(1);
                    Debug.Log("Rock Damaged!");
                }
                else
                {
                    Debug.Log("Attack is not fully charged. Rock deflected the attack.");
                }

                alreadyHitEnemies.Add(collision);
            }
        }
    }
}