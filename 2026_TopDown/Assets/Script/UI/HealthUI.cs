using UnityEngine;
using UnityEngine.UI; 

public class HealthUI : MonoBehaviour
{
    [Header("하트 이미지 소스")]
    public Sprite fullHeart;  
    public Sprite halfHeart;  
    public Sprite emptyHeart; 

    [Header("UI 화면에 배치된 하트들")]
    public Image[] heartImages; 

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            int heartValue = currentHealth - (i * 2);

            if (heartValue >= 2)
            {
                heartImages[i].sprite = fullHeart;
            }
            else if (heartValue == 1)
            {
                heartImages[i].sprite = halfHeart;
            }
            else
            {
                heartImages[i].sprite = emptyHeart;
            }
        }
    }
}