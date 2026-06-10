using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerOxygen : MonoBehaviour
{
    [Header("Oxygen Settings")]
    public float maxOxygen = 100f;
    public float currentOxygen;
    public float depletionRate = 1f;

    [Header("UI Settings")]
    public Image oxygenUIImage;
    public Sprite[] oxygenSprites;

    private bool isDead = false;
    private PlayerHealth playerHealth;

    private float pulseTimer = 0f;
    private Vector3 originalScale;
    private bool isPulsing = false;

    void Start()
    {
        currentOxygen = maxOxygen;
        playerHealth = GetComponent<PlayerHealth>();

        if (oxygenUIImage != null)
        {
            originalScale = oxygenUIImage.transform.localScale;
        }
    }

    void Update()
    {
        if (isDead) return;

        currentOxygen -= depletionRate * Time.deltaTime;

        if (currentOxygen <= 0)
        {
            currentOxygen = 0;
            DieFromLackOfOxygen();
        }

        UpdateOxygenUI();

        pulseTimer += Time.deltaTime;
        if (pulseTimer >= 1f)
        {
            pulseTimer -= 1f;

            if (oxygenUIImage != null && !isPulsing)
            {
                StartCoroutine(PulseRoutine());
            }
        }
    }

    // 외부(PlayerHealth)에서 산소를 퍼센트 단위로 깎을 때 부르는 함수
    public void ReduceOxygenByPercentage(float percent)
    {
        if (isDead) return;

        // 최대 산소량 기준으로 퍼센트만큼 깎을 양을 계산합니다 (예: 100의 10% = 10)
        float dropAmount = maxOxygen * (percent / 100f);
        currentOxygen -= dropAmount;

        Debug.Log($"피격 발생! 산소가 {percent}% ({dropAmount}) 깎였습니다. 남은 산소: {currentOxygen}");

        if (currentOxygen <= 0)
        {
            currentOxygen = 0;
            DieFromLackOfOxygen();
        }

        UpdateOxygenUI(); // 깎인 직후 UI를 즉시 업데이트
    }

    private void UpdateOxygenUI()
    {
        if (oxygenUIImage != null && oxygenSprites != null && oxygenSprites.Length == 11)
        {
            float percentage = currentOxygen / maxOxygen;
            int spriteIndex = Mathf.CeilToInt(percentage * 10f);
            spriteIndex = Mathf.Clamp(spriteIndex, 0, 10);
            oxygenUIImage.sprite = oxygenSprites[spriteIndex];
        }
    }

    private void DieFromLackOfOxygen()
    {
        isDead = true;
        Debug.Log("Out of Oxygen!");

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(9999);
        }
    }

    private IEnumerator PulseRoutine()
    {
        isPulsing = true;

        float pulseDuration = 0.15f;
        float elapsedTime = 0f;
        Vector3 targetScale = originalScale * 1.2f;

        while (elapsedTime < pulseDuration)
        {
            elapsedTime += Time.deltaTime;
            oxygenUIImage.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / pulseDuration);
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < pulseDuration)
        {
            elapsedTime += Time.deltaTime;
            oxygenUIImage.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / pulseDuration);
            yield return null;
        }

        oxygenUIImage.transform.localScale = originalScale;

        isPulsing = false;
    }
}