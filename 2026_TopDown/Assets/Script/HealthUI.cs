using UnityEngine;
using UnityEngine.UI; // UI를 다루기 위해 꼭 필요합니다!

public class HealthUI : MonoBehaviour
{
    [Header("하트 이미지 소스")]
    public Sprite fullHeart;  // 꽉 찬 하트 이미지
    public Sprite halfHeart;  // 반 칸 하트 이미지
    public Sprite emptyHeart; // 빈 하트 이미지

    [Header("UI 화면에 배치된 하트들")]
    public Image[] heartImages; // Hierarchy에 만든 하트 이미지들을 넣을 배열

    // 체력이 변할 때마다 이 함수를 호출해주세요!
    // currentHealth: 현재 체력, maxHearts: 화면에 보여줄 최대 하트 개수
    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            // 이 하트가 담당하는 체력 수치를 계산합니다. (하트 하나당 체력 2씩 담당)
            // i=0(첫번째 하트)은 체력 1, 2를 담당
            // i=1(두번째 하트)은 체력 3, 4를 담당
            int heartValue = currentHealth - (i * 2);

            if (heartValue >= 2)
            {
                // 체력이 2 이상 남았다면 꽉 찬 하트
                heartImages[i].sprite = fullHeart;
            }
            else if (heartValue == 1)
            {
                // 체력이 딱 1 남았다면 반 칸 하트
                heartImages[i].sprite = halfHeart;
            }
            else
            {
                // 그 이하라면 빈 하트
                heartImages[i].sprite = emptyHeart;
            }
        }
    }
}