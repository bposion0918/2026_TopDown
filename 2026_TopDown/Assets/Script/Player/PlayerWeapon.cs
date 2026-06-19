using UnityEngine;
using System.Collections.Generic;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int damage = 5;
    public float knockbackPower = 10f;
    public bool isFullyCharged = false;

    [Header("Camera Shake Settings")]
    public float enemyShakeDuration = 0.2f;
    public float enemyShakeMagnitude = 0.1f; // 0.3에서 0.1로 대폭 감소
    public float rockShakeDuration = 0.15f;
    public float rockShakeMagnitude = 0.08f; // 0.25에서 0.08로 감소

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
        // 1. 적(Enemy) 타격 판정
        if (collision.CompareTag("Enemy"))
        {
            if (alreadyHitEnemies.Contains(collision)) return;

            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                Vector2 knockbackDir = (collision.transform.position - playerTransform.position).normalized;

                enemyHealth.TakeDamage(damage, knockbackDir, knockbackPower, isFullyCharged);

                // 몬스터 타격 시 카메라 흔들림 (인스펙터 변수 사용)
                if (isFullyCharged && CameraController.instance != null)
                {
                    CameraController.instance.TriggerShake(enemyShakeDuration, enemyShakeMagnitude);
                }

                alreadyHitEnemies.Add(collision);
            }
        }
        // 2. 바위(Rock) 타격 판정
        else if (collision.CompareTag("Rock"))
        {
            if (alreadyHitEnemies.Contains(collision)) return;

            BreakableRock rock = collision.GetComponent<BreakableRock>();

            if (rock != null)
            {
                if (isFullyCharged)
                {
                    rock.TakeDamage(1);
                    Debug.Log("Rock Damaged!");

                    // 바위 파괴 시 카메라 흔들림 (인스펙터 변수 사용)
                    if (CameraController.instance != null)
                    {
                        CameraController.instance.TriggerShake(rockShakeDuration, rockShakeMagnitude);
                    }
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