using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [Header("이동 속도 설정")]
    public float slowMoveSpeed = 0.2f; // 처음 1초 동안의 아주 느린 속도
    public float fastMoveSpeed = 1.5f; // 1초 뒤에 적용될 원래(빠른) 속도
    public float speedUpTime = 1.0f;   // 속도가 빨라지기 시작하는 시간 (1초)

    [Header("기타 설정")]
    public float shrinkSpeed = 0.5f;   // 작아지는 속도
    public float destroyTime = 2f;     // 사라지기까지 걸리는 시간

    private TextMeshPro textMesh;
    private Color textColor;
    private float timer = 0f;

    public void Setup(int damage)
    {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = damage.ToString();
        textColor = textMesh.color;

        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // 시간이 흐르는 것을 기록합니다.
        timer += Time.deltaTime;

        // 1초(speedUpTime)가 지났는지 확인하여 현재 속도를 결정합니다.
        float currentSpeed = (timer < speedUpTime) ? slowMoveSpeed : fastMoveSpeed;

        // 1. 결정된 속도로 위로 이동
        transform.position += Vector3.up * currentSpeed * Time.deltaTime;

        // 2. 텍스트 크기가 서서히 작아짐
        transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;
        if (transform.localScale.x < 0) transform.localScale = Vector3.zero;

        // 3. 투명도(Alpha)를 서서히 줄여서 자연스럽게 페이드아웃 효과
        textColor.a -= (1f / destroyTime) * Time.deltaTime;
        textMesh.color = textColor;
    }
}