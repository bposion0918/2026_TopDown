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

    [Header("무기 각도 보정")]
    public float angleOffset = 90f;

    [Header("무기 오브젝트 연결")]
    public Transform weaponPivot;
    public GameObject weaponObject;

    private bool isAttacking = false;
    private bool canAttack = true;
    private PlayerController playerController;
    private Collider2D weaponCollider;

    //  새로 추가: PlayerWeapon 스크립트를 조종하기 위한 변수
    private PlayerWeapon playerWeaponScript;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        if (weaponObject != null)
        {
            weaponCollider = weaponObject.GetComponent<Collider2D>();
            //  무기 오브젝트에 붙어있는 PlayerWeapon 스크립트도 찾아옵니다.
            playerWeaponScript = weaponObject.GetComponent<PlayerWeapon>();
        }

        weaponObject.SetActive(false);
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed && !isAttacking && canAttack && !playerController.isDead)
        {
            StartCoroutine(SwingWeaponRoutine());
        }
    }

    private IEnumerator SwingWeaponRoutine()
    {
        isAttacking = true;
        canAttack = false;

        //  공격 시작 시 가장 먼저 할 일: 무기의 타격 명부를 싹 비워줍니다!
        if (playerWeaponScript != null)
        {
            playerWeaponScript.ClearHitList();
        }

        // 1. 무기 이미지와 콜라이더(타격 판정) 켜기
        weaponObject.SetActive(true);
        if (weaponCollider != null) weaponCollider.enabled = true;

        Vector2 aimDir = playerController.lastFacingDir;
        float baseAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg + angleOffset;

        float elapsedTime = 0f;

        // 2. 무기 회전시키기
        while (elapsedTime < attackDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAngle = Mathf.Lerp(baseAngle - (swingAngle / 2f), baseAngle + (swingAngle / 2f), elapsedTime / attackDuration);
            weaponPivot.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        // 3. 타격 판정(Collider) 끄기
        if (weaponCollider != null) weaponCollider.enabled = false;

        // 4. 무기 잔상 대기
        yield return new WaitForSeconds(weaponLingerTime);

        // 5. 무기 숨기기
        weaponObject.SetActive(false);
        isAttacking = false;

        // 6. 쿨타임 계산 및 대기
        float remainingCooldown = attackCooldown - (attackDuration + weaponLingerTime);
        if (remainingCooldown > 0)
        {
            yield return new WaitForSeconds(remainingCooldown);
        }

        canAttack = true;
    }
}