using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [Header("현재 스테이지 데이터")]
    public StageData currentStageData;

    void Start()
    {
        SpawnRandomMap();
    }

    public void SpawnRandomMap()
    {
        if (currentStageData.mapDesigns.Length == 0)
        {
            Debug.LogError("스테이지 데이터에 등록된 맵 디자인이 없습니다!");
            return;
        }
        int randomIndex = Random.Range(0, currentStageData.mapDesigns.Length);

        Instantiate(currentStageData.mapDesigns[randomIndex], Vector3.zero, Quaternion.identity);
    }
}