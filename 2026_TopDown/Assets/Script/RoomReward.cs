using UnityEngine;

public class RoomReward : MonoBehaviour
{
    [Header("보상 설정")]
    public GameObject coinPrefab;
    public Transform mapCenterPoint;

    public void DropReward()
    {
        // 0부터 99까지 난수 생성
        int randomChance = Random.Range(0, 100);

        // 33% 확률 (0 ~ 32)
        if (randomChance < 33)
        {
            // 코인 프리팹과 생성 위치가 모두 정상적으로 연결되어 있는지 확인
            if (coinPrefab != null && mapCenterPoint != null)
            {
                Instantiate(coinPrefab, mapCenterPoint.position, Quaternion.identity);
                Debug.Log("코인이 정상적으로 생성되었습니다! (떴음)");
            }
            else
            {
                // 당첨은 되었으나, 유니티 에디터에서 프리팹이나 위치 할당을 까먹었을 때 에러 발생
                Debug.LogError("에러: 코인을 생성해야 하지만 프리팹이나 생성 위치(MapCenterPoint)가 연결되지 않았습니다!");
            }
        }
        // 67% 확률 (33 ~ 99)
        else if (randomChance >= 33 && randomChance <= 100)
        {
            Debug.Log("이번 방에서는 코인이 생성되지 않았습니다. (안 떴음)");
        }
        // 그 외의 알 수 없는 값이 나왔을 때 (둘 다 아닐 때)
        else
        {
            Debug.LogError("에러: 알 수 없는 확률 연산 오류가 발생했습니다!");
        }
    }
}