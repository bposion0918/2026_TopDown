using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [Header("특수 방 생성 설정")]
    public int bossRoomCount = 1;
    public int shopRoomCount = 1;

    [Range(0f, 100f)]
    public float treasureRoomChance = 100f;
    public int maxTreasureRooms = 1;

    [Header("스테이지 데이터")]
    public StageData currentStageData;

    [Header("맵 생성 설정")]
    public int maxRooms = 15;
    public float roomWidth = 40f;
    public float roomHeight = 25f;

    private Dictionary<Vector2Int, RoomInfo> spawnedRooms = new Dictionary<Vector2Int, RoomInfo>();

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        List<Vector2Int> roomCoordinates = new List<Vector2Int>();
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        int requiredDeadEnds = bossRoomCount + shopRoomCount;
        int attempts = 0;

        // --- 1. 조건에 맞는 맵 다시 그리기 루프 ---
        while (true)
        {
            roomCoordinates.Clear();
            deadEnds.Clear();
            Vector2Int currentPos = Vector2Int.zero;
            roomCoordinates.Add(currentPos);

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

            foreach (Vector2Int pos in roomCoordinates)
            {
                if (pos == Vector2Int.zero) continue;

                int neighborCount = 0;
                if (roomCoordinates.Contains(pos + Vector2Int.up)) neighborCount++;
                if (roomCoordinates.Contains(pos + Vector2Int.down)) neighborCount++;
                if (roomCoordinates.Contains(pos + Vector2Int.left)) neighborCount++;
                if (roomCoordinates.Contains(pos + Vector2Int.right)) neighborCount++;

                if (neighborCount == 1)
                {
                    deadEnds.Add(pos);
                }
            }

            if (deadEnds.Count >= requiredDeadEnds)
            {
                break;
            }

            attempts++;
            if (attempts > 100)
            {
                Debug.LogWarning("막다른 길이 부족합니다!");
                break;
            }
        }

        // --- 2. 방 종류 배정하기 ---
        Dictionary<Vector2Int, RoomType> roomTypes = new Dictionary<Vector2Int, RoomType>();

        if (deadEnds.Count > 0 && bossRoomCount > 0)
        {
            deadEnds.Sort((a, b) => b.sqrMagnitude.CompareTo(a.sqrMagnitude));

            for (int i = 0; i < bossRoomCount && deadEnds.Count > 0; i++)
            {
                roomTypes[deadEnds[0]] = RoomType.Boss;
                deadEnds.RemoveAt(0);
            }
        }

        for (int i = 0; i < shopRoomCount && deadEnds.Count > 0; i++)
        {
            int randIdx = Random.Range(0, deadEnds.Count);
            roomTypes[deadEnds[randIdx]] = RoomType.Shop;
            deadEnds.RemoveAt(randIdx);
        }

        for (int i = 0; i < maxTreasureRooms && deadEnds.Count > 0; i++)
        {
            if (Random.Range(0f, 100f) <= treasureRoomChance)
            {
                int randIdx = Random.Range(0, deadEnds.Count);
                roomTypes[deadEnds[randIdx]] = RoomType.Treasure;
                deadEnds.RemoveAt(randIdx);
            }
        }

        // --- 3. 프리팹 스폰 ---
        foreach (Vector2Int pos in roomCoordinates)
        {
            RoomType currentType = RoomType.Normal;
            if (pos == Vector2Int.zero) currentType = RoomType.Start;
            else if (roomTypes.ContainsKey(pos)) currentType = roomTypes[pos];

            GameObject roomPrefab = null;
            switch (currentType)
            {
                case RoomType.Start: roomPrefab = currentStageData.startRooms[0]; break;
                case RoomType.Boss: roomPrefab = currentStageData.bossRooms[0]; break;
                case RoomType.Shop: roomPrefab = currentStageData.shopRooms[0]; break;
                case RoomType.Treasure: roomPrefab = currentStageData.treasureRooms[0]; break;
                default: roomPrefab = currentStageData.normalRooms[Random.Range(0, currentStageData.normalRooms.Length)]; break;
            }

            Vector3 spawnPosition = new Vector3(pos.x * roomWidth, pos.y * roomHeight, 0);
            GameObject newRoomObj = Instantiate(roomPrefab, spawnPosition, Quaternion.identity, transform);

            RoomInfo roomInfo = newRoomObj.GetComponent<RoomInfo>();
            if (roomInfo != null)
            {
                roomInfo.roomType = currentType;
                roomInfo.SetCameraPositionForDoors(spawnPosition);
                spawnedRooms.Add(pos, roomInfo);
            }
        }

        // --- 4. 문 연결 및 우선순위 디자인 바꾸기 ---
        foreach (KeyValuePair<Vector2Int, RoomInfo> kvp in spawnedRooms)
        {
            Vector2Int pos = kvp.Key;
            RoomInfo room = kvp.Value;

            bool hasTop = spawnedRooms.ContainsKey(pos + Vector2Int.up);
            bool hasBottom = spawnedRooms.ContainsKey(pos + Vector2Int.down);
            bool hasLeft = spawnedRooms.ContainsKey(pos + Vector2Int.left);
            bool hasRight = spawnedRooms.ContainsKey(pos + Vector2Int.right);

            room.SetupDoors(hasTop, hasBottom, hasLeft, hasRight);

            if (hasTop)
            {
                RoomInfo targetRoom = spawnedRooms[pos + Vector2Int.up];
                room.topDoor.connectedDoor = targetRoom.bottomDoor;
                RoomType displayType = GetPriorityRoomType(room.roomType, targetRoom.roomType);
                room.topDoor.SetDoorAppearance(displayType);
            }
            if (hasBottom)
            {
                RoomInfo targetRoom = spawnedRooms[pos + Vector2Int.down];
                room.bottomDoor.connectedDoor = targetRoom.topDoor;
                RoomType displayType = GetPriorityRoomType(room.roomType, targetRoom.roomType);
                room.bottomDoor.SetDoorAppearance(displayType);
            }
            if (hasLeft)
            {
                RoomInfo targetRoom = spawnedRooms[pos + Vector2Int.left];
                room.leftDoor.connectedDoor = targetRoom.rightDoor;
                RoomType displayType = GetPriorityRoomType(room.roomType, targetRoom.roomType);
                room.leftDoor.SetDoorAppearance(displayType);
            }
            if (hasRight)
            {
                RoomInfo targetRoom = spawnedRooms[pos + Vector2Int.right];
                room.rightDoor.connectedDoor = targetRoom.leftDoor;
                RoomType displayType = GetPriorityRoomType(room.roomType, targetRoom.roomType);
                room.rightDoor.SetDoorAppearance(displayType);
            }
        }
    }

    //  핵심: 두 방을 비교해서 특수 방이 하나라도 있으면 그 디자인을 따라가게 하는 함수
    private RoomType GetPriorityRoomType(RoomType current, RoomType target)
    {
        if (current == RoomType.Boss || target == RoomType.Boss) return RoomType.Boss;
        if (current == RoomType.Treasure || target == RoomType.Treasure) return RoomType.Treasure;
        if (current == RoomType.Shop || target == RoomType.Shop) return RoomType.Shop;

        return RoomType.Normal;
    }
}