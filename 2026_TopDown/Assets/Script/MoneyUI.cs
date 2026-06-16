using UnityEngine;
using TMPro;
using System.Collections; // 코루틴을 사용하기 위해 필수 추가

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    private int currentDisplayMoney = 0; // 화면에 표시 중인 이전 돈 액수 기억
    private Vector3 originalScale;       // 텍스트의 원래 크기 기억
    private bool isPulsing = false;      // 현재 커졌다 작아지는 애니메이션 중인지 확인

    void Start()
    {
        // 게임 시작 시 텍스트의 원래 크기를 저장해 둡니다.
        if (moneyText != null)
        {
            originalScale = moneyText.transform.localScale;
        }
    }

    void Update()
    {
        if (GameDataManager.instance != null && GameDataManager.instance.playerData != null)
        {
            int actualMoney = GameDataManager.instance.playerData.money;

            // 실제 돈이 화면에 표시 중이던 돈보다 많아졌다면? (코인을 먹었다면!)
            if (actualMoney > currentDisplayMoney)
            {
                // 팝(Pop) 애니메이션 실행
                if (!isPulsing && moneyText != null)
                {
                    StartCoroutine(PulseRoutine());
                }
            }

            // 현재 기억하는 돈 액수를 업데이트하고 텍스트에 적용합니다.
            currentDisplayMoney = actualMoney;
            moneyText.text = " : " + actualMoney.ToString() + " Coins ";
        }
        else
        {
            // 에디터 테스트 시 데이터가 없을 때 기본값
            moneyText.text = "0";
        }
    }

    // 텍스트가 순식간에 커졌다 작아지는 코루틴 애니메이션
    private IEnumerator PulseRoutine()
    {
        isPulsing = true;

        float pulseDuration = 0.1f; // 커지는 데 걸리는 시간 (아주 짧게 0.1초)
        float elapsedTime = 0f;
        Vector3 targetScale = originalScale * 1.5f; // 텍스트가 1.5배까지 커집니다. (원하시면 수치 조절)

        // 1. 순식간에 커지기
        while (elapsedTime < pulseDuration)
        {
            elapsedTime += Time.deltaTime;
            if (moneyText != null)
            {
                moneyText.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / pulseDuration);
            }
            yield return null;
        }

        // 2. 다시 원래 크기로 작아지기
        elapsedTime = 0f;
        while (elapsedTime < pulseDuration)
        {
            elapsedTime += Time.deltaTime;
            if (moneyText != null)
            {
                moneyText.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / pulseDuration);
            }
            yield return null;
        }

        // 혹시 모를 오차를 방지하기 위해 원래 크기로 완벽히 되돌림
        if (moneyText != null)
        {
            moneyText.transform.localScale = originalScale;
        }

        isPulsing = false;
    }
}