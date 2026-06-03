using UnityEngine;
using System.Collections; //  코루틴(IEnumerator)을 쓰기 위해 반드시 필요합니다!

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 6;       // 총 체력 (하트 3개, 반 칸 = 1)
    public int currentHealth;

    [Header("물 데미지 설정")]
    public float waterDamageInterval = 1f; // 1초마다 데미지
    private float waterTimer = 0f;
    private bool isInWater = false;

    private bool isDead = false;

    private PlayerController playerController;
    public HealthUI healthUI;

    // ---- [새로 추가된 피격 효과 변수들] ----
    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;
    [Header("피격 효과 설정")]
    public Color damageColor = Color.red;    // 피격 시 바뀔 색상
    public float flashDuration = 0.1f;       // 깜빡이는 시간
    // ------------------------------------

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();

        // 플레이어의 SpriteRenderer 컴포넌트를 가져옵니다.
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }
        else
        {
            Debug.LogError(" PlayerHealth 스크립트에 HealthUI가 연결되지 않았습니다!");
        }
    }

    void Update()
    {
        if (isDead) return;

        if (isInWater)
        {
            waterTimer += Time.deltaTime;

            if (waterTimer >= waterDamageInterval)
            {
                TakeDamage(1, true);
                waterTimer = 0f;
            }
        }
    }

    public void TakeDamage(int damage, bool isWaterDamage = false)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"현재 체력: {currentHealth} / {maxHealth}");

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        // ---- [새로 추가된 피격 효과 트리거] ----
        // 죽지 않고 살아있을 때만 빨간색으로 깜빡입니다.
        if (currentHealth > 0)
        {
            // 이미 깜빡이는 중이었다면 이전 효과를 종료하고 새로 시작합니다 (연속 피격 대비)
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRedRoutine());

            //  [여기에 추가!] 데미지를 입었을 때 오디오 매니저를 불러와 피격음 재생
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayHitSFX();
            }
        }
        // -------------------------------------

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die(isWaterDamage);
        }
    }

    // ---- [새로 추가된 피격 효과 코루틴] ----
    private IEnumerator FlashRedRoutine()
    {
        // 1. 플레이어 캐릭터 색상을 빨간색으로 변경
        spriteRenderer.color = damageColor;

        // 2. 지정한 시간(0.1초)만큼 대기
        yield return new WaitForSeconds(flashDuration);

        // 3. 다시 원래 색상(정상 상태인 흰색)으로 복귀
        spriteRenderer.color = Color.white;
    }
    // ------------------------------------

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            isInWater = true;
            waterTimer = 1f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            isInWater = false;
            waterTimer = 0f;
        }
    }

}