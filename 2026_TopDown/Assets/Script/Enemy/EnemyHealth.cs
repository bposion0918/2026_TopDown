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

    [Header("사망 애니메이션")]
    public Sprite[] deathSprites;
    public float deathFrameTime = 0.15f;

    private EnemyAI enemyAI;
    private SpriteRenderer sr;
    private Coroutine flashCoroutine;
    private bool isDead = false; // 이미 죽었는지 체크하는 변수 추가

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

    public void TakeDamage(int damage, Vector2 knockbackDir, float knockbackPower, bool isFullyCharged = false)
    {
        // 이미 죽은 상태면 더 이상 데미지를 입지 않도록 차단
        if (isDead) return;

        currentHealth -= damage;

        // --- 1. 데미지 텍스트 띄우기 ---
        if (damageTextPrefab != null && textSpawnPoint != null)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.2f), 0);
            GameObject textObj = Instantiate(damageTextPrefab, textSpawnPoint.position + randomOffset, Quaternion.identity);

            DamageText dmgText = textObj.GetComponent<DamageText>();

            if (dmgText != null) dmgText.Setup(damage, isFullyCharged);
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

        // 죽지 않았을 때만 다시 AI를 켭니다
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

        // 즉시 삭제하는 대신 사망 애니메이션 코루틴을 실행합니다.
        StartCoroutine(DeathAnimationRoutine());
    }

    private IEnumerator DeathAnimationRoutine()
    {
        // 1. 죽었을 때 플레이어를 계속 쫓아오거나 부딪히지 않도록 기능을 전부 끕니다.
        if (enemyAI != null) enemyAI.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 2. 등록된 사망 스프라이트가 있다면 순서대로 착착착 재생합니다.
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
            // 만약 스프라이트를 안 넣었다면 0.2초 대기 후 사라집니다.
            yield return new WaitForSeconds(0.2f);
        }

        // 3. 애니메이션이 모두 끝난 뒤에 비로소 몬스터를 삭제합니다.
        Destroy(gameObject);
    }
}