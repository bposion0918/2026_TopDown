using UnityEngine;
using TMPro;

public class RewardUIManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject rewardPanel;
    public TextMeshProUGUI moneyCardText; // 2번 카드의 텍스트 (반환받을 금액 표시용)

    void Start()
    {
        // 게임 시작 시, 보상 대기 상태(hasPendingReward)라면 패널을 엽니다.
        if (GameDataManager.instance != null && GameDataManager.instance.playerData.hasPendingReward)
        {
            OpenRewardPanel();
        }
        else
        {
            rewardPanel.SetActive(false);
        }
    }

    private void OpenRewardPanel()
    {
        // 게임 시간을 멈추고 선택 창을 띄웁니다.
        Time.timeScale = 0f;
        rewardPanel.SetActive(true);

        // 이전 생애 돈의 50%를 계산하여 텍스트에 표시
        int returnMoney = Mathf.RoundToInt(GameDataManager.instance.playerData.lastRunMoney * 0.5f);
        if (moneyCardText != null)
        {
            moneyCardText.text = $"돈 {returnMoney} 코인 획득";
        }
    }

    // [버튼 연결용 1] 산소 영구 50 증가 선택 시
    public void SelectOxygenBonus()
    {
        GameDataManager.instance.playerData.accumulatedOxygenBonus += 50f;
        FinishSelection();
    }

    // [버튼 연결용 2] 돈 50% 반환 획득 선택 시
    public void SelectMoneyReturn()
    {
        int returnMoney = Mathf.RoundToInt(GameDataManager.instance.playerData.lastRunMoney * 0.5f);
        GameDataManager.instance.AddMoney(returnMoney);
        FinishSelection();
    }

    private void FinishSelection()
    {
        // 보상을 획득했으므로 플래그를 내리고 데이터를 최종 저장합니다.
        GameDataManager.instance.playerData.hasPendingReward = false;
        GameDataManager.instance.playerData.lastRunMoney = 0;
        GameDataManager.instance.SaveData(GameDataManager.instance.playerData);

        // 플레이어의 산소 수치를 즉시 갱신 (영구 보너스를 선택했을 경우를 대비)
        PlayerOxygen playerOxygen = FindAnyObjectByType<PlayerOxygen>();
        if (playerOxygen != null)
        {
            playerOxygen.ApplyOxygenBonus();
        }

        // 창을 닫고 게임 시간을 정상화합니다.
        rewardPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}