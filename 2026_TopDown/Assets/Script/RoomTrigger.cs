using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    // 플레이어가 이 트리거에 닿았을 때 실행됨
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트의 태그가 "Player"인지 확인
        if (other.CompareTag("Player"))
        {
            // 카메라 컨트롤러에게 이 트리거 오브젝트의 중심 위치로 이동하라고 명령
            CameraController.instance.ChangeRoom(transform.position);
        }
    }
}
