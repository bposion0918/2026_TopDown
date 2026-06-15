using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOxygen : MonoBehaviour
{
    [Header("Oxygen Settings")]
    public float maxOxygen = 100f;
    public float currentOxygen;
    public float depletionRate = 1f;

    [Header("UI Settings")]
    public Image oxygenFillImage;
    public GameObject oxygenBackground;
    public TextMeshProUGUI oxygenText;

    [Header("Blink Settings")]
    public float blinkThreshold = 0.2f;
    public float blinkSpeed = 2f;
    public Color normalColor = Color.white;
    public Color darkColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    private bool isDead = false;
    private PlayerHealth playerHealth;

    private float pulseTimer = 0f;
    private Vector3 originalScale;
    private bool isPulsing = false;

    void Start()
    {
        currentOxygen = maxOxygen;
        playerHealth = GetComponent<PlayerHealth>();

        if (oxygenFillImage != null)
        {
            oxygenFillImage.color = normalColor;
            originalScale = oxygenFillImage.transform.localScale;
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
        HandleBlinking();

        pulseTimer += Time.deltaTime;
        if (pulseTimer >= 1f)
        {
            pulseTimer -= 1f;

            if (oxygenFillImage != null && !isPulsing)
            {
                StartCoroutine(PulseRoutine());
            }
        }
    }

    public void ReduceOxygenByPercentage(float percent)
    {
        if (isDead) return;

        float dropAmount = maxOxygen * (percent / 100f);
        currentOxygen -= dropAmount;

        Debug.Log($"피격 발생! 산소가 {percent}% ({dropAmount}) 깎였습니다. 남은 산소: {currentOxygen}");

        if (currentOxygen <= 0)
        {
            currentOxygen = 0;
            DieFromLackOfOxygen();
        }

        UpdateOxygenUI();
    }

    // [새로 추가된 기능] 산소 아이템을 먹었을 때 회복시켜주는 함수
    public void AddOxygenByPercentage(float percent)
    {
        if (isDead) return;

        float addAmount = maxOxygen * (percent / 100f);
        currentOxygen += addAmount;

        // 산소가 최대치를 넘지 않도록 제한
        if (currentOxygen > maxOxygen)
        {
            currentOxygen = maxOxygen;
        }

        Debug.Log($"산소 회복! {percent}% ({addAmount}) 증가. 현재 산소: {currentOxygen}");
        UpdateOxygenUI();
    }

    private void UpdateOxygenUI()
    {
        float percentage = currentOxygen / maxOxygen;

        if (oxygenFillImage != null)
        {
            oxygenFillImage.fillAmount = percentage;
        }

        if (oxygenText != null)
        {
            int displayPercent = Mathf.CeilToInt(percentage * 100f);
            displayPercent = Mathf.Clamp(displayPercent, 0, 100);
            oxygenText.text = $"O<sub>2</sub> : {displayPercent}%";
        }
    }

    private void HandleBlinking()
    {
        if (oxygenFillImage == null) return;

        float percentage = currentOxygen / maxOxygen;

        if (percentage <= blinkThreshold && currentOxygen > 0)
        {
            float lerpTime = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            oxygenFillImage.color = Color.Lerp(darkColor, normalColor, lerpTime);
        }
        else
        {
            oxygenFillImage.color = normalColor;
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

        if (oxygenFillImage != null)
        {
            oxygenFillImage.color = darkColor;
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
            Vector3 currentScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / pulseDuration);

            if (oxygenFillImage != null) oxygenFillImage.transform.localScale = currentScale;
            if (oxygenBackground != null) oxygenBackground.transform.localScale = currentScale;

            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < pulseDuration)
        {
            elapsedTime += Time.deltaTime;
            Vector3 currentScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / pulseDuration);

            if (oxygenFillImage != null) oxygenFillImage.transform.localScale = currentScale;
            if (oxygenBackground != null) oxygenBackground.transform.localScale = currentScale;

            yield return null;
        }

        if (oxygenFillImage != null) oxygenFillImage.transform.localScale = originalScale;
        if (oxygenBackground != null) oxygenBackground.transform.localScale = originalScale;

        isPulsing = false;
    }
}