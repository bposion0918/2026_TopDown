using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("이동 및 속도 (물리 엔진)")]
    public float maxSpeed = 1f;     // 기본 최고 속도를 1로 변경
    public float frameTime = 0.15f;

    [Header("방향별 애니메이션 스프라이트")]
    public Sprite[] spriteUp;
    public Sprite[] spriteDown;
    public Sprite[] spriteLeft;
    public Sprite[] spriteRight;

    [Header("사망 애니메이션 및 UI")]
    public Sprite[] spriteNormalDeath;
    public GameObject gameOverPanel;

    [Header("오디오 설정")]
    public AudioSource bgmAudioSource;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 input;

    private Sprite[] currentSprites;
    private int frameIndex = 0;
    private float timer = 0f;

    public bool isDead = false;

    [HideInInspector] public Vector2 lastFacingDir = Vector2.down;

    private PlayerStats playerStats; // 스탯 스크립트 연결용

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerStats = GetComponent<PlayerStats>(); // 플레이어 스탯 가져오기

        currentSprites = spriteDown;
        sr.sprite = currentSprites[0];
    }

    public void OnMove(InputValue value)
    {
        if (isDead) return;

        input = value.Get<Vector2>();

        if (input.sqrMagnitude > 0.01f)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                if (input.x > 0)
                {
                    ChangeSprites(spriteRight);
                    lastFacingDir = Vector2.right;
                }
                else
                {
                    ChangeSprites(spriteLeft);
                    lastFacingDir = Vector2.left;
                }
            }
            else
            {
                if (input.y > 0)
                {
                    ChangeSprites(spriteUp);
                    lastFacingDir = Vector2.up;
                }
                else
                {
                    ChangeSprites(spriteDown);
                    lastFacingDir = Vector2.down;
                }
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

        // PlayerStats에서 현재 이동 속도를 가져옴 (없으면 기본값 1 적용)
        float currentMaxSpeed = playerStats != null ? playerStats.currentMoveSpeed : maxSpeed;

        if (input.sqrMagnitude > 0.01f)
        {
            rb.linearVelocity = input.normalized * currentMaxSpeed;
        }
    }

    private void ChangeSprites(Sprite[] newSprites)
    {
        if (currentSprites == newSprites) return;

        currentSprites = newSprites;
        frameIndex = 0;
        timer = 0f;
        sr.sprite = currentSprites[frameIndex];
    }

    public void PlayNormalDeathAnimation()
    {
        if (!isDead) StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        isDead = true;
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

        StartCoroutine(PlayDeathAnimationRoutine());

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

    private IEnumerator PlayDeathAnimationRoutine()
    {
        int deathFrameIndex = 0;
        Sprite[] targetSprites = spriteNormalDeath;

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