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

    [Header("산소 감소 연출 (공기방울)")]
    public GameObject bubblePrefab;
    public int bubbleCount = 3;

    [Tooltip("방울이 나오는 기본 중심점 (Y를 낮추면 더 아래에서 나옵니다)")]
    public Vector2 bubbleSpawnOffset = new Vector2(0f, 0.2f);
    [Tooltip("중심점을 기준으로 방울이 무작위로 흩어지는 범위")]
    public Vector2 bubbleRandomArea = new Vector2(0.3f, 0.2f);

    private int playerLayer;
    private int enemyLayer;
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

        // [핵심 버그 수정 1] 게임이 시작될 때마다 레이어 충돌 무시 상태를 무조건 초기화!
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
    }

    // [핵심 버그 수정 2] R키를 눌러 씬이 강제로 재시작되거나 파괴될 때도 무적 상태를 확실히 풀어줌!
    private void OnDestroy()
    {
        int pLayer = LayerMask.NameToLayer("Player");
        int eLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(pLayer, eLayer, false);
    }

    void Update()
    {
        if (isDead) return;
    }

    public void TakeDamage(int damage, float oxygenDamage = 0f)
    {
        if (isDead) return;
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log($"현재 체력: {currentHealth} / {maxHealth}");

        if (oxygenDamage > 0f && playerOxygen != null)
        {
            playerOxygen.ReduceOxygen(oxygenDamage);
            Debug.Log($"몬스터에게 피격당해 산소가 {oxygenDamage}만큼 깎였습니다!");

            if (bubblePrefab != null)
            {
                for (int i = 0; i < bubbleCount; i++)
                {
                    float randomX = Random.Range(-bubbleRandomArea.x, bubbleRandomArea.x);
                    float randomY = Random.Range(-bubbleRandomArea.y, bubbleRandomArea.y);

                    Vector3 spawnPos = transform.position + new Vector3(bubbleSpawnOffset.x + randomX, bubbleSpawnOffset.y + randomY, 0);
                    Instantiate(bubblePrefab, spawnPos, Quaternion.identity);
                }
            }
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

            float oxDamage = (enemyHealth != null) ? enemyHealth.oxygenDamageAmount : 0f;
            TakeDamage(1, oxDamage);
        }
    }
}