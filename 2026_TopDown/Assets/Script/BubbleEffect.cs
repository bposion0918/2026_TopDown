using UnityEngine;
using System.Collections;

public class BubbleEffect : MonoBehaviour
{
    [Header("공기방울 설정")]
    public float floatSpeed = 1.5f; // 위로 올라가는 속도
    public float swayAmount = 0.3f; // 좌우로 흔들리는 폭
    public float lifeTime = 1.0f;   // 방울이 유지되는 시간

    private Vector3 startPos;
    private SpriteRenderer sr;

    void Start()
    {
        startPos = transform.position;
        sr = GetComponent<SpriteRenderer>();

        // 방울마다 크기, 속도, 흔들림을 살짝씩 다르게 해서 자연스럽게 만듦!
        floatSpeed += Random.Range(-0.5f, 0.5f);
        swayAmount += Random.Range(-0.1f, 0.2f);
        transform.localScale *= Random.Range(0.7f, 1.3f);

        StartCoroutine(FadeAndDestroyRoutine());
    }

    void Update()
    {
        // 1. 위로 이동
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        // 2. 좌우로 뽀글뽀글 흔들림 (Mathf.Sin 활용)
        float sway = Mathf.Sin(Time.time * 6f) * swayAmount * Time.deltaTime;
        transform.Translate(Vector3.right * sway);
    }

    private IEnumerator FadeAndDestroyRoutine()
    {
        float elapsed = 0f;
        Color startColor = sr.color;

        while (elapsed < lifeTime)
        {
            elapsed += Time.deltaTime;
            // 시간이 지날수록 점점 투명해짐
            float alpha = Mathf.Lerp(1f, 0f, elapsed / lifeTime);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // 완전히 투명해지면 삭제
        Destroy(gameObject);
    }
}