using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [Header("추적할 대상")]
    public Transform player;

    [Header("방 간격 설정 (맵 스포너와 동일하게)")]
    public float roomWidth = 30f;
    public float roomHeight = 25f;

    void LateUpdate()
    {
        if (player == null) return;

        // 플레이어의 현재 위치를 방 크기 단위로 반올림하여, 현재 속한 방의 정중앙 좌표를 계산합니다.
        float targetX = Mathf.Round(player.position.x / roomWidth) * roomWidth;
        float targetY = Mathf.Round(player.position.y / roomHeight) * roomHeight;

        // 카메라 위치를 현재 방의 정중앙으로 고정 (Z축은 카메라를 위해 -10 유지)
        transform.position = new Vector3(targetX, targetY, -10f);
    }
}