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
    public float invincibilityTime = 1.0f; // 무적 시간 (1초 뒤에 다시 몬스터에게 데미지를 입음)
    private bool isInvincible = false;     // 현재 플레이어가 무적 상태인지 확인하는 변수

    private int playerLayer;
    private int enemyLayer;

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (healthUI != null) healthUI.UpdateHearts(currentHealth);

        //  2. 시작할 때 Player와 Enemy의 레이어 번호를 찾아옵니다.
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

        // 몬스터에게 맞는 데미지인데, 무적 상태라면 데미지를 받지 않고 무시합니다.
        if (!isWaterDamage && isInvincible) return;

        currentHealth -= damage;
        Debug.Log($"현재 체력: {currentHealth} / {maxHealth}");

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        if (currentHealth > 0)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRedRoutine());

            // 물 데미지가 아닌 몬스터 피격이면 무적 타이머를 시작합니다.
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

    // 피격 후 일정 시간 동안 무적으로 만들어주는 코루틴 (깜빡임 효과 포함)
    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        // 핵심: 무적 상태가 시작되면 플레이어와 몬스터 레이어 간의 물리 충돌을 무시합니다! (통과됨)
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // 빨간색 피격 효과가 끝날 때까지 잠깐 대기합니다.
        yield return new WaitForSeconds(flashDuration);

        // 남은 무적 시간 동안 캐릭터를 반투명하게 깜빡거리게 만듭니다. 
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

        // 핵심: 무적이 끝나면 다시 두 레이어가 물리적으로 부딪히도록 돌려놓습니다!
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

    // 몬스터와 충돌했을 때 실행되는 물리 함수 (비비고 있으면 계속 실행됨)
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // TakeDamage 함수 안에 이미 무적(isInvincible) 체크가 있으므로, 
            // 닿아있는 동안 무조건 데미지 1을 주라고 호출해도 알아서 걸러줍니다.
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