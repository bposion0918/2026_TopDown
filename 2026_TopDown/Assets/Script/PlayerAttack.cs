using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public float attackDuration = 0.5f;
    public float attackCooldown = 1.0f;
    public float weaponLingerTime = 0.3f;
    public float swingAngle = 60f;

    [Header("차지(기 모으기) 설정")]
    public float maxChargeTime = 5.0f;
    public float chargeInterval = 1.0f;
    public float damageBonusPerInterval = 0.2f;

    // 🌟 새로 추가: 끝까지 모았을 때만 곱해질 폭발적인 보너스 배율
    public float maxChargeBonus = 3.0f;

    private float currentChargeTime = 0f;
    private bool isCharging = false;
    private int originalBaseDamage = 5;
    private int lastLoggedLevel = 0;

    [Header("게이지 UI 설정")]
    public SpriteRenderer chargeGaugeRenderer; // 👈 게이지를 그려줄 화가(컴포넌트)
    public Sprite[] chargeSprites;             // 👈 0%, 20%, 40%, 60%, 80%, 100% 이미지 6장

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

        // 🌟 게임이 시작될 때는 게이지를 숨겨둡니다.
        if (chargeGaugeRenderer != null)
        {
            chargeGaugeRenderer.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;

            if (currentChargeTime > maxChargeTime)
            {
                currentChargeTime = maxChargeTime;
            }

            int currentLevel = Mathf.FloorToInt(currentChargeTime / chargeInterval);

            // 🌟 1. 매 프레임마다 현재 단계에 맞는 이미지로 교체합니다.
            UpdateGaugeUI(currentLevel);

            if (currentLevel > lastLoggedLevel)
            {
                lastLoggedLevel = currentLevel;

                if (currentChargeTime >= maxChargeTime)
                {
                    Debug.Log($"🔥 [최대 차지 도달!] {currentLevel}단계 (데미지 +{currentLevel * 20}%)");
                }
                else
                {
                    Debug.Log($"⚡ [차지 중...] {currentLevel}단계 (데미지 +{currentLevel * 20}%)");
                }
            }
        }
    }

    // 🌟 2. 단계(0~5)에 맞춰 준비된 6장의 이미지 중 하나를 꺼내오는 함수
    private void UpdateGaugeUI(int level)
    {
        if (chargeGaugeRenderer != null && chargeSprites != null && chargeSprites.Length > 0)
        {
            // 혹시라도 레벨이 배열 크기를 넘어가서 에러가 나지 않도록 안전장치를 겁니다.
            int spriteIndex = Mathf.Clamp(level, 0, chargeSprites.Length - 1);
            chargeGaugeRenderer.sprite = chargeSprites[spriteIndex];
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

                // 🌟 3. 마우스를 누르기 시작하면 게이지를 화면에 띄우고 빈칸(0번) 이미지를 넣습니다.
                if (chargeGaugeRenderer != null) chargeGaugeRenderer.gameObject.SetActive(true);
                UpdateGaugeUI(0);

                Debug.Log("기 모으기 시작! (0단계)");
            }
        }
        else
        {
            if (isCharging)
            {
                isCharging = false;

                // 🌟 4. 마우스를 떼고 공격이 나가는 순간 게이지를 다시 숨깁니다.
                if (chargeGaugeRenderer != null) chargeGaugeRenderer.gameObject.SetActive(false);

                StartCoroutine(SwingWeaponRoutine());
            }
        }
    }

    private IEnumerator SwingWeaponRoutine()
    {
        isAttacking = true;
        canAttack = false;

        if (playerWeaponScript != null)
        {
            playerWeaponScript.ClearHitList();

            int chargeLevel = Mathf.FloorToInt(currentChargeTime / chargeInterval);
            float damageMultiplier = 1.0f + (chargeLevel * damageBonusPerInterval);
            int finalDamage = Mathf.RoundToInt(originalBaseDamage * damageMultiplier);

            // 무기 초기화: 기본적으로는 풀차지가 아니라고 설정
            playerWeaponScript.isFullyCharged = false;

            if (currentChargeTime >= maxChargeTime)
            {
                finalDamage = Mathf.RoundToInt(finalDamage * maxChargeBonus);

                // 풀차지를 달성했다면 무기에게 상태를 전달!
                playerWeaponScript.isFullyCharged = true;

                Debug.Log("🔥 [풀 차지 보너스 발동!] 데미지 3배 증폭!");
            }

            playerWeaponScript.damage = finalDamage;

            Debug.Log($"💥 [공격 발동!] 총 차지 시간: {currentChargeTime:F1}초 / 최종 데미지: {finalDamage}");
        }

        weaponObject.SetActive(true);
        if (weaponCollider != null) weaponCollider.enabled = true;

        Vector2 aimDir = playerController.lastFacingDir;
        float baseAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg + angleOffset;

        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAngle = Mathf.Lerp(baseAngle - (swingAngle / 2f), baseAngle + (swingAngle / 2f), elapsedTime / attackDuration);
            weaponPivot.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        if (weaponCollider != null) weaponCollider.enabled = false;

        yield return new WaitForSeconds(weaponLingerTime);

        weaponObject.SetActive(false);
        isAttacking = false;

        float remainingCooldown = attackCooldown - (attackDuration + weaponLingerTime);
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