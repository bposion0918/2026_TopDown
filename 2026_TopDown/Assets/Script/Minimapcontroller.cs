using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public Camera minimapCamera;

    [Header("배경 이미지 연결")]
    public RectTransform backgroundUI; // 새로 만든 배경 이미지 연결칸
    public float padding = 20f;        // 배경이 미니맵보다 얼마나 더 클지 (테두리 여백)

    private RectTransform rectTransform;

    [Header("평상시 (우측 상단)")]
    public Vector2 normalPosition = new Vector2(-20f, -20f);
    public Vector2 normalSize = new Vector2(250f, 250f);
    public float normalCamSize = 40f;

    [Header("확대 시 (정중앙)")]
    public Vector2 expandedPosition = Vector2.zero;
    public Vector2 expandedSize = new Vector2(800f, 800f);
    public float expandedCamSize = 100f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (minimapCamera == null) return;

        if (Input.GetKey(KeyCode.Tab))
        {
            // 1. 미니맵 앵커와 위치를 중앙으로 확대
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = expandedPosition;
            rectTransform.sizeDelta = expandedSize;
            minimapCamera.orthographicSize = expandedCamSize;

            // 2. 배경 이미지도 동일하게 중앙으로 확대 (패딩만큼 크기를 더 키워 테두리 느낌을 줌)
            if (backgroundUI != null)
            {
                backgroundUI.anchorMin = new Vector2(0.5f, 0.5f);
                backgroundUI.anchorMax = new Vector2(0.5f, 0.5f);
                backgroundUI.pivot = new Vector2(0.5f, 0.5f);
                backgroundUI.anchoredPosition = expandedPosition;
                backgroundUI.sizeDelta = expandedSize + new Vector2(padding, padding);
            }
        }
        else
        {
            // 1. 평상시 미니맵 위치 복구
            rectTransform.anchorMin = new Vector2(1f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(1f, 1f);
            rectTransform.anchoredPosition = normalPosition;
            rectTransform.sizeDelta = normalSize;
            minimapCamera.orthographicSize = normalCamSize;

            // 2. 평상시 배경 이미지 위치 복구
            if (backgroundUI != null)
            {
                backgroundUI.anchorMin = new Vector2(1f, 1f);
                backgroundUI.anchorMax = new Vector2(1f, 1f);
                backgroundUI.pivot = new Vector2(1f, 1f);
                backgroundUI.anchoredPosition = normalPosition;
                backgroundUI.sizeDelta = normalSize + new Vector2(padding, padding);
            }
        }
    }
}