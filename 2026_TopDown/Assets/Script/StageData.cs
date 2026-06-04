using UnityEngine;

[CreateAssetMenu(fileName = "NewStageData", menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    public int stageIndex;

    [Header("방 종류별 프리팹 목록")]
    public GameObject[] startRooms;     // 시작방 디자인들
    public GameObject[] normalRooms;    // 일반 전투방 디자인들
    public GameObject[] treasureRooms;  // 보물방 디자인들
    public GameObject[] shopRooms;      // 상점방 디자인들
    public GameObject[] bossRooms;      // 보스방 디자인들
}