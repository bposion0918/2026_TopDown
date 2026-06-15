using UnityEngine;

public class OxygenItem : MonoBehaviour
{
    [Header("산소 회복량 (%)")]
    public float restorePercentage = 20f; // 최대 산소의 20%를 회복

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 플레이어에게 부착된 PlayerOxygen 스크립트를 가져와서 회복 함수를 실행합니다.
            PlayerOxygen playerOxygen = collision.GetComponent<PlayerOxygen>();
            if (playerOxygen != null)
            {
                playerOxygen.AddOxygenByPercentage(restorePercentage);
            }

            // 먹었으니 화면에서 아이템 삭제
            Destroy(gameObject);
        }
    }
}