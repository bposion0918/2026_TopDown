using UnityEngine;

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
    public HealthUI healthUI; // 인스펙터에서 캔버스에 있는 HealthUI를 꼭 드래그해서 넣어야 합니다!

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();

        // 시작 시 UI 업데이트를 시도합니다.
        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }
        else
        {
            // 만약 유니티 에디터에서 연결을 깜빡했다면 콘솔창에 빨간 경고를 띄워줍니다!
            Debug.LogError(" PlayerHealth 스크립트에 HealthUI가 연결되지 않았습니다! 유니티 인스펙터를 확인해주세요.");
        }
    }

    void Update()
    {
        if (isDead) return;

        // 물에 빠져있는 상태라면 타이머가 굴러갑니다.
        if (isInWater)
        {
            waterTimer += Time.deltaTime;

            // 1초가 지날 때마다
            if (waterTimer >= waterDamageInterval)
            {
                TakeDamage(1, true); // 반 칸(1) 데미지를 입히고, 물 데미지라고 알려줌
                waterTimer = 0f;     // 타이머 다시 0으로 초기화
            }
        }
    }

    // ==========================================================
    // [수정됨] 2개로 나뉘어 있던 데미지 함수를 하나로 깔끔하게 통합했습니다.
    // ==========================================================
    public void TakeDamage(int damage, bool isWaterDamage = false)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"현재 체력: {currentHealth} / {maxHealth}");

        // 1. 데미지를 입을 때마다 하트 UI를 갱신합니다.
        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        // 2. 체력이 0 이하가 되면 사망 처리
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die(isWaterDamage); // 죽을 때 무슨 원인으로 죽었는지 전달
        }
    }

    // 사망 처리 로직
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

    // 플레이어가 특정 구역에 닿았을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            isInWater = true;
            waterTimer = 1f;
        }
    }

    // 플레이어가 구역에서 빠져나왔을 때
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            isInWater = false;
            waterTimer = 0f;
        }
    }
}