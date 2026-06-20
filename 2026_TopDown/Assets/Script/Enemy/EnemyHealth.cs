using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("더미(테스트용) 모드")]
    public bool isDummy = false;

    [Header("산소 회복 및 피해량")]
    public float oxygenRewardPercentage = 1f;
    public float oxygenDamageAmount = 10f; // [추가됨] 플레이어를 때렸을 때 깎을 산소 수치

    [Header("피격 효과 및 텍스트")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;
    public GameObject damageTextPrefab;
    public Transform textSpawnPoint;

    [Header("사망 애니메이션")]
    public Sprite[] deathSprites;
    public float deathFrameTime = 0.15f;

    private EnemyAI enemyAI;
    private SpriteRenderer sr;
    private Coroutine flashCoroutine;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();

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

    public void TakeDamage(int damage, Vector2 knockbackDir, float knockbackPower, bool isFullyCharged = false)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (damageTextPrefab != null && textSpawnPoint != null)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.2f), 0);
            GameObject textObj = Instantiate(damageTextPrefab, textSpawnPoint.position + randomOffset, Quaternion.identity);

            DamageText dmgText = textObj.GetComponent<DamageText>();

            if (dmgText != null) dmgText.Setup(damage, isFullyCharged);
        }

        if (sr != null)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRedRoutine());
        }

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

        if (this != null && currentHealth > 0 && enemyAI != null && !isDead)
        {
            enemyAI.enabled = true;
        }
    }

    private void Die()
    {
        isDead = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerOxygen playerOxygen = player.GetComponent<PlayerOxygen>();
            if (playerOxygen != null)
            {
                playerOxygen.AddOxygenByPercentage(oxygenRewardPercentage);
            }
        }

        StartCoroutine(DeathAnimationRoutine());
    }

    private IEnumerator DeathAnimationRoutine()
    {
        if (enemyAI != null) enemyAI.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (deathSprites != null && deathSprites.Length > 0)
        {
            foreach (Sprite frame in deathSprites)
            {
                sr.sprite = frame;
                yield return new WaitForSeconds(deathFrameTime);
            }
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
        }

        Destroy(gameObject);
    }
}