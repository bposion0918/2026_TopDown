using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("이동 설정")]
    public Transform teleportTarget; // 플레이어가 순간이동할 도착 지점 (다음 방의 문 앞)
    public Vector3 nextRoomCameraPosition; // 다음 방의 카메라 중심 좌표

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. 플레이어 위치를 다음 방으로 순간이동
            collision.transform.position = teleportTarget.position;

            // 2. 카메라도 다음 방으로 이동
            if (CameraController.instance != null)
            {
                CameraController.instance.ChangeRoom(nextRoomCameraPosition);
            }
        }
    }
}