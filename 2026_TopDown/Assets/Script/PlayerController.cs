using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
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
    public Sprite[] spriteDeath;
    public GameObject gameOverPanel;

    [Header("오디오 설정")]
    public AudioSource bgmAudioSource; // 배경음악을 재생 중인 AudioSource를 여기에 드래그합니다.

    // 내부에서 쓸 부품들 (컴포넌트 및 상태 변수)
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 input;
    private Vector2 velocity;

    private Sprite[] currentSprites; // 현재 재생 중인 방향의 이미지 배열
    private int frameIndex = 0;      // 현재 보여줄 이미지의 순서(인덱스)
    private float timer = 0f;        // 걷는 애니메이션용 타이머

    private Coroutine deathTimerCoroutine; // 1.5초 시한폭탄을 담아둘 상자
    private bool isDead = false;           // 플레이어 사망 여부

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // 시작할 때 정면(아래)을 보게 설정
        currentSprites = spriteDown;
        sr.sprite = currentSprites[0];
    }

    // ==============================================================
    // 1. 이동 입력 및 애니메이션 방향 전환
    // ==============================================================
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

    // ==============================================================
    // 2. 걷기 애니메이션 수동 재생 (매 프레임)
    // ==============================================================
    private void Update()
    {
        if (isDead) return;

        // 움직이지 않으면 첫 번째(서 있는) 이미지로 고정
        if (input.sqrMagnitude <= 0.01f)
        {
            frameIndex = 0;
            sr.sprite = currentSprites[frameIndex];
            return;
        }

        // 지정된 시간(frameTime)마다 다음 이미지로 넘김
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

    // ==============================================================
    // 3. 실제 물리적 이동 처리
    // ==============================================================
    private void FixedUpdate()
    {
        if (isDead) return;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    // 방향이 바뀔 때 애니메이션 배열을 교체해 주는 함수
    private void ChangeSprites(Sprite[] newSprites)
    {
        if (currentSprites == newSprites) return;

        currentSprites = newSprites;
        frameIndex = 0;
        timer = 0f;
        sr.sprite = currentSprites[frameIndex];
    }

    // ==============================================================
    // 4. 함정 타일맵(DeathTilemap) 충돌 및 1.5초 타이머 관리
    // ==============================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn") && !isDead)
        {
            Tilemap map = collision.GetComponent<Tilemap>();
            deathTimerCoroutine = StartCoroutine(DeathTimerRoutine(map));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn") && !isDead)
        {
            // 1.5초가 되기 전에 빠져나오면 코루틴(시한폭탄) 해제
            if (deathTimerCoroutine != null)
            {
                StopCoroutine(deathTimerCoroutine);
                deathTimerCoroutine = null;
            }
        }
    }

    private IEnumerator DeathTimerRoutine(Tilemap map)
    {
        yield return new WaitForSeconds(1.5f); // 1.5초 대기
        StartCoroutine(DeathSequence(map));    // 시간 다 되면 사망 코루틴 시작
    }

    // ==============================================================
    // 5. 사망 연출 (애니메이션 재생 -> UI 페이드인 대기 -> 데이터 매니저 호출)
    // ==============================================================
    private IEnumerator DeathSequence(Tilemap map)
    {
        // 1. 이동 및 물리 조작 완벽히 차단
        isDead = true;
        velocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        // [추가] 죽자마자 BGM 즉시 끄기!
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop();
        }
        else
        {
            // 만약 인스펙터에 깜빡하고 등록 안 했을 때를 대비한 자동 찾기 기능
            AudioSource automaticBgm = GameObject.FindWithTag("BGM")?.GetComponent<AudioSource>();
            if (automaticBgm != null) automaticBgm.Stop();
        }

        // 2. 사망 애니메이션 무한 반복 시작 (백그라운드에서 별도로 돌아감)
        StartCoroutine(PlayDeathAnimationRoutine());

        // 3. UI 페이드인 시작 및 완료될 때까지 '대기' (★ 핵심: yield return)
        // 2초 동안 캔버스가 나타나기를 기다립니다.
        yield return StartCoroutine(FadeInGameOverPanel(2f));

        // 4. UI가 다 나타난 뒤, 0.1초 정도 여운을 주고 GameDataManager 호출
        yield return new WaitForSeconds(0.1f);

        if (GameDataManager.instance != null)
        {
            // 매니저를 불러와서 아이템을 날리고 GameOver 씬으로 이동시킵니다.
            GameDataManager.instance.PlayerDead();
        }
        else
        {
            Debug.LogWarning("씬에 GameDataManager가 없습니다! 강제로 GameOver 씬을 로드합니다.");
            SceneManager.LoadScene("GameOver");
        }
    }

    // 사망 도트 애니메이션만 계속 돌려주는 전용 코루틴
    private IEnumerator PlayDeathAnimationRoutine()
    {
        int deathFrameIndex = 0;

        // 씬이 넘어가기 전까지 계속 사망 스프라이트를 넘겨줍니다.
        while (isDead)
        {
            if (spriteDeath != null && spriteDeath.Length > 0)
            {
                sr.sprite = spriteDeath[deathFrameIndex];
                deathFrameIndex++;
                if (deathFrameIndex >= spriteDeath.Length)
                {
                    deathFrameIndex = 0;
                }
            }
            yield return new WaitForSeconds(frameTime);
        }
    }

    // ==============================================================
    // 6. UI 서서히 나타나기 (Fade In) - 기존과 동일하지만 깔끔하게 유지
    // ==============================================================
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
