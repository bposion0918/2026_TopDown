using System.Collections;
using UnityEngine;

public class CoinItem : MonoBehaviour
{
    public int coinValue = 1;

    private bool isPickedUp = false;
    private Vector3 targetOriginalScale; // 프리팹의 원래 크기를 저장할 변수

    private void Awake()
    {
        // 생성되자마자 본래 크기를 기억합니다.
        targetOriginalScale = transform.localScale;
    }

    private void Start()
    {
        // 등장 애니메이션 시작
        StartCoroutine(SpawnAnimationRoutine());
    }

    private IEnumerator SpawnAnimationRoutine()
    {
        // 크기를 0으로 초기화
        transform.localScale = Vector3.zero;

        float spawnDuration = 0.4f; // 0.4초 동안 커집니다. (원하는 시간으로 조절 가능)
        float elapsedTime = 0f;

        while (elapsedTime < spawnDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, targetOriginalScale, elapsedTime / spawnDuration);
            yield return null;
        }

        transform.localScale = targetOriginalScale; // 오차 보정
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPickedUp && collision.CompareTag("Player"))
        {
            isPickedUp = true;

            if (GameDataManager.instance != null)
            {
                GameDataManager.instance.AddMoney(coinValue);
            }

            StartCoroutine(PickupEffectRoutine(collision.transform));
        }
    }

    private IEnumerator PickupEffectRoutine(Transform playerTransform)
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        transform.SetParent(playerTransform);
        transform.localPosition = new Vector3(0f, 0.2f, 0f);

        // 미리 저장해둔 원래 크기를 기준으로 계산합니다.
        Vector3 startScale = targetOriginalScale * 0.5f;
        Vector3 popScale = targetOriginalScale * 1.2f;

        // [1단계] 0.5초 동안 커지기
        float elapsedTime = 0f;
        float appearTime = 0.5f;
        while (elapsedTime < appearTime)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, popScale, elapsedTime / appearTime);
            yield return null;
        }
        transform.localScale = popScale;

        // [2단계] 1.5초 유지
        yield return new WaitForSeconds(1.5f);

        // [3단계] 1초 동안 작아지면서 소멸
        elapsedTime = 0f;
        float disappearTime = 1.0f;
        while (elapsedTime < disappearTime)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(popScale, Vector3.zero, elapsedTime / disappearTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}