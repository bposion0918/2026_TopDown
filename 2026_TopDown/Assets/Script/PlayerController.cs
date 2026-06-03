using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("이동 및 속도")]
    public float moveSpeed = 2f;
    public float frameTime = 0.15f; // 애니메이션 이미지가 바뀌는 속도

    [Header("방향별 애니메이션 스프라이트")]
    public Sprite[] spriteUp;
    public Sprite[] spriteDown;
    public Sprite[] spriteLeft;
    public Sprite[] spriteRight;

    [Header("사망 애니메이션 및 UI")]
    public Sprite[] spriteWaterDeath;  // 물에 빠져 죽었을 때
    public Sprite[] spriteNormalDeath; // 일반 공격으로 죽었을 때
    public GameObject gameOverPanel;

    [Header("오디오 설정")]
    public AudioSource bgmAudioSource;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 input;
    private Vector2 velocity;

    private Sprite[] currentSprites;
    private int frameIndex = 0;
    private float timer = 0f;

    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        currentSprites = spriteDown;
        sr.sprite = currentSprites[0];
    }

    public void OnMove(InputValue value)
    {
        if (isDead) return;

        input = value.Get<Vector2>();
        velocity = input.normalized * moveSpeed;

        if (input.sqrMagnitude > 0.01f)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                if (input.x > 0) ChangeSprites(spriteRight);
                else ChangeSprites(spriteLeft);
            }
            else
            {
                if (input.y > 0) ChangeSprites(spriteUp);
                else ChangeSprites(spriteDown);
            }
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (input.sqrMagnitude <= 0.01f)
        {
            frameIndex = 0;
            sr.sprite = currentSprites[frameIndex];
            return;
        }

        timer += Time.deltaTime;
        if (timer >= frameTime)
        {
            timer = 0f;
            frameIndex++;

            if (frameIndex >= currentSprites.Length)
                frameIndex = 0;

            sr.sprite = currentSprites[frameIndex];
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private void ChangeSprites(Sprite[] newSprites)
    {
        if (currentSprites == newSprites) return;

        currentSprites = newSprites;
        frameIndex = 0;
        timer = 0f;
        sr.sprite = currentSprites[frameIndex];
    }

    // ==============================================================
    // 외부(PlayerHealth)에서 호출할 사망 트리거 함수들
    // ==============================================================
    public void PlayWaterDeathAnimation()
    {
        if (!isDead) StartCoroutine(DeathSequence(true));
    }

    public void PlayNormalDeathAnimation()
    {
        if (!isDead) StartCoroutine(DeathSequence(false));
    }

    // ==============================================================
    // 사망 연출 시퀀스
    // ==============================================================
    private IEnumerator DeathSequence(bool isWaterDeath)
    {
        isDead = true;
        velocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop();
        }
        else
        {
            AudioSource automaticBgm = GameObject.FindWithTag("BGM")?.GetComponent<AudioSource>();
            if (automaticBgm != null) automaticBgm.Stop();
        }

        // isWaterDeath 여부에 따라 다른 애니메이션을 재생하도록 전달
        StartCoroutine(PlayDeathAnimationRoutine(isWaterDeath));

        yield return StartCoroutine(FadeInGameOverPanel(2f));
        yield return new WaitForSeconds(0.1f);

        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.PlayerDead();
        }
        else
        {
            Debug.LogWarning("씬에 GameDataManager가 없습니다! 강제로 GameOver 씬을 로드합니다.");
            SceneManager.LoadScene("GameOver");
        }
    }

    private IEnumerator PlayDeathAnimationRoutine(bool isWaterDeath)
    {
        int deathFrameIndex = 0;

        // 원인에 따라 사용할 배열을 선택
        Sprite[] targetSprites = isWaterDeath ? spriteWaterDeath : spriteNormalDeath;

        while (isDead)
        {
            if (targetSprites != null && targetSprites.Length > 0)
            {
                sr.sprite = targetSprites[deathFrameIndex];
                deathFrameIndex++;
                if (deathFrameIndex >= targetSprites.Length)
                {
                    deathFrameIndex = 0;
                }
            }
            yield return new WaitForSeconds(frameTime);
        }
    }

    private IEnumerator FadeInGameOverPanel(float fadeDuration)
    {
        if (gameOverPanel == null) yield break;

        gameOverPanel.SetActive(true);
        CanvasGroup canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameOverPanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = elapsedTime / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
}