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

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log($"현재 체력: {currentHealth} / {maxHealth}");

        if (playerOxygen != null && damage < 9999)
        {
            playerOxygen.ReduceOxygenByPercentage(10f);
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
            TakeDamage(1);
        }
    }
}