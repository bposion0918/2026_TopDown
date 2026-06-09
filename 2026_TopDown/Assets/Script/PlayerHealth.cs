using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 6;
    public int currentHealth;

    [Header("물 데미지 설정")]
    public float waterDamageInterval = 1f;

    private float nextWaterDamageTime = 0f;
    private bool isInWater = false;
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

    // 산소 스크립트를 조종하기 위한 변수 추가
    private PlayerOxygen playerOxygen;

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 내 몸(Player)에 붙어있는 PlayerOxygen 컴포넌트를 찾아서 연결해줍니다.
        playerOxygen = GetComponent<PlayerOxygen>();

        if (healthUI != null) healthUI.UpdateHearts(currentHealth);

        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    void Update()
    {
        if (isDead) return;

        if (isInWater && Time.time >= nextWaterDamageTime)
        {
            TakeDamage(1, true);
            nextWaterDamageTime = Time.time + waterDamageInterval;
        }
    }

    public void TakeDamage(int damage, bool isWaterDamage = false)
    {
        if (isDead) return;

        if (!isWaterDamage && isInvincible) return;

        currentHealth -= damage;
        Debug.Log($"현재 체력: {currentHealth} / {maxHealth}");

        // 데미지를 입었을 때 산소도 10% 같이 깎아줍니다.
        // (단, 산소 부족으로 인한 9999 즉사 데미지일 때는 실행하지 않습니다)
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

            if (!isWaterDamage)
            {
                StartCoroutine(InvincibilityRoutine());
            }

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayHitSFX();
            }
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die(isWaterDamage);
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

    private void Die(bool isWaterDeath)
    {
        isDead = true;

        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (isWaterDeath)
        {
            Debug.Log("물에서 체력이 다 닳음 -> 물 사망 애니메이션");
            playerController.PlayWaterDeathAnimation();
        }
        else
        {
            Debug.Log("일반 공격으로 체력이 다 닳음 -> 일반 사망 애니메이션");
            playerController.PlayNormalDeathAnimation();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            isInWater = true;
            playerController.SetInWaterState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            isInWater = false;
            playerController.SetInWaterState(false);
        }
    }
}