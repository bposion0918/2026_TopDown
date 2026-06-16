using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [Header("이 방의 문들 (없으면 비워두기)")]
    public Door topDoor;
    public Door bottomDoor;
    public Door leftDoor;
    public Door rightDoor;

    [Header("문이 없을 때 구멍을 막아줄 벽 (가짜 벽)")]
    public GameObject topWallBlocker;
    public GameObject bottomWallBlocker;
    public GameObject leftWallBlocker;
    public GameObject rightWallBlocker;

    // 인접한 방이 있는지 여부에 따라 문을 활성화/비활성화 하는 함수
    public void SetupDoors(bool hasTop, bool hasBottom, bool hasLeft, bool hasRight)
    {
        // [위쪽] 문이 있으면 켜고, 가짜 벽은 끕니다 (!hasTop 은 반대를 의미)
        if (topDoor != null) topDoor.gameObject.SetActive(hasTop);
        if (topWallBlocker != null) topWallBlocker.SetActive(!hasTop);

        // [아래쪽]
        if (bottomDoor != null) bottomDoor.gameObject.SetActive(hasBottom);
        if (bottomWallBlocker != null) bottomWallBlocker.SetActive(!hasBottom);

        // [왼쪽]
        if (leftDoor != null) leftDoor.gameObject.SetActive(hasLeft);
        if (leftWallBlocker != null) leftWallBlocker.SetActive(!hasLeft);

        // [오른쪽]
        if (rightDoor != null) rightDoor.gameObject.SetActive(hasRight);
        if (rightWallBlocker != null) rightWallBlocker.SetActive(!hasRight);
    }

    // 각 문에게 "너희 방의 카메라 중심점은 여기야"라고 알려주는 함수
    public void SetCameraPositionForDoors(Vector3 pos)
    {
        if (topDoor != null) topDoor.cameraTargetPos = pos;
        if (bottomDoor != null) bottomDoor.cameraTargetPos = pos;
        if (leftDoor != null) leftDoor.cameraTargetPos = pos;
        if (rightDoor != null) rightDoor.cameraTargetPos = pos;
    }
}