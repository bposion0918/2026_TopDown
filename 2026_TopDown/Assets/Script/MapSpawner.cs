using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [Header("스테이지 데이터")]
    public StageData currentStageData;

    [Header("맵 생성 설정")]
    public int maxRooms = 6; // 생성할 총 방의 개수
    public float roomWidth = 40f;  // 방의 가로 간격
    public float roomHeight = 25f; // 방의 세로 간격

    // 생성된 방들을 좌표와 함께 저장하는 딕셔너리
    private Dictionary<Vector2Int, RoomInfo> spawnedRooms = new Dictionary<Vector2Int, RoomInfo>();

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        List<Vector2Int> roomCoordinates = new List<Vector2Int>();
        Vector2Int currentPos = Vector2Int.zero; // (0,0)에서 시작
        roomCoordinates.Add(currentPos);

        // 1. 랜덤하게 이어지는 방의 좌표들 먼저 생성 (Random Walker 알고리즘)
        while (roomCoordinates.Count < maxRooms)
        {
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            Vector2Int randomDir = directions[Random.Range(0, 4)];
            currentPos += randomDir;

            if (!roomCoordinates.Contains(currentPos))
            {
                roomCoordinates.Add(currentPos);
            }
        }

        // 2. 생성된 좌표를 바탕으로 실제 방 프리팹 스폰
        foreach (Vector2Int pos in roomCoordinates)
        {
            GameObject roomPrefab;
            if (pos == Vector2Int.zero) roomPrefab = currentStageData.startRooms[0]; // 시작방
            else roomPrefab = currentStageData.normalRooms[Random.Range(0, currentStageData.normalRooms.Length)]; // 일반방 랜덤

            Vector3 spawnPosition = new Vector3(pos.x * roomWidth, pos.y * roomHeight, 0);
            GameObject newRoomObj = Instantiate(roomPrefab, spawnPosition, Quaternion.identity, transform);

            RoomInfo roomInfo = newRoomObj.GetComponent<RoomInfo>();
            roomInfo.SetCameraPositionForDoors(spawnPosition); // 카메라 중심점 전달

            spawnedRooms.Add(pos, roomInfo);
        }

        // 3. 인접한 방 확인 및 문(Door) 연결하기
        foreach (KeyValuePair<Vector2Int, RoomInfo> kvp in spawnedRooms)
        {
            Vector2Int pos = kvp.Key;
            RoomInfo room = kvp.Value;

            // 주변 좌표에 다른 방이 존재하는지 확인
            bool hasTop = spawnedRooms.ContainsKey(pos + Vector2Int.up);
            bool hasBottom = spawnedRooms.ContainsKey(pos + Vector2Int.down);
            bool hasLeft = spawnedRooms.ContainsKey(pos + Vector2Int.left);
            bool hasRight = spawnedRooms.ContainsKey(pos + Vector2Int.right);

            // 방이 있는 방향의 문만 활성화
            room.SetupDoors(hasTop, hasBottom, hasLeft, hasRight);

            // 활성화된 문들끼리 양방향 연결 (서로의 반대편 문을 연결)
            if (hasTop) room.topDoor.connectedDoor = spawnedRooms[pos + Vector2Int.up].bottomDoor;
            if (hasBottom) room.bottomDoor.connectedDoor = spawnedRooms[pos + Vector2Int.down].topDoor;
            if (hasLeft) room.leftDoor.connectedDoor = spawnedRooms[pos + Vector2Int.left].rightDoor;
            if (hasRight) room.rightDoor.connectedDoor = spawnedRooms[pos + Vector2Int.right].leftDoor;
        }
    }
}