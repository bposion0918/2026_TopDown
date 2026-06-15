using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;

    // 산소 회복량 (1%)
    public float oxygenRewardPercentage = 1f;

    // AI 스크립트를 조종하기 위한 변수
    private EnemyAI enemyAI;

    void Start()
    {
        currentHealth = maxHealth;
        enemyAI = GetComponent<EnemyAI>(); // 시작할 때 자기 몸에 있는 AI 스크립트를 찾아둠
    }

    // 몬스터가 데미지를 입을 때 실행되는 함수
    public void TakeDamage(int damage, Vector2 knockbackDir, float knockbackPower)
    {
        currentHealth -= damage;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 밀쳐내기 전에, 몬스터가 스스로 다가오던 속도를 0으로 초기화해야 깔끔하게 밀려남!
            rb.linearVelocity = Vector2.zero;

            // 플레이어가 때린 방향으로 밀어내기
            rb.AddForce(knockbackDir * knockbackPower, ForceMode2D.Impulse);
        }

        // 맞았을 때 AI를 잠깐 멈추는 기절 효과 실행
        if (enemyAI != null && gameObject.activeSelf)
        {
            StartCoroutine(KnockbackRoutine());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 0.2초 동안 몬스터의 추적 AI를 끄는 코루틴
    private IEnumerator KnockbackRoutine()
    {
        enemyAI.enabled = false;               // AI 스크립트 끄기 (물리 엔진이 마음껏 밀어낼 수 있음)
        yield return new WaitForSeconds(0.2f); // 0.2초 대기 (이 시간 동안 뒤로 쭈욱 밀려남)

        // 몬스터가 죽지 않고 살아있다면 다시 쫓아오게 AI 켜기
        if (this != null && currentHealth > 0)
        {
            enemyAI.enabled = true;
        }
    }

    private void Die()
    {
        // 1. 플레이어를 찾아서 산소를 회복시킵니다.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerOxygen playerOxygen = player.GetComponent<PlayerOxygen>();
            if (playerOxygen != null)
            {
                playerOxygen.AddOxygenByPercentage(oxygenRewardPercentage);
            }
        }

        // 2. 몬스터 오브젝트 삭제
        Destroy(gameObject);
    }
}