using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("몬스터 체력 설정")]
    public int maxHealth = 20;
    private int currentHealth;

    [Header("피격 효과 설정")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;

    [Header("사망 애니메이션 설정")]
    public Sprite[] deathSprites;
    public float deathFrameTime = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;
    private bool isDead = false;

    // 넉백 처리를 위해 필요한 컴포넌트들
    private Rigidbody2D rb;
    private EnemyAI enemyAI;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();
    }

    // 파라미터 추가: 넉백 방향(knockbackDir)과 힘(knockbackPower)
    public void TakeDamage(int damage, Vector2 knockbackDir = default, float knockbackPower = 0f)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name}이(가) {damage} 데미지를 입었습니다! (남은 체력: {currentHealth})");

        if (currentHealth > 0)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRedRoutine());

            // 체력이 남았다면 넉백 코루틴을 실행합니다.
            if (knockbackPower > 0)
            {
                StartCoroutine(KnockbackRoutine(knockbackDir, knockbackPower));
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 넉백을 처리하는 코루틴
    private IEnumerator KnockbackRoutine(Vector2 dir, float power)
    {
        // 1. 넉백되는 동안 플레이어를 쫓아오지 못하도록 추적 AI를 잠시 끕니다.
        if (enemyAI != null) enemyAI.enabled = false;

        // 2. 외부에서 들어오던 속도를 0으로 만들고, 물리적인 힘(Impulse)을 가해 밀어냅니다.
        // 여기서 유니티의 물리 엔진이 작용합니다! 질량(Mass)이 클수록 알아서 덜 밀려납니다.
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(dir * power, ForceMode2D.Impulse);
        }

        // 3. 0.2초 동안 밀려나도록 대기합니다. (원하는 만큼 시간 조절 가능)
        yield return new WaitForSeconds(0.2f);

        // 4. 죽지 않았다면 다시 AI를 켜서 쫓아오게 만듭니다.
        if (!isDead)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero; // 밀려나는 힘 멈춤
            if (enemyAI != null) enemyAI.enabled = true;
        }
    }

    private IEnumerator FlashRedRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white;
        }
    }

    void Die()
    {
        isDead = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (enemyAI != null) enemyAI.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        spriteRenderer.color = Color.white;
        StartCoroutine(DeathAnimationRoutine());
    }

    private IEnumerator DeathAnimationRoutine()
    {
        if (deathSprites != null && deathSprites.Length > 0)
        {
            for (int i = 0; i < deathSprites.Length; i++)
            {
                spriteRenderer.sprite = deathSprites[i];
                yield return new WaitForSeconds(deathFrameTime);
            }
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(gameObject);
    }
}