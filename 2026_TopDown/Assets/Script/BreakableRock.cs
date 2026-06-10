using UnityEngine;

public class BreakableRock : MonoBehaviour
{
    [Header("Rock Settings")]
    public int maxHealth = 2;
    private int currentHealth;

    [Header("State Sprites")]
    public Sprite state2Sprite; // HP 2 (Normal)
    public Sprite state1Sprite; // HP 1 (Cracked)
    public Sprite state0Sprite; // HP 0 (Destroyed rubble)

    private SpriteRenderer sr;
    private BoxCollider2D col;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();

        UpdateRockState();
    }

    // PlayerWeapon에서 풀차지 공격일 때만 이 함수를 호출할 예정입니다.
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        UpdateRockState();
    }

    private void UpdateRockState()
    {
        if (currentHealth == 2)
        {
            if (state2Sprite != null) sr.sprite = state2Sprite;
        }
        else if (currentHealth == 1)
        {
            if (state1Sprite != null) sr.sprite = state1Sprite;
        }
        else if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (state0Sprite != null) sr.sprite = state0Sprite;

            // HP가 0이 되면 플레이어가 밟고 지나갈 수 있도록 충돌체를 끕니다.
            if (col != null)
            {
                col.enabled = false;
            }
        }
    }
}