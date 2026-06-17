using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("버튼 이미지 설정")]
    public Graphic buttonGraphic;
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    [Header("텍스트 설정 (TMP)")]
    public TextMeshProUGUI buttonText;
    public Color textNormalColor = Color.gray;
    public Color textHoverColor = Color.white;

    [Header("화살표(커서) 설정")]
    public GameObject cursorObject; // 마우스를 올렸을 때 나타날 화살표(닻) 오브젝트

    private void Start()
    {
        // 시작할 때 원래 색상으로 초기화하고 화살표는 숨겨둡니다.
        if (buttonGraphic != null) buttonGraphic.color = normalColor;
        if (buttonText != null) buttonText.color = textNormalColor;
        if (cursorObject != null) cursorObject.SetActive(false);
    }

    // 마우스가 버튼 위로 올라왔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonGraphic != null) buttonGraphic.color = hoverColor;
        if (buttonText != null) buttonText.color = textHoverColor;

        // 화살표 켜기
        if (cursorObject != null) cursorObject.SetActive(true);
    }

    // 마우스가 버튼 밖으로 나갔을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonGraphic != null) buttonGraphic.color = normalColor;
        if (buttonText != null) buttonText.color = textNormalColor;

        // 화살표 끄기
        if (cursorObject != null) cursorObject.SetActive(false);
    }
}