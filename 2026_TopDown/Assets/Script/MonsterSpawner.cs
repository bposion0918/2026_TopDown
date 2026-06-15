using UnityEngine;
using System.Collections.Generic;

public class MonsterSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject[] monsterPrefabs;
    public int minMonsters = 1;
    public int maxMonsters = 3;

    [Header("스폰 범위 설정")]
    public Vector2 spawnAreaSize = new Vector2(10f, 10f);

    [Header("방 잠금 시스템")]
    public Door[] doorsInRoom; // 이 방에 있는 모든 문들을 넣어주세요!

    private List<GameObject> spawnedMonsters = new List<GameObject>();
    private bool isRoomLocked = false;
    private bool isCleared = false;

    // [추가] 방 보상을 지급할 스크립트 변수
    private RoomReward roomReward;

    void Start()
    {
        roomReward = GetComponentInParent<RoomReward>();

        SpawnMonsters();

        // 소환된 몬스터가 한 마리도 없다면 처음부터 클리어된 방으로 취급합니다.
        if (spawnedMonsters.Count == 0)
        {
            isCleared = true;
        }
    }

    public void SpawnMonsters()
    {
        if (monsterPrefabs == null || monsterPrefabs.Length == 0) return;

        int spawnCount = Random.Range(minMonsters, maxMonsters + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject monsterToSpawn = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
            Vector2 randomPos = GetRandomPosition();

            GameObject newMonster = Instantiate(monsterToSpawn, randomPos, Quaternion.identity, transform);

            // 생성된 몬스터를 명부에 등록합니다!
            spawnedMonsters.Add(newMonster);
        }
    }

    private Vector2 GetRandomPosition()
    {
        float randomX = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float randomY = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);
        return (Vector2)transform.position + new Vector2(randomX, randomY);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }

    // 플레이어가 이 방 중앙(트리거 영역)으로 들어왔을 때 발동!
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 아직 안 깼고, 잠긴 적도 없다면 -> 문 닫아!
        if (collision.CompareTag("Player") && !isCleared && !isRoomLocked)
        {
            LockDoors();
        }
    }

    private void Update()
    {
        // 방이 잠겨있고, 아직 클리어하지 않은 상태라면 매 프레임 몬스터 수를 검사합니다.
        if (isRoomLocked && !isCleared)
        {
            // 리스트에서 체력이 0이 되어 삭제된(null) 몬스터들을 명부에서 솎아냅니다.
            spawnedMonsters.RemoveAll(monster => monster == null);

            // 남은 몬스터가 0마리라면 문을 개방하고 보상을 지급합니다!
            if (spawnedMonsters.Count == 0)
            {
                UnlockDoors();

                // 코인 보상 지급 로직 실행
                if (roomReward != null)
                {
                    roomReward.DropReward();
                }
                else
                {
                    Debug.LogError("에러: MonsterSpawner가 RoomReward 스크립트를 찾지 못했습니다! (프리팹에 잘 붙어있는지 확인하세요)");
                }
            }
        }
    }

    private void LockDoors()
    {
        isRoomLocked = true;
        foreach (Door door in doorsInRoom)
        {
            if (door != null) door.CloseDoor();
        }
    }

    private void UnlockDoors()
    {
        isCleared = true;
        isRoomLocked = false;
        foreach (Door door in doorsInRoom)
        {
            if (door != null) door.OpenDoor();
        }
    }
}