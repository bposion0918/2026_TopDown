using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    void Update()
    {
        // 데이터 매니저와 플레이어 데이터가 정상적으로 존재할 때
        if (GameDataManager.instance != null && GameDataManager.instance.playerData != null)
        {
            moneyText.text = GameDataManager.instance.playerData.money.ToString() + " : coin";
        }
        else
        {
            // 에디터에서 바로 시작해서 데이터가 아직 없을 때의 기본 화면
            moneyText.text = "0 : coin";
        }
    }
}