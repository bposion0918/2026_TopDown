using UnityEngine;
using System.Collections.Generic;

public class PlayerWeapon : MonoBehaviour
{
    [Header("무기 설정")]
    public int damage = 5;
    public float knockbackPower = 10f; // 넉백 파워(밀어내는 힘) 추가!

    private List<Collider2D> alreadyHitEnemies = new List<Collider2D>();
    private Transform playerTransform; // 플레이어의 위치를 알기 위한 변수

    private void Awake()
    {
        // 무기의 부모(플레이어) 위치를 자동으로 찾아옵니다.
        playerTransform = GetComponentInParent<PlayerController>().transform;
    }

    public void ClearHitList()
    {
        alreadyHitEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (alreadyHitEnemies.Contains(collision)) return;

            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                // 넉백 방향 계산 (몬스터 위치 - 플레이어 위치 = 플레이어에서 몬스터로 향하는 방향)
                Vector2 knockbackDir = (collision.transform.position - playerTransform.position).normalized;

                // 데미지를 줄 때 방향과 힘도 같이 넘겨줍니다!
                enemyHealth.TakeDamage(damage, knockbackDir, knockbackPower);

                alreadyHitEnemies.Add(collision);
            }
        }
    }
}