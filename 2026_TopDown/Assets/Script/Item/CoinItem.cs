using System.Collections;
using UnityEngine;

public class CoinItem : MonoBehaviour
{
    public int coinValue = 1;

    // 애니메이션 도중에 또 먹어지는(중복 획득) 버그 방지용
    private bool isPickedUp = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPickedUp && collision.CompareTag("Player"))
        {
            isPickedUp = true; // 이제 두 번 못 먹음!

            // 1. 돈 수치는 먹자마자 바로 올려주기
            if (GameDataManager.instance != null)
            {
                GameDataManager.instance.AddMoney(coinValue);
            }

            // 2. 머리 위로 뜨면서 애니메이션 시작
            StartCoroutine(PickupEffectRoutine(collision.transform));
        }
    }

    private IEnumerator PickupEffectRoutine(Transform playerTransform)
    {
        // 더 이상 물리적으로 부딪히지 않게 콜라이더 끄기
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 플레이어의 자식(Child)으로 넣어서, 플레이어가 이동해도 머리 위를 따라다니게 함
        transform.SetParent(playerTransform);
        transform.localPosition = new Vector3(0f, 0.2f, 0f); // 머리 위로 살짝 띄움 (필요하면 1.2f 수치 조절해!)

        Vector3 originalScale = transform.localScale;
        Vector3 startScale = originalScale * 0.5f; // 원래 크기의 0.5배로 시작
        Vector3 targetScale = originalScale * 1.2f; // 원래 크기보다 살짝 더 크게(1.2배) 뿅! 커져야 먹는 맛이 남

        // [1단계] 0.5초 동안 0.5배에서 타겟 크기로 커지기
        float elapsedTime = 0f;
        float appearTime = 0.5f;
        while (elapsedTime < appearTime)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / appearTime);
            yield return null;
        }
        transform.localScale = targetScale; // 혹시 모를 오차 깔끔하게 보정

        // [2단계] 1.5초 동안 그 상태로 지속 (머리 위에서 자랑하기)
        yield return new WaitForSeconds(1.5f);

        // [3단계] 1초 동안 스르륵 0으로 작아지면서 사라지기
        elapsedTime = 0f;
        float disappearTime = 1.0f;
        while (elapsedTime < disappearTime)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, elapsedTime / disappearTime);
            yield return null;
        }

        // 연출이 다 끝났으니 진짜로 오브젝트 파괴!
        Destroy(gameObject);
    }
}