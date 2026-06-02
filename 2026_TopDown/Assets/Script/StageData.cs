using UnityEngine;

[CreateAssetMenu(fileName = "NewStageData", menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    public int stageIndex;

    [Header("방 프리팹 풀 (Pool)")]
    public GameObject startRoomPrefab;      // 시작 방 프리팹
    public GameObject[] normalRoomPrefabs;  // 일반 방 프리팹들 (여러 개 등록 가능)
    public GameObject treasureRoomPrefab;   // 보물 방 프리팹
    public GameObject shopRoomPrefab;       // 상점 방 프리팹
    public GameObject bossRoomPrefab;       // 보스 방 프리팹
}
