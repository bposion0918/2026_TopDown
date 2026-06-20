using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("기본 스탯 (아무 템도 없을 때)")]
    public int baseDamage = 5;
    public float baseMoveSpeed = 1f;
    public float baseAttackSpeed = 1.0f; // 쿨타임 배율 (낮을수록 빠름)
    public float baseRange = 1.0f; // 무기 크기 배율

    [Header("현재 스탯 (아이템 적용 후)")]
    public int currentDamage;
    public float currentMoveSpeed;
    public float currentAttackSpeed;
    public float currentRange;

    void Start()
    {
        // 게임을 시작할 때 기본 스탯을 현재 스탯으로 복사해 줍니다.
        UpdateStats();
    }

    // 나중에 아이템을 먹거나 스탯이 바뀔 때마다 호출할 함수입니다.
    public void UpdateStats()
    {
        currentDamage = baseDamage;
        currentMoveSpeed = baseMoveSpeed;
        currentAttackSpeed = baseAttackSpeed;
        currentRange = baseRange;

        // 나중에 이 부분에 "만약 공격력 아이템을 먹었다면 currentDamage += 2;" 같은 로직들이 들어갑니다!
    }
}