using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 6;
    public int currentHealth;

    private bool isDead = false;

    private PlayerController playerController;
    public HealthUI healthUI;

    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;

    [Header("피격 효과 및 무적 설정")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;
    public float invincibilityTime = 1.0f;
    private bool isInvincible = false;

    private int playerLayer;
    private int enemyLayer;

    // 산소를 깎아야 하므로 PlayerOxygen 컴포넌트를 연결해 둡니다.
    private PlayerOxygen playerOxygen;

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerOxygen = GetComponent<PlayerOxygen>();

        if (healthUI != null) healthUI.UpdateHearts(currentHealth);

        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    void Update()
    {
        if (isDead) return;
    }

    // [수정됨] 산소 피해량(oxygenDamage)을 추가로 받을 수 있게 만들었습니다. 기본값은 0입니다.
    public void TakeDamage(int damage, float oxygenDamage = 0f)
    {
        if (isDead) return;
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log($"현재 체력: {currentHealth} / {maxHealth}");

        // --- 피격 시 산소가 닳는 로직을 복구했습니다 ---
        if (oxygenDamage > 0f && playerOxygen != null)
        {
            playerOxygen.ReduceOxygen(oxygenDamage);
            Debug.Log($"몬스터에게 피격당해 산소가 {oxygenDamage}만큼 깎였습니다!");
        }

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        if (currentHealth > 0)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRedRoutine());

            StartCoroutine(InvincibilityRoutine());

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayHitSFX();
            }
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private IEnumerator FlashRedRoutine()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        yield return new WaitForSeconds(flashDuration);

        float blinkTime = invincibilityTime - flashDuration;
        float elapsed = 0f;
        while (elapsed < blinkTime)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }

        spriteRenderer.color = Color.white;
        isInvincible = false;

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
    }

    private void Die()
    {
        isDead = true;

        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        Debug.Log("체력이 다 닳음 -> 일반 사망 애니메이션");
        playerController.PlayNormalDeathAnimation();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

            if (enemyHealth != null && enemyHealth.isDummy)
            {
                return;
            }

            // [수정됨] 몬스터가 가진 고유의 산소 피해량을 가져와서 적용합니다.
            float oxDamage = (enemyHealth != null) ? enemyHealth.oxygenDamageAmount : 0f;
            TakeDamage(1, oxDamage);
        }
    }
}