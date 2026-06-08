using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public float attackDuration = 0.5f; // 무기 휘두르는 시간
    public float attackCooldown = 1.0f; // 공격 후 다음 공격까지 걸리는 시간 (기본 1초)
    public float weaponLingerTime = 0.3f; // 👈 무기를 다 휘두른 후 이미지가 남아있는 시간
    public float swingAngle = 60f;      // 휘두르는 각도

    [Header("무기 각도 보정")]
    public float angleOffset = 90f;

    [Header("무기 오브젝트 연결")]
    public Transform weaponPivot;
    public GameObject weaponObject;

    private bool isAttacking = false;
    private bool canAttack = true;
    private PlayerController playerController;
    private Collider2D weaponCollider; // 👈 무기의 타격 판정을 제어할 변수

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        // 무기 오브젝트에 붙어있는 Collider2D를 자동으로 찾아옵니다.
        if (weaponObject != null)
        {
            weaponCollider = weaponObject.GetComponent<Collider2D>();
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

        // 1. 공격 시작: 무기 이미지와 콜라이더(타격 판정)를 모두 켭니다.
        weaponObject.SetActive(true);
        if (weaponCollider != null) weaponCollider.enabled = true;

        Vector2 aimDir = playerController.lastFacingDir;
        float baseAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg + angleOffset;

        float elapsedTime = 0f;

        // 2. 0.5초(attackDuration) 동안 무기 회전시키기
        while (elapsedTime < attackDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAngle = Mathf.Lerp(baseAngle - (swingAngle / 2f), baseAngle + (swingAngle / 2f), elapsedTime / attackDuration);
            weaponPivot.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        // 3. 휘두르기 완료: 이미지는 남겨두되, 타격 판정(Collider)만 즉시 꺼서 억울한 피격을 막습니다.
        if (weaponCollider != null) weaponCollider.enabled = false;

        // 4. 무기 이미지가 화면에 남아있는 시간(0.3초)만큼 대기합니다.
        yield return new WaitForSeconds(weaponLingerTime);

        // 5. 대기 시간이 끝나면 무기를 완전히 숨기고 공격 상태를 종료합니다.
        weaponObject.SetActive(false);
        isAttacking = false;

        // 6. 쿨타임 대기 (전체 쿨타임 1초에서 휘두른 시간(0.5)과 잔상 시간(0.3)을 뺀 나머지 시간만 대기)
        float remainingCooldown = attackCooldown - (attackDuration + weaponLingerTime);
        if (remainingCooldown > 0)
        {
            yield return new WaitForSeconds(remainingCooldown);
        }

        canAttack = true;
    }
}