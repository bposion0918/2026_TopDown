using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("플레이어가 등장할 위치 (이 문 앞)")]
    public Transform spawnPoint;

    [HideInInspector] public Door connectedDoor; // 알고리즘이 자동으로 연결해 줄 반대편 문
    [HideInInspector] public Vector3 cameraTargetPos; // 이 문이 속한 방의 카메라 위치

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 닿았고, 연결된 문이 존재한다면
        if (collision.CompareTag("Player") && connectedDoor != null)
        {
            // 1. 플레이어를 '반대편 문의 스폰 위치'로 이동
            collision.transform.position = connectedDoor.spawnPoint.position;

            // 2. 카메라도 반대편 방의 위치로 이동
            if (CameraController.instance != null)
            {
                CameraController.instance.ChangeRoom(connectedDoor.cameraTargetPos);
            }
        }
    }
}