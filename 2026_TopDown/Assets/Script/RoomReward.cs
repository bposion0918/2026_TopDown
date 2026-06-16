using UnityEngine;

public class RoomReward : MonoBehaviour
{
    [Header("보상 설정")]
    public GameObject coinPrefab;
    public GameObject oxygenPrefab;   // 새로 추가된 산소 프리팹 칸
    public Transform mapCenterPoint;

    public void DropReward()
    {
        // 0부터 99까지 난수 생성 (총 100개 확률)
        int randomChance = Random.Range(0, 100);

        // 1. 33% 확률로 돈 드롭 (0 ~ 32)
        if (randomChance < 33)
        {
            if (coinPrefab != null && mapCenterPoint != null)
            {
                Instantiate(coinPrefab, mapCenterPoint.position, Quaternion.identity);
                Debug.Log("방 클리어: 코인 당첨! (33%)");
            }
            else
            {
                Debug.LogError("에러: 코인 프리팹이 RoomReward에 연결되지 않았습니다!");
            }
        }
        // 2. 15% 확률로 산소 드롭 (33 ~ 53)
        else if (randomChance >= 33 && randomChance < 48)
        {
            if (oxygenPrefab != null && mapCenterPoint != null)
            {
                Instantiate(oxygenPrefab, mapCenterPoint.position, Quaternion.identity);
                Debug.Log("방 클리어: 산소 당첨! (33%)");
            }
            else
            {
                Debug.LogError("에러: 산소 프리팹이 RoomReward에 연결되지 않았습니다!");
            }
        }
        // 3. 나머지 51% 확률로 꽝 (48 ~ 99)
        else
        {
            Debug.Log("방 클리어: 아무 보상도 나오지 않았습니다. (꽝)");
        }
    }
}