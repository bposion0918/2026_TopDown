using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // TextMeshPro를 제어하기 위해 필수 추가

public class MenuHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("버튼 이미지 설정")]
    public Graphic buttonGraphic;
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    [Header("텍스트 설정 (TMP)")]
    public TextMeshProUGUI buttonText;       // 글자 오브젝트를 넣을 칸
    public Color textNormalColor = Color.gray; // 평상시 글자 색상
    public Color textHoverColor = Color.white; // 마우스를 올렸을 때 밝아질 글자 색상

    private void Start()
    {
        // 시작할 때 원래 색상으로 초기화
        if (buttonGraphic != null) buttonGraphic.color = normalColor;
        if (buttonText != null) buttonText.color = textNormalColor;
    }

    // 마우스가 버튼 위로 올라왔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonGraphic != null) buttonGraphic.color = hoverColor;
        if (buttonText != null) buttonText.color = textHoverColor; // 텍스트도 같이 변경
    }

    // 마우스가 버튼 밖으로 나갔을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonGraphic != null) buttonGraphic.color = normalColor;
        if (buttonText != null) buttonText.color = textNormalColor; // 텍스트도 원래대로 복구
    }
}