using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [Header("이 방의 문들 (없으면 비워두기)")]
    public Door topDoor;
    public Door bottomDoor;
    public Door leftDoor;
    public Door rightDoor;

    // 인접한 방이 있는지 여부에 따라 문을 활성화/비활성화 하는 함수
    public void SetupDoors(bool hasTop, bool hasBottom, bool hasLeft, bool hasRight)
    {
        if (topDoor != null) topDoor.gameObject.SetActive(hasTop);
        if (bottomDoor != null) bottomDoor.gameObject.SetActive(hasBottom);
        if (leftDoor != null) leftDoor.gameObject.SetActive(hasLeft);
        if (rightDoor != null) rightDoor.gameObject.SetActive(hasRight);
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