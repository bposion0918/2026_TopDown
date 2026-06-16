using System.Collections;
using UnityEngine;

public class OxygenItem : MonoBehaviour
{
    [Header("산소 회복량 (%)")]
    public float restorePercentage = 20f;

    private bool isPickedUp = false;
    private Vector3 targetOriginalScale;

    private void Awake()
    {
        targetOriginalScale = transform.localScale;
    }

    private void Start()
    {
        StartCoroutine(SpawnAnimationRoutine());
    }

    private IEnumerator SpawnAnimationRoutine()
    {
        transform.localScale = Vector3.zero;

        float spawnDuration = 0.4f;
        float elapsedTime = 0f;

        while (elapsedTime < spawnDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, targetOriginalScale, elapsedTime / spawnDuration);
            yield return null;
        }

        transform.localScale = targetOriginalScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPickedUp && collision.CompareTag("Player"))
        {
            isPickedUp = true;

            PlayerOxygen playerOxygen = collision.GetComponent<PlayerOxygen>();
            if (playerOxygen != null)
            {
                playerOxygen.AddOxygenByPercentage(restorePercentage);
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

        Vector3 startScale = targetOriginalScale * 0.5f;
        Vector3 popScale = targetOriginalScale * 1.2f;

        // [1단계] 등장
        float elapsedTime = 0f;
        float appearTime = 0.5f;
        while (elapsedTime < appearTime)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, popScale, elapsedTime / appearTime);
            yield return null;
        }
        transform.localScale = popScale;

        // [2단계] 유지
        yield return new WaitForSeconds(1.5f);

        // [3단계] 소멸
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