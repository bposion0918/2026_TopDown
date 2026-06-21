using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public float attackDuration = 0.5f;
    public float fastAttackDuration = 0.2f;
    public float attackCooldown = 1.0f;
    public float weaponLingerTime = 0.3f;
    public float swingAngle = 60f;

    [Header("차지(기 모으기) 설정")]
    public float maxChargeTime = 2.5f;
    public float chargeInterval = 0.5f;
    public float damageBonusPerInterval = 0.2f;
    public float highChargeBonus = 1.5f;
    public float maxChargeBonus = 3.0f;

    private float currentChargeTime = 0f;
    private bool isCharging = false;
    private int originalBaseDamage = 5;
    private int lastLoggedLevel = 0;

    private float overchargeTimer = 0f;
    private int overchargeTicks = 0;
    private PlayerOxygen playerOxygen;
    private PlayerStats playerStats;

    [Header("게이지 UI 설정")]
    public Image chargeGaugeImage;
    public GameObject chargeGaugeBackground;
    public Color[] gaugeColors;

    [Header("게이지 효과 (흔들림 및 반짝임)")]
    public float shakeThreshold = 0.8f;
    public float shakeAmount = 2f;
    public Color flashColor = Color.white;
    public float flashDuration = 0.08f; // 한 번 깜빡일 때 체감이 잘 되도록 0.05에서 조금 늘렸어

    private RectTransform gaugeRect;
    private RectTransform bgRect;
    private Vector2 originalGaugePos;
    private Vector2 originalBgPos;

    private bool hasFlashedMax = false;
    private bool isFlashing = false;

    [Header("무기 각도 보정")]
    public float angleOffset = 90f;

    [Header("무기 오브젝트 연결")]
    public Transform weaponPivot;
    public GameObject weaponObject;

    private bool isAttacking = false;
    private bool canAttack = true;
    private PlayerController playerController;
    private Collider2D weaponCollider;

    private PlayerWeapon playerWeaponScript;
    private Vector3 originalWeaponScale;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerOxygen = GetComponent<PlayerOxygen>();
        playerStats = GetComponent<PlayerStats>();

        if (weaponObject != null)
        {
            weaponCollider = weaponObject.GetComponent<Collider2D>();
            playerWeaponScript = weaponObject.GetComponent<PlayerWeapon>();
            originalWeaponScale = weaponObject.transform.localScale;

            if (playerWeaponScript != null)
            {
                originalBaseDamage = playerWeaponScript.damage;
            }
        }

        weaponObject.SetActive(false);

        if (chargeGaugeImage != null)
        {
            chargeGaugeImage.gameObject.SetActive(false);
            gaugeRect = chargeGaugeImage.GetComponent<RectTransform>();
        }

        if (chargeGaugeBackground != null)
        {
            chargeGaugeBackground.SetActive(false);
            bgRect = chargeGaugeBackground.GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        if (isCharging)
        {
            float speedMultiplier = playerStats != null ? playerStats.currentAttackSpeed : 1f;
            float actualMaxChargeTime = maxChargeTime * speedMultiplier;
            float actualChargeInterval = chargeInterval * speedMultiplier;

            if (currentChargeTime < actualMaxChargeTime)
            {
                currentChargeTime += Time.deltaTime;

                if (currentChargeTime >= actualMaxChargeTime)
                {
                    currentChargeTime = actualMaxChargeTime;

                    if (!hasFlashedMax)
                    {
                        hasFlashedMax = true;

                        // [수정됨] 100% 도달 시 1번째 소리와 함께 1번째 하얀색 깜빡임 실행
                        if (AudioManager.instance != null) AudioManager.instance.PlayChargeReady();
                        StartCoroutine(SingleFlashRoutine(flashColor));

                        overchargeTicks = 1;
                        overchargeTimer = 0f;
                    }
                }

                int currentLevel = actualChargeInterval > 0 ? Mathf.FloorToInt(currentChargeTime / actualChargeInterval) : 0;
                if (currentLevel > lastLoggedLevel)
                {
                    lastLoggedLevel = currentLevel;
                }
            }
            else
            {
                overchargeTimer += Time.deltaTime;

                if (overchargeTimer >= 1.0f)
                {
                    overchargeTimer -= 1.0f;
                    overchargeTicks++;

                    if (overchargeTicks <= 3)
                    {
                        // [수정됨] 2번째, 3번째 소리와 함께 하얀색 깜빡임 실행
                        if (AudioManager.instance != null) AudioManager.instance.PlayChargeReady();
                        StartCoroutine(SingleFlashRoutine(flashColor));
                    }
                    else
                    {
                        // [수정됨] 패널티 경고음과 함께 '빨간색' 깜빡임 실행
                        if (AudioManager.instance != null) AudioManager.instance.PlayWarningSound();
                        if (playerOxygen != null) playerOxygen.ReduceOxygenByPercentage(5f);
                        StartCoroutine(SingleFlashRoutine(Color.black));
                    }
                }
            }

            float percentage = currentChargeTime / actualMaxChargeTime;
            int levelForUI = actualChargeInterval > 0 ? Mathf.FloorToInt(currentChargeTime / actualChargeInterval) : 0;
            UpdateGaugeUI(percentage, levelForUI);
            HandleGaugeShake(percentage);
        }
    }

    private void UpdateGaugeUI(float percentage, int level)
    {
        if (chargeGaugeImage != null)
        {
            chargeGaugeImage.fillAmount = percentage;

            if (!isFlashing && gaugeColors != null && gaugeColors.Length > 0)
            {
                int colorIndex = Mathf.Clamp(level, 0, gaugeColors.Length - 1);
                chargeGaugeImage.color = gaugeColors[colorIndex];
            }
        }
    }

    // [수정됨] 여러 번 반복하던 로직을 지우고, 원하는 색상으로 딱 한 번만 깜빡이도록 변경
    private IEnumerator SingleFlashRoutine(Color targetFlashColor)
    {
        isFlashing = true;

        int maxLevel = Mathf.FloorToInt(maxChargeTime / chargeInterval);
        Color originalColor = Color.white;

        if (gaugeColors != null && gaugeColors.Length > 0)
        {
            int colorIndex = Mathf.Clamp(maxLevel, 0, gaugeColors.Length - 1);
            originalColor = gaugeColors[colorIndex];
        }

        // 지정된 색상(하얀색 또는 빨간색)으로 번쩍!
        if (chargeGaugeImage != null) chargeGaugeImage.color = targetFlashColor;
        yield return new WaitForSeconds(flashDuration);

        // 다시 원래 게이지 색상으로 복구
        if (chargeGaugeImage != null) chargeGaugeImage.color = originalColor;

        isFlashing = false;
    }

    private void HandleGaugeShake(float percentage)
    {
        if (percentage >= shakeThreshold)
        {
            Vector2 randomOffset = Random.insideUnitCircle * shakeAmount;

            if (gaugeRect != null) gaugeRect.anchoredPosition = originalGaugePos + randomOffset;
            if (bgRect != null) bgRect.anchoredPosition = originalBgPos + randomOffset;
        }
        else
        {
            if (gaugeRect != null) gaugeRect.anchoredPosition = originalGaugePos;
            if (bgRect != null) bgRect.anchoredPosition = originalBgPos;
        }
    }

    public void OnAttack(InputValue value)
    {
        if (playerController.isDead) return;

        if (value.isPressed)
        {
            if (!isAttacking && canAttack)
            {
                isCharging = true;
                currentChargeTime = 0f;
                lastLoggedLevel = 0;

                overchargeTimer = 0f;
                overchargeTicks = 0;
                hasFlashedMax = false;
                isFlashing = false;

                if (gaugeRect != null) originalGaugePos = gaugeRect.anchoredPosition;
                if (bgRect != null) originalBgPos = bgRect.anchoredPosition;

                if (chargeGaugeImage != null) chargeGaugeImage.gameObject.SetActive(true);
                if (chargeGaugeBackground != null) chargeGaugeBackground.SetActive(true);

                UpdateGaugeUI(0f, 0);
            }
        }
        else
        {
            if (isCharging)
            {
                isCharging = false;

                if (gaugeRect != null) gaugeRect.anchoredPosition = originalGaugePos;
                if (bgRect != null) bgRect.anchoredPosition = originalBgPos;

                if (chargeGaugeImage != null) chargeGaugeImage.gameObject.SetActive(false);
                if (chargeGaugeBackground != null) chargeGaugeBackground.SetActive(false);

                StartCoroutine(SwingWeaponRoutine());
            }
        }
    }

    private IEnumerator SwingWeaponRoutine()
    {
        isAttacking = true;
        canAttack = false;

        float speedMultiplier = playerStats != null ? playerStats.currentAttackSpeed : 1f;
        float actualAttackDuration = attackDuration * speedMultiplier;
        float actualFastAttackDuration = fastAttackDuration * speedMultiplier;
        float actualMaxChargeTime = maxChargeTime * speedMultiplier;
        float actualChargeInterval = chargeInterval * speedMultiplier;
        float actualCooldown = attackCooldown * speedMultiplier;

        float currentAttackDuration = actualAttackDuration;

        if (playerWeaponScript != null)
        {
            playerWeaponScript.ClearHitList();

            int baseDmg = playerStats != null ? playerStats.currentDamage : originalBaseDamage;
            int chargeLevel = actualChargeInterval > 0 ? Mathf.FloorToInt(currentChargeTime / actualChargeInterval) : 0;

            float damageMultiplier = 1.0f + (chargeLevel * damageBonusPerInterval);
            int finalDamage = Mathf.RoundToInt(baseDmg * damageMultiplier);

            float percentage = currentChargeTime / actualMaxChargeTime;
            playerWeaponScript.isFullyCharged = false;

            if (percentage >= 1.0f)
            {
                finalDamage = Mathf.RoundToInt(finalDamage * maxChargeBonus);
                playerWeaponScript.isFullyCharged = true;
                currentAttackDuration = actualFastAttackDuration;

                if (AudioManager.instance != null) AudioManager.instance.PlayChargedAttack();
            }
            else
            {
                if (percentage >= shakeThreshold)
                {
                    finalDamage = Mathf.RoundToInt(finalDamage * highChargeBonus);
                    currentAttackDuration = actualFastAttackDuration;
                }

                if (AudioManager.instance != null) AudioManager.instance.PlayNormalAttack();
            }

            playerWeaponScript.damage = finalDamage;
        }

        if (weaponObject != null)
        {
            float rangeMultiplier = playerStats != null ? playerStats.currentRange : 1f;
            weaponObject.transform.localScale = originalWeaponScale * rangeMultiplier;
            weaponObject.SetActive(true);
        }

        if (weaponCollider != null) weaponCollider.enabled = true;

        Vector2 aimDir = playerController.lastFacingDir;
        float baseAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg + angleOffset;

        float elapsedTime = 0f;

        while (elapsedTime < currentAttackDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAngle = Mathf.Lerp(baseAngle - (swingAngle / 2f), baseAngle + (swingAngle / 2f), elapsedTime / currentAttackDuration);
            weaponPivot.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        if (weaponCollider != null) weaponCollider.enabled = false;

        yield return new WaitForSeconds(weaponLingerTime);

        weaponObject.SetActive(false);
        isAttacking = false;

        float remainingCooldown = actualCooldown - (currentAttackDuration + weaponLingerTime);
        if (remainingCooldown > 0)
        {
            yield return new WaitForSeconds(remainingCooldown);
        }

        canAttack = true;
    }
}