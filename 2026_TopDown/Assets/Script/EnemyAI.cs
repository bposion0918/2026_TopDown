using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("이동 및 추적 설정")]
    public float moveSpeed = 1.5f;       // 슬라임의 이동 속도
    public float detectionRange = 15f;   // 플레이어 감지 범위

    [Header("애니메이션 설정")]
    public Sprite[] moveSprites;         //  움직일 때 반복할 슬라임 이미지들
    public float frameTime = 0.15f;      //  이미지가 바뀌는 속도

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;           // 좌우 반전(flip)과 이미지를 바꿀 컴포넌트
    private Vector2 movement;

    private int frameIndex = 0;
    private float timer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>(); // 시작할 때 SpriteRenderer를 찾아옵니다.

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            movement = direction;

            // 1. 방향 바라보기 (좌우 반전)
            if (movement.x < 0)
            {
                sr.flipX = true;  // 플레이어가 왼쪽에 있으면 이미지 좌우 뒤집기
            }
            else if (movement.x > 0)
            {
                sr.flipX = false; // 플레이어가 오른쪽에 있으면 원본 방향 보기
            }

            // 2. 움직일 때 애니메이션 재생
            AnimateSlime();
        }
        else
        {
            movement = Vector2.zero;

            // 플레이어를 놓쳐서 멈춰있을 때는 기본(첫 번째) 모습으로 돌아갑니다.
            if (moveSprites != null && moveSprites.Length > 0)
            {
                sr.sprite = moveSprites[0];
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }

    // 슬라임의 이미지를 순서대로 반복해서 바꿔주는 함수
    private void AnimateSlime()
    {
        if (moveSprites == null || moveSprites.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameTime)
        {
            timer = 0f;
            frameIndex++;

            // 준비된 이미지를 다 보여줬으면 다시 처음(0번)으로 돌아갑니다.
            if (frameIndex >= moveSprites.Length)
            {
                frameIndex = 0;
            }

            sr.sprite = moveSprites[frameIndex];
        }
    }
}