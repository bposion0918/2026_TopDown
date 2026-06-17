using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("더미(테스트용) 모드")]
    public bool isDummy = false;

    [Header("산소 회복량")]
    public float oxygenRewardPercentage = 1f;

    [Header("피격 효과 및 텍스트")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;
    public GameObject damageTextPrefab;
    public Transform textSpawnPoint;

    private EnemyAI enemyAI;
    private SpriteRenderer sr;
    private Coroutine flashCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();

        // 더미가 아닐 때만 AI를 가져옵니다.
        if (!isDummy)
        {
            enemyAI = GetComponent<EnemyAI>();
        }

        if (isDummy)
        {
            maxHealth = 9999;
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDir, float knockbackPower)
    {
        currentHealth -= damage;

        // --- 1. 데미지 텍스트 띄우기 (처음처럼 약간의 랜덤 위치로 복구) ---
        if (damageTextPrefab != null && textSpawnPoint != null)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.2f), 0);
            GameObject textObj = Instantiate(damageTextPrefab, textSpawnPoint.position + randomOffset, Quaternion.identity);

            DamageText dmgText = textObj.GetComponent<DamageText>();
            if (dmgText != null) dmgText.Setup(damage);
        }

        // --- 2. 피격 시 빨간색 깜빡임 효과 ---
        if (sr != null)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRedRoutine());
        }

        // --- 3. 넉백 및 사망 처리 ---
        if (!isDummy)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(knockbackDir * knockbackPower, ForceMode2D.Impulse);
            }

            if (enemyAI != null && gameObject.activeSelf)
            {
                StartCoroutine(KnockbackRoutine());
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }
        else
        {
            if (currentHealth <= 0) currentHealth = maxHealth;
        }
    }

    private IEnumerator FlashRedRoutine()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = Color.white;
    }

    private IEnumerator KnockbackRoutine()
    {
        if (enemyAI != null) enemyAI.enabled = false;
        yield return new WaitForSeconds(0.2f);

        if (this != null && currentHealth > 0 && enemyAI != null)
        {
            enemyAI.enabled = true;
        }
    }

    private void Die()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerOxygen playerOxygen = player.GetComponent<PlayerOxygen>();
            if (playerOxygen != null)
            {
                playerOxygen.AddOxygenByPercentage(oxygenRewardPercentage);
            }
        }
        Destroy(gameObject);
    }
}