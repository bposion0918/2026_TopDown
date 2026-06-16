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
    public Color darkColor = new Color(0.3f, 0.3f, 1f);

    private bool isDead = false;
    private PlayerHealth playerHealth;

    private float pulseTimer = 0f;
    private Vector3 originalScale;
    private bool isPulsing = false;

    private float baseMaxOxygen;

    void Start()
    {
        baseMaxOxygen = maxOxygen;
        playerHealth = GetComponent<PlayerHealth>();

        if (oxygenFillImage != null)
        {
            oxygenFillImage.color = normalColor;
            originalScale = oxygenFillImage.transform.localScale;
        }

        ApplyOxygenBonus();
    }

    public void ApplyOxygenBonus()
    {
        if (GameDataManager.instance != null && GameDataManager.instance.playerData != null)
        {
            maxOxygen = baseMaxOxygen + GameDataManager.instance.playerData.accumulatedOxygenBonus;
        }

        currentOxygen = maxOxygen;
        UpdateOxygenUI();
    }

    void Update()
    {
        if (isDead) return;

        // 시간에 따라 depletionRate(초당 1 등 설정값)만큼 고정 수치로 닳음
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
            if (oxygenFillImage != null && !isPulsing) StartCoroutine(PulseRoutine());
        }
    }

    // [수정됨] 퍼센트가 아닌 고정 수치(amount)만큼 산소를 깎는 함수로 변경
    public void ReduceOxygen(float amount)
    {
        if (isDead) return;

        currentOxygen -= amount;

        if (currentOxygen <= 0)
        {
            currentOxygen = 0;
            DieFromLackOfOxygen();
        }
        UpdateOxygenUI();
    }

    public void AddOxygenByPercentage(float percent)
    {
        if (isDead) return;
        float addAmount = maxOxygen * (percent / 100f);
        currentOxygen += addAmount;
        if (currentOxygen > maxOxygen) currentOxygen = maxOxygen;
        UpdateOxygenUI();
    }

    public void AddOxygen(float amount)
    {
        if (isDead) return;
        currentOxygen += amount;
        if (currentOxygen > maxOxygen) currentOxygen = maxOxygen;
        UpdateOxygenUI();
    }

    private void UpdateOxygenUI()
    {
        float percentage = currentOxygen / maxOxygen;
        if (oxygenFillImage != null) oxygenFillImage.fillAmount = percentage;

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
        if (playerHealth != null) playerHealth.TakeDamage(9999);
        if (oxygenFillImage != null) oxygenFillImage.color = darkColor;
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