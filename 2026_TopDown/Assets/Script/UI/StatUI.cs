using UnityEngine;
using TMPro;

public class StatUI : MonoBehaviour
{
    [Header("플레이어 정보 연결")]
    public PlayerStats playerStats; // 통제실 스크립트
    public PlayerAttack playerAttack; // [추가됨] 최대 차지 데미지 계산용 스크립트

    [Header("UI 텍스트 연결")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI maxDamageText; // [추가됨] 최대 데미지 텍스트
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI rangeText;

    void Update()
    {
        if (playerStats != null)
        {
            // 1. 기본 공격력 표기
            if (attackText != null) attackText.text = $"{playerStats.currentDamage}";

            // 2. 최대 차지 데미지 계산 및 표기
            if (maxDamageText != null && playerAttack != null)
            {
                // PlayerAttack에 설정된 수치들을 가져와서 똑같이 계산해 줌
                int maxChargeLevel = Mathf.FloorToInt(playerAttack.maxChargeTime / playerAttack.chargeInterval);
                float damageMultiplier = 1.0f + (maxChargeLevel * playerAttack.damageBonusPerInterval);

                // 1차 계산 (기본 데미지 * 차지 단계별 배율)
                int tempDamage = Mathf.RoundToInt(playerStats.currentDamage * damageMultiplier);

                // 2차 계산 (풀차지 시 3배 같은 특수 보너스 곱하기)
                int maxDamage = Mathf.RoundToInt(tempDamage * playerAttack.maxChargeBonus);

                maxDamageText.text = $"{maxDamage}";
            }

            // 3. 나머지 스탯 표기
            if (moveSpeedText != null) moveSpeedText.text = $"{playerStats.currentMoveSpeed:F1}";
            if (attackSpeedText != null) attackSpeedText.text = $"{playerStats.currentAttackSpeed:F1}";
            if (rangeText != null) rangeText.text = $"{playerStats.currentRange:F1}";
        }
    }
}