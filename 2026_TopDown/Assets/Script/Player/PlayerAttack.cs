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
    private PlayerStats playerStats; // [추가됨]

    [Header("게이지 UI 설정")]
    public Image chargeGaugeImage;
    public GameObject chargeGaugeBackground;
    public Color[] gaugeColors;

    [Header("게이지 효과 (흔들림 및 반짝임)")]
    public float shakeThreshold = 0.8f;
    public float shakeAmount = 2f;
    public Color flashColor = Color.white;
    public float flashDuration = 0.05f;
    public int flashCount = 3;

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
    private Vector3 originalWeaponScale; // [추가됨] 무기 사거리 계산용

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerOxygen = GetComponent<PlayerOxygen>();
        playerStats = GetComponent<PlayerStats>();

        if (weaponObject != null)
        {
            weaponCollider = weaponObject.GetComponent<Collider2D>();
            playerWeaponScript = weaponObject.GetComponent<PlayerWeapon>();
            originalWeaponScale = weaponObject.transform.localScale; // 무기의 원래 크기 기억하기

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
            // 공격 속도 배율 가져오기 (수치가 낮을수록 빨리 차징됨)
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
                        StartCoroutine(MaxChargeFlashRoutine());

                        if (AudioManager.instance != null) AudioManager.instance.PlayChargeReady();

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
                        if (AudioManager.instance != null) AudioManager.instance.PlayChargeReady();
                    }
                    else
                    {
                        if (AudioManager.instance != null) AudioManager.instance.PlayWarningSound();
                        if (playerOxygen != null) playerOxygen.ReduceOxygenByPercentage(5f);
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

    private IEnumerator MaxChargeFlashRoutine()
    {
        isFlashing = true;

        int maxLevel = Mathf.FloorToInt(maxChargeTime / chargeInterval);
        Color originalColor = Color.white;
        if (gaugeColors != null && gaugeColors.Length > 0)
        {
            int colorIndex = Mathf.Clamp(maxLevel, 0, gaugeColors.Length - 1);
            originalColor = gaugeColors[colorIndex];
        }

        for (int i = 0; i < flashCount; i++)
        {
            if (chargeGaugeImage != null) chargeGaugeImage.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            if (chargeGaugeImage != null) chargeGaugeImage.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

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

        // 공속 스탯이 무기 휘두르는 속도, 차징 속도, 쿨타임에 전부 다르게 적용됨!
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

            // 공격력 스탯 연동
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

        // 사거리 스탯 연동: 숫자가 높으면 무기가 확 커짐!
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