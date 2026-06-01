using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance; // 다른 스크립트에서 쉽게 접근하기 위한 싱글톤

    [Header("카메라 이동 속도")]
    public float speed = 5f;

    private Vector3 targetPosition;

    void Awake()
    {
        instance = this;
        // 시작할 때는 카메라의 현재 위치를 목표 위치로 설정
        targetPosition = transform.position;
    }

    void Update()
    {
        // 현재 위치에서 목표 위치로 부드럽게(Lerp) 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
    }

    // 트리거에서 호출할 함수: 새로운 방의 중심 위치를 전달받음
    public void ChangeRoom(Vector3 newRoomPosition)
    {
        // 2D 게임이므로 카메라의 Z축(보통 -10)은 그대로 유지하고 X, Y 위치만 변경
        targetPosition = new Vector3(newRoomPosition.x, newRoomPosition.y, transform.position.z);
    }
}
