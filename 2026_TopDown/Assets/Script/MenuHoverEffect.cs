using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("커서 설정")]
    public Transform cursorTriangle; // 이동시킬 삼각형 UI 오브젝트

    [Header("색상 설정")]
    public Graphic buttonGraphic;    // 색상을 바꿀 텍스트 또는 이미지
    public Color normalColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 평소 색상
    public Color hoverColor = Color.white;                      // 마우스 올렸을 때 색상

    private void Start()
    {
        // 게임 시작 시 버튼을 어두운 색으로 초기화
        if (buttonGraphic != null)
        {
            buttonGraphic.color = normalColor;
        }

        // 1. [추가] 처음에는 어떤 버튼도 선택되지 않았으므로 화살표를 숨깁니다.
        if (cursorTriangle != null)
        {
            cursorTriangle.gameObject.SetActive(false);
        }
    }

    // 마우스 커서가 버튼 영역 안으로 들어올 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonGraphic != null)
        {
            buttonGraphic.color = hoverColor;
        }

        if (cursorTriangle != null)
        {
            // 2. [추가] 마우스를 올리면 화살표를 다시 보이게 합니다.
            cursorTriangle.gameObject.SetActive(true);

            // 삼각형의 Y축 위치를 이 버튼에 맞춤
            cursorTriangle.position = new Vector3(cursorTriangle.position.x, transform.position.y, cursorTriangle.position.z);
        }
    }

    // 마우스 커서가 버튼 영역 밖으로 빠져나갈 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonGraphic != null)
        {
            buttonGraphic.color = normalColor;
        }

        // 3. [추가] 마우스가 버튼을 벗어나면 화살표를 숨깁니다.
        if (cursorTriangle != null)
        {
            cursorTriangle.gameObject.SetActive(false);
        }
    }
}