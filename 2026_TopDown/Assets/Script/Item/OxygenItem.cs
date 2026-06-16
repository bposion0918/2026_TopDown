using System.Collections;
using UnityEngine;

public class OxygenItem : MonoBehaviour
{
    [Header("산소 회복량 (%)")]
    public float restorePercentage = 10f;

    private bool isPickedUp = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPickedUp && collision.CompareTag("Player"))
        {
            isPickedUp = true;

            // 1. 산소 수치 바로 채워주기
            PlayerOxygen playerOxygen = collision.GetComponent<PlayerOxygen>();
            if (playerOxygen != null)
            {
                playerOxygen.AddOxygenByPercentage(restorePercentage);
            }

            // 2. 애니메이션 연출 시작
            StartCoroutine(PickupEffectRoutine(collision.transform));
        }
    }

    private IEnumerator PickupEffectRoutine(Transform playerTransform)
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        transform.SetParent(playerTransform);
        transform.localPosition = new Vector3(0f, 0.2f, 0f);

        Vector3 originalScale = transform.localScale;
        Vector3 startScale = originalScale * 0.5f;
        Vector3 targetScale = originalScale * 1f;

        // [1단계] 1초 등장
        float elapsedTime = 0f;
        float appearTime = 1f;
        while (elapsedTime < appearTime)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / appearTime);
            yield return null;
        }
        transform.localScale = targetScale;

        // [2단계] 1.5초 유지
        yield return new WaitForSeconds(1.5f);

        // [3단계] 1초 스르륵 소멸
        elapsedTime = 0f;
        float disappearTime = 1.0f;
        while (elapsedTime < disappearTime)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, elapsedTime / disappearTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}