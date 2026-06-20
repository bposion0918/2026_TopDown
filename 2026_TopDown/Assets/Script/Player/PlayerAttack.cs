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

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        if (weaponObject != null)
        {
            weaponCollider = weaponObject.GetComponent<Collider2D>();
            playerWeaponScript = weaponObject.GetComponent<PlayerWeapon>();

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
            currentChargeTime += Time.deltaTime;

            if (currentChargeTime >= maxChargeTime)
            {
                currentChargeTime = maxChargeTime;

                if (!hasFlashedMax)
                {
                    hasFlashedMax = true;
                    StartCoroutine(MaxChargeFlashRoutine());

                    // [추가됨] 기 모으기 100% 완료 사운드 재생
                    if (AudioManager.instance != null)
                    {
                        AudioManager.instance.PlayChargeReady();
                    }
                }
            }

            int currentLevel = Mathf.FloorToInt(currentChargeTime / chargeInterval);
            float percentage = currentChargeTime / maxChargeTime;

            UpdateGaugeUI(percentage, currentLevel);
            HandleGaugeShake(percentage);

            if (currentLevel > lastLoggedLevel)
            {
                lastLoggedLevel = currentLevel;

                if (currentChargeTime >= maxChargeTime)
                {
                    Debug.Log($"[최대 차지 도달!] {currentLevel}단계 (데미지 +{currentLevel * 20}%)");
                }
                else
                {
                    Debug.Log($"[차지 중...] {currentLevel}단계 (데미지 +{currentLevel * 20}%)");
                }
            }
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

            if (gaugeRect != null)
                gaugeRect.anchoredPosition = originalGaugePos + randomOffset;

            if (bgRect != null)
                bgRect.anchoredPosition = originalBgPos + randomOffset;
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

                hasFlashedMax = false;
                isFlashing = false;

                if (gaugeRect != null) originalGaugePos = gaugeRect.anchoredPosition;
                if (bgRect != null) originalBgPos = bgRect.anchoredPosition;

                if (chargeGaugeImage != null) chargeGaugeImage.gameObject.SetActive(true);
                if (chargeGaugeBackground != null) chargeGaugeBackground.SetActive(true);

                UpdateGaugeUI(0f, 0);

                Debug.Log("기 모으기 시작! (0단계)");
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

        float currentAttackDuration = attackDuration;

        if (playerWeaponScript != null)
        {
            playerWeaponScript.ClearHitList();

            int chargeLevel = Mathf.FloorToInt(currentChargeTime / chargeInterval);
            float damageMultiplier = 1.0f + (chargeLevel * damageBonusPerInterval);
            int finalDamage = Mathf.RoundToInt(originalBaseDamage * damageMultiplier);

            float percentage = currentChargeTime / maxChargeTime;
            playerWeaponScript.isFullyCharged = false;

            if (percentage >= 1.0f)
            {
                finalDamage = Mathf.RoundToInt(finalDamage * maxChargeBonus);
                playerWeaponScript.isFullyCharged = true;
                currentAttackDuration = fastAttackDuration;
                Debug.Log("[풀 차지 보너스 발동!] 데미지 3배 증폭 & 공격 속도 상승!");

                // [추가됨] 풀 차지 공격 사운드 재생
                if (AudioManager.instance != null) AudioManager.instance.PlayChargedAttack();
            }
            else
            {
                if (percentage >= shakeThreshold)
                {
                    finalDamage = Mathf.RoundToInt(finalDamage * highChargeBonus);
                    currentAttackDuration = fastAttackDuration;
                    Debug.Log("[고출력 차지 보너스 발동!] 데미지 1.5배 증폭 & 공격 속도 상승!");
                }

                // [추가됨] 일반 공격 (또는 고출력 공격) 사운드 재생
                if (AudioManager.instance != null) AudioManager.instance.PlayNormalAttack();
            }

            playerWeaponScript.damage = finalDamage;
            Debug.Log($"[공격 발동!] 총 차지 시간: {currentChargeTime:F1}초 / 최종 데미지: {finalDamage}");
        }

        weaponObject.SetActive(true);
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

        float remainingCooldown = attackCooldown - (currentAttackDuration + weaponLingerTime);
        if (remainingCooldown > 0)
        {
            yield return new WaitForSeconds(remainingCooldown);
        }

        if (playerWeaponScript != null)
        {
            playerWeaponScript.damage = originalBaseDamage;
        }

        canAttack = true;
    }
}