using UnityEngine;
using System.Collections; 

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 6;       
    public int currentHealth;

    [Header("물 데미지 설정")]
    public float waterDamageInterval = 1f; 

    private float nextWaterDamageTime = 0f;
    private bool isInWater = false;

    private bool isDead = false;

    private PlayerController playerController;
    public HealthUI healthUI;

    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;

    [Header("피격 효과 설정")]
    public Color damageColor = Color.red;   
    public float flashDuration = 0.1f;       

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }
        else
        {
            Debug.LogError(" PlayerHealth 스크립트에 HealthUI가 연결되지 않았습니다!");
        }
    }

    void Update()
    {
        if (isDead) return;

        if (isInWater && Time.time >= nextWaterDamageTime)
        {
            TakeDamage(1, true);

            nextWaterDamageTime = Time.time + waterDamageInterval;
        }
    }

    public void TakeDamage(int damage, bool isWaterDamage = false)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"현재 체력: {currentHealth} / {maxHealth}");

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        if (currentHealth > 0)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRedRoutine());

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayHitSFX();
            }
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die(isWaterDamage);
        }
    }

    private IEnumerator FlashRedRoutine()
    {
        spriteRenderer.color = damageColor;

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = Color.white;
    }

    private void Die(bool isWaterDeath)
    {
        isDead = true;

        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (isWaterDeath)
        {
            Debug.Log("물에서 체력이 다 닳음 -> 물 사망 애니메이션");
            playerController.PlayWaterDeathAnimation();
        }
        else
        {
            Debug.Log("일반 공격으로 체력이 다 닳음 -> 일반 사망 애니메이션");
            playerController.PlayNormalDeathAnimation();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            isInWater = true;
            playerController.SetInWaterState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            isInWater = false;
            playerController.SetInWaterState(false);
        }
    }

}