using UnityEngine;

public class RoomIcon : MonoBehaviour
{
    private SpriteRenderer sr;
    private bool isVisited = false;

    public Color currentColor = Color.white;                               // 현재 위치 (흰색)
    public Color visitedColor = new Color(0.5f, 0.5f, 0.5f, 1f);           // 방문한 방 (회색)
    public Color hiddenColor = new Color(0f, 0f, 0f, 0f);                  // 미방문 방 (투명)

    // Start보다 먼저, 생성 즉시 무조건 실행되는 Awake 함수 사용
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (sr != null)
        {
            sr.color = hiddenColor; // 시작 시 투명하게 숨김
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isVisited = true;
            if (sr != null)
            {
                sr.color = currentColor;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isVisited && sr != null)
            {
                sr.color = visitedColor;
            }
        }
    }
}