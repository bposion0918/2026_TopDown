using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("이동 및 추적 설정")]
    public float moveSpeed = 1.5f;
    public float detectionRange = 15f;

    [Header("지능형 장애물 회피 (레이더 스캔)")]
    public LayerMask obstacleLayer;
    public float avoidDistance = 0.8f;   // 앞을 내다보는 사거리 (바위를 감지할 거리)
    public float castRadius = 0.2f;      // 몬스터 몸통 두께 (콜라이더보다 약간 작게 설정)

    [Header("몬스터 겹침 방지 (밀어내기)")]
    public LayerMask enemyLayer;
    public float separationRadius = 0.6f;
    public float separationPower = 0.5f;

    [Header("애니메이션 설정")]
    public Sprite[] moveSprites;
    public float frameTime = 0.15f;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 currentMoveDir;
    private float lastDodgeSign = 1f;    // 우회 방향 기억 변수 (1 = 우측, -1 = 좌측)

    private int frameIndex = 0;
    private float timer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

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
            Vector2 dirToPlayer = (player.position - transform.position).normalized;
            Vector2 bestDir = dirToPlayer;
            bool pathFound = false;

            // 1. 레이더 스캔: 정면(0도)부터 90도까지 각도를 넓혀가며 뚫린 길을 찾습니다.
            float[] scanAngles = { 0f, 15f, 30f, 45f, 60f, 75f, 90f };

            foreach (float angle in scanAngles)
            {
                // 이전에 피했던 방향(좌 또는 우)을 우선적으로 검사합니다.
                Vector2 checkDir1 = Quaternion.Euler(0, 0, angle * lastDodgeSign) * dirToPlayer;
                RaycastHit2D hit1 = Physics2D.CircleCast(transform.position, castRadius, checkDir1, avoidDistance, obstacleLayer);

                if (hit1.collider == null)
                {
                    bestDir = checkDir1;
                    pathFound = true;
                    // 방향을 틀었다면 그 방향(부호)을 기억합니다.
                    if (angle != 0f) lastDodgeSign = Mathf.Sign(angle * lastDodgeSign);
                    break;
                }

                // 우선했던 방향이 막혔다면 반대쪽 방향도 검사합니다.
                if (angle != 0f)
                {
                    Vector2 checkDir2 = Quaternion.Euler(0, 0, -angle * lastDodgeSign) * dirToPlayer;
                    RaycastHit2D hit2 = Physics2D.CircleCast(transform.position, castRadius, checkDir2, avoidDistance, obstacleLayer);

                    if (hit2.collider == null)
                    {
                        bestDir = checkDir2;
                        pathFound = true;
                        // 반대쪽이 뚫렸으므로 기억하는 우회 방향을 반대로 뒤집습니다.
                        lastDodgeSign = Mathf.Sign(-angle * lastDodgeSign);
                        break;
                    }
                }
            }

            // 사방이 완전히 막힌 최악의 경우, 제자리에 멈추지 않고 억지로라도 플레이어 쪽으로 밀어붙입니다.
            if (!pathFound)
            {
                bestDir = dirToPlayer;
            }

            // 2. 몬스터 겹침 방지 연산
            Vector2 separationForce = Vector2.zero;
            Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, separationRadius, enemyLayer);

            foreach (Collider2D enemy in nearbyEnemies)
            {
                if (enemy.gameObject != gameObject)
                {
                    Vector2 pushDir = transform.position - enemy.transform.position;
                    float dist = pushDir.magnitude;
                    if (dist > 0)
                    {
                        separationForce += pushDir.normalized / dist;
                    }
                }
            }

            bestDir += separationForce * separationPower;
            bestDir.Normalize();

            // 3. 부드러운 방향 전환 (Lerp)
            currentMoveDir = Vector2.Lerp(currentMoveDir, bestDir, Time.deltaTime * 8f).normalized;

            // 4. 애니메이션 좌우 반전 처리
            if (currentMoveDir.x < -0.1f)
            {
                sr.flipX = true;
            }
            else if (currentMoveDir.x > 0.1f)
            {
                sr.flipX = false;
            }

            AnimateSlime();
        }
        else
        {
            currentMoveDir = Vector2.zero;

            if (moveSprites != null && moveSprites.Length > 0)
            {
                sr.sprite = moveSprites[0];
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = currentMoveDir * moveSpeed;
    }

    private void AnimateSlime()
    {
        if (moveSprites == null || moveSprites.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameTime)
        {
            timer = 0f;
            frameIndex++;

            if (frameIndex >= moveSprites.Length)
            {
                frameIndex = 0;
            }

            sr.sprite = moveSprites[frameIndex];
        }
    }
}