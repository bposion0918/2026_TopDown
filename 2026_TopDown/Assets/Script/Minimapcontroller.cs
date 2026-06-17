using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public Camera minimapCamera;
    private RectTransform rectTransform;

    [Header("평상시 (우측 상단)")]
    // 우측 상단 모서리로부터 얼만큼 떨어져 있을지 결정합니다.
    public Vector2 normalPosition = new Vector2(-20f, -20f);
    public Vector2 normalSize = new Vector2(250f, 250f);
    public float normalCamSize = 40f;

    [Header("확대 시 (정중앙)")]
    public Vector2 expandedSize = new Vector2(800f, 800f);
    public float expandedCamSize = 100f; // 맵 전체가 보이게 카메라 시야를 넓힙니다.

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (minimapCamera == null) return;

        if (Input.GetKey(KeyCode.Tab))
        {
            // 1. 앵커(기준점)와 피벗을 화면 '정중앙'으로 변경
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            // 2. 위치를 중앙(0,0)으로 맞추고 크기와 시야를 확대
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = expandedSize;
            minimapCamera.orthographicSize = expandedCamSize;
        }
        else
        {
            // 1. 앵커(기준점)와 피벗을 다시 화면 '우측 상단'으로 복구
            rectTransform.anchorMin = new Vector2(1f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(1f, 1f);

            // 2. 우측 상단 여백 위치로 맞추고 크기와 시야를 축소
            rectTransform.anchoredPosition = normalPosition;
            rectTransform.sizeDelta = normalSize;
            minimapCamera.orthographicSize = normalCamSize;
        }
    }
}