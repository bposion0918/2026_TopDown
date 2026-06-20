using UnityEngine;
using TMPro;

public class StatUI : MonoBehaviour
{
    [Header("플레이어 스탯 연결")]
    public PlayerStats playerStats; // 통제실 스크립트 연결

    [Header("UI 텍스트 연결")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI rangeText;

    void Update()
    {
        if (playerStats != null)
        {
            // "F1"은 소수점 첫째 자리까지만 깔끔하게 보여주라는 뜻입니다. (예: 1.563 -> 1.5)
            if (attackText != null) attackText.text = $"{playerStats.currentDamage}";
            if (moveSpeedText != null) moveSpeedText.text = $"{playerStats.currentMoveSpeed:F1}";
            if (attackSpeedText != null) attackSpeedText.text = $"{playerStats.currentAttackSpeed:F1}";
            if (rangeText != null) rangeText.text = $"{playerStats.currentRange:F1}";
        }
    }
}