using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [Header("현재 스테이지 데이터")]
    public StageData currentStageData; // 인스펙터에서 Stage1_Data를 끌어다 넣을 곳

    void Start()
    {
        SpawnRandomMap();
    }

    public void SpawnRandomMap()
    {
        // 1. 등록된 맵 디자인이 하나도 없으면 에러 메시지를 띄우고 정지
        if (currentStageData.mapDesigns.Length == 0)
        {
            Debug.LogError("스테이지 데이터에 등록된 맵 디자인이 없습니다!");
            return;
        }

        // 2. 0부터 (맵 디자인 개수 - 1) 사이의 숫자 중 하나를 랜덤으로 뽑음
        int randomIndex = Random.Range(0, currentStageData.mapDesigns.Length);

        // 3. 뽑힌 번호의 맵 프리팹을 화면 정중앙(0,0,0)에 소환!
        Instantiate(currentStageData.mapDesigns[randomIndex], Vector3.zero, Quaternion.identity);
    }
}