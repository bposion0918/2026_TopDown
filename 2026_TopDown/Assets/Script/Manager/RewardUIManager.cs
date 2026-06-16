using System.Collections;
using UnityEngine;
using TMPro;

public class RewardUIManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject rewardPanel;
    public TextMeshProUGUI moneyCardText;

    // 투명도 조절을 위한 CanvasGroup
    private CanvasGroup panelCanvasGroup;

    [Header("애니메이션 설정")]
    public float animDuration = 0.3f; // 스르륵 나타나는 시간 (0.3초)

    void Start()
    {
        if (GameDataManager.instance != null && GameDataManager.instance.playerData.hasPendingReward)
        {
            // 바로 여는 대신 나타나는 애니메이션 코루틴을 실행합니다.
            StartCoroutine(OpenPanelRoutine());
        }
        else
        {
            rewardPanel.SetActive(false);
        }
    }

    private IEnumerator OpenPanelRoutine()
    {
        // 1. 게임 시간 정지 및 패널 활성화
        Time.timeScale = 0f;
        rewardPanel.SetActive(true);

        // CanvasGroup 컴포넌트가 없다면 자동으로 추가합니다.
        panelCanvasGroup = rewardPanel.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = rewardPanel.AddComponent<CanvasGroup>();
        }

        // 2. 초기 상태 설정 (투명도 0, 크기 0.8배)
        panelCanvasGroup.alpha = 0f;
        rewardPanel.transform.localScale = Vector3.one * 0.8f;

        // 돈 반환 수치 계산 및 텍스트 적용
        int returnMoney = Mathf.RoundToInt(GameDataManager.instance.playerData.lastRunMoney * 0.5f);
        if (moneyCardText != null)
        {
            moneyCardText.text = $"Get {returnMoney} Coins";
        }

        // 3. 스르륵 나타나는 애니메이션 진행
        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            // timeScale이 0이므로 unscaledDeltaTime을 사용해야 애니메이션이 재생됩니다.
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animDuration;

            // 투명도와 크기를 부드럽게 증가시킵니다.
            panelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            rewardPanel.transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, t);

            yield return null;
        }

        // 최종 오차 보정
        panelCanvasGroup.alpha = 1f;
        rewardPanel.transform.localScale = Vector3.one;
    }

    public void SelectOxygenBonus()
    {
        GameDataManager.instance.playerData.accumulatedOxygenBonus += 50f;
        StartCoroutine(ClosePanelRoutine()); // 선택 시 닫히는 애니메이션 실행
    }

    public void SelectMoneyReturn()
    {
        int returnMoney = Mathf.RoundToInt(GameDataManager.instance.playerData.lastRunMoney * 0.5f);
        GameDataManager.instance.AddMoney(returnMoney);
        StartCoroutine(ClosePanelRoutine()); // 선택 시 닫히는 애니메이션 실행
    }

    private IEnumerator ClosePanelRoutine()
    {
        // 선택을 완료하고 창이 닫힐 때도 스르륵 사라지는 연출을 적용합니다.
        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animDuration;

            panelCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            rewardPanel.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, t);

            yield return null;
        }

        FinishSelection();
    }

    private void FinishSelection()
    {
        GameDataManager.instance.playerData.hasPendingReward = false;
        GameDataManager.instance.playerData.lastRunMoney = 0;
        GameDataManager.instance.SaveData(GameDataManager.instance.playerData);

        PlayerOxygen playerOxygen = FindAnyObjectByType<PlayerOxygen>();
        if (playerOxygen != null)
        {
            playerOxygen.ApplyOxygenBonus();
        }

        rewardPanel.SetActive(false);
        Time.timeScale = 1f; // 게임 시간 정상화
    }
}