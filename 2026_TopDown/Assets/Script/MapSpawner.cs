using UnityEngine;
using System.Collections.Generic;

public class MapSpawner : MonoBehaviour
{
    [Header("스테이지 데이터")]
    public StageData currentStageData;

    [Header("방 생성 간격 (방의 가로/세로 크기보다 커야 함)")]
    public float xOffset = 40f;
    public float yOffset = 25f;

    void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        // 1. 시작 방 생성 (0, 0 위치)
        GameObject startRoom = Instantiate(currentStageData.startRooms[0], Vector3.zero, Quaternion.identity);

        // 2. 일반 방 생성 (오른쪽에 하나 만들어보기 예시)
        Vector3 nextRoomPos = new Vector3(xOffset, 0, 0);
        GameObject normalRoom = Instantiate(currentStageData.normalRooms[Random.Range(0, currentStageData.normalRooms.Length)], nextRoomPos, Quaternion.identity);

        // TODO: 여기서 두 방의 문(Door)들을 찾아서 서로의 teleportTarget을 연결해주는 작업이 필요합니다.
    }
}