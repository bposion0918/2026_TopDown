using UnityEngine;

public class CoinItem : MonoBehaviour
{
    public int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 어떤 오브젝트가 코인에 닿았는지 확인합니다.
        Debug.Log($"[{gameObject.name}]에 '{collision.gameObject.name}'이(가) 닿았습니다!");

        if (collision.CompareTag("Player"))
        {
            if (GameDataManager.instance != null)
            {
                GameDataManager.instance.AddMoney(coinValue);
                Debug.Log($"돈 획득 성공! (현재 돈: {GameDataManager.instance.playerData.money})");
            }
            else
            {
                Debug.LogError("GameDataManager가 씬에 존재하지 않습니다!");
            }

            Destroy(gameObject);
        }
    }
}