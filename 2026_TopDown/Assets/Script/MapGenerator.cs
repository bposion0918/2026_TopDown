using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("전체 스테이지 데이터 리스트")]
    // 에디터에서 Stage1_Data, Stage2_Data 등을 순서대로 넣어둡니다.
    public List<StageData> allStageData = new List<StageData>();

    [Header("맵 세팅")]
    public Vector2 roomOffset; // 방의 실제 가로/세로 길이 (유니티 좌표 기준)
    public int baseRoomCount = 5; // 1스테이지의 기본 방 개수

    private StageData currentStageData;
    private Dictionary<Vector2, GameObject> spawnedRooms = new Dictionary<Vector2, GameObject>();
    private List<Vector2> roomPositions = new List<Vector2>();

    void Start()
    {
        // 1. GameDataManager 연동하여 현재 스테이지 데이터 가져오기
        int currentStage = GameDataManager.instance != null ? GameDataManager.instance.playerData.stage : 1;

        // 올바른 스테이지 데이터를 선택 (인덱스는 0부터 시작하므로 stage - 1)
        if (allStageData.Count >= currentStage)
        {
            currentStageData = allStageData[currentStage - 1];
        }
        else
        {
            Debug.LogError("해당 스테이지의 StageData가 등록되지 않았습니다!");
            return;
        }

        GenerateMap();
    }

    void GenerateMap()
    {
        int maxRooms = baseRoomCount + (currentStageData.stageIndex * 2);

        SpawnRoom(Vector2.zero, RoomType.Start);

        while (roomPositions.Count < maxRooms)
        {
            Vector2 randomExistingRoom = roomPositions[Random.Range(0, roomPositions.Count)];
            Vector2 newPos = GetRandomAdjacentPosition(randomExistingRoom);

            if (!spawnedRooms.ContainsKey(newPos))
            {
                SpawnRoom(newPos, RoomType.Normal);
            }
        }

        AssignSpecialRooms();

        MovePlayerToStart();
    }

    void SpawnRoom(Vector2 gridPos, RoomType type)
    {
        Vector3 worldPos = new Vector3(gridPos.x * roomOffset.x, gridPos.y * roomOffset.y, 0);
        GameObject prefabToSpawn = null;

        switch (type)
        {
            case RoomType.Start:
                prefabToSpawn = currentStageData.startRoomPrefab;
                break;
            case RoomType.Normal:
                // 일반 방 배열 중에서 무작위로 하나를 초이스!
                int randIndex = Random.Range(0, currentStageData.normalRoomPrefabs.Length);
                prefabToSpawn = currentStageData.normalRoomPrefabs[randIndex];
                break;
            case RoomType.Treasure:
                prefabToSpawn = currentStageData.treasureRoomPrefab;
                break;
            case RoomType.Shop:
                prefabToSpawn = currentStageData.shopRoomPrefab;
                break;
            case RoomType.Boss:
                prefabToSpawn = currentStageData.bossRoomPrefab;
                break;
        }

        if (prefabToSpawn != null)
        {
            GameObject newRoom = Instantiate(prefabToSpawn, worldPos, Quaternion.identity);
            spawnedRooms.Add(gridPos, newRoom);
            if (!roomPositions.Contains(gridPos)) roomPositions.Add(gridPos);
        }
    }
    void AssignSpecialRooms()
    {
        // 맵의 가장 끝(막다른 길)에 있는 방을 찾아서 보스방이나 보물방 프리팹으로 교체하는 
        // 세부 로직이 들어갈 자리입니다. 보통은 시작방에서 가장 먼 거리에 있는 방을 보스방으로 만듭니다.
    }

    Vector2 GetRandomAdjacentPosition(Vector2 currentPos)
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        return currentPos + directions[Random.Range(0, directions.Length)];
    }

    void MovePlayerToStart()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) player.transform.position = Vector3.zero;
    }
}