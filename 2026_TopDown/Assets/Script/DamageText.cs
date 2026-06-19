using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [Header("이동 속도 설정")]
    public float slowMoveSpeed = 0.2f;
    public float fastMoveSpeed = 1.5f;
    public float speedUpTime = 1.0f;

    [Header("기타 설정")]
    public float shrinkSpeed = 0.5f;
    public float destroyTime = 2f;

    private TextMeshPro textMesh;
    private Color textColor;
    private float timer = 0f;

    // 수정됨: isFullyCharged 매개변수 추가
    public void Setup(int damage, bool isFullyCharged)
    {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = damage.ToString();

        // 추가됨: 풀차지면 빨간색, 아니면 하얀색으로 텍스트 색상 변경
        textMesh.color = isFullyCharged ? Color.red : Color.white;

        textColor = textMesh.color;

        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // ... (Update 안의 내용은 기존 코드 그대로 두시면 됩니다)
        timer += Time.deltaTime;
        float currentSpeed = (timer < speedUpTime) ? slowMoveSpeed : fastMoveSpeed;
        transform.position += Vector3.up * currentSpeed * Time.deltaTime;
        transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;
        if (transform.localScale.x < 0) transform.localScale = Vector3.zero;
        textColor.a -= (1f / destroyTime) * Time.deltaTime;
        textMesh.color = textColor;
    }
}