// RoomInfo.cs
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [Header("방 종류")]
    public RoomType roomType = RoomType.Normal; // 이제 에러 없이 외부의 RoomType.cs를 가져다 씁니다.

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

    public void SetupDoors(bool hasTop, bool hasBottom, bool hasLeft, bool hasRight)
    {
        if (topDoor != null) topDoor.gameObject.SetActive(hasTop);
        if (topWallBlocker != null) topWallBlocker.SetActive(!hasTop);

        if (bottomDoor != null) bottomDoor.gameObject.SetActive(hasBottom);
        if (bottomWallBlocker != null) bottomWallBlocker.SetActive(!hasBottom);

        if (leftDoor != null) leftDoor.gameObject.SetActive(hasLeft);
        if (leftWallBlocker != null) leftWallBlocker.SetActive(!hasLeft);

        if (rightDoor != null) rightDoor.gameObject.SetActive(hasRight);
        if (rightWallBlocker != null) rightWallBlocker.SetActive(!hasRight);
    }

    public void SetCameraPositionForDoors(Vector3 pos)
    {
        if (topDoor != null) topDoor.cameraTargetPos = pos;
        if (bottomDoor != null) bottomDoor.cameraTargetPos = pos;
        if (leftDoor != null) leftDoor.cameraTargetPos = pos;
        if (rightDoor != null) rightDoor.cameraTargetPos = pos;
    }
}