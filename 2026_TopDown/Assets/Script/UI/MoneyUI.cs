using UnityEngine;
using TMPro;
using System.Collections;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    private int currentDisplayMoney = 0;
    private Vector3 originalScale;
    private bool isPulsing = false;

    void Start()
    {
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

            if (actualMoney > currentDisplayMoney)
            {
                if (!isPulsing && moneyText != null) StartCoroutine(PulseRoutine());
            }

            currentDisplayMoney = actualMoney;

            // [수정된 핵심 로직] 
            // 1. Mathf.Clamp로 숫자를 0에서 99 사이로 묶어둠
            // 2. ToString("D2")로 항상 두 자리 숫자로 출력 (예: 1 -> 01)
            int displayMoney = Mathf.Clamp(actualMoney, 0, 99);
            moneyText.text = displayMoney.ToString("D2");
        }
        else
        {
            // 데이터가 없을 때의 기본 표기도 "00"으로 변경
            moneyText.text = "00";
        }
    }

    private IEnumerator PulseRoutine()
    {
        isPulsing = true;
        Vector3 targetScale = originalScale * 1.5f;

        // 1. 순식간에 커지기 (0.1초)
        yield return StartCoroutine(ScaleOverTime(originalScale, targetScale, 0.1f));
        // 2. 다시 원래 크기로 작아지기 (0.1초)
        yield return StartCoroutine(ScaleOverTime(targetScale, originalScale, 0.1f));

        if (moneyText != null) moneyText.transform.localScale = originalScale;
        isPulsing = false;
    }

    private IEnumerator ScaleOverTime(Vector3 start, Vector3 end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (moneyText != null)
            {
                moneyText.transform.localScale = Vector3.Lerp(start, end, elapsedTime / duration);
            }
            yield return null;
        }
    }
}