using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("문 설정")]
    public Transform spawnPoint;
    public Animator animator;           // 문 열림/닫힘 애니메이터
    public Collider2D solidCollider;    // 문이 닫혔을 때 플레이어 통과를 막을 단단한 벽
    public bool isLocked = false;

    [HideInInspector] public Door connectedDoor;
    [HideInInspector] public Vector3 cameraTargetPos;

    private void Start()
    {
        // 방에 처음 들어갈 때는 문이 열려있는 상태로 시작합니다.
        if (solidCollider != null) solidCollider.enabled = false;
        if (animator != null) animator.SetBool("isOpen", true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 문이 잠겨있으면 포탈이 작동하지 않음!
        if (isLocked) return;

        if (collision.CompareTag("Player") && connectedDoor != null)
        {
            collision.transform.position = connectedDoor.spawnPoint.position;

            if (CameraController.instance != null)
            {
                CameraController.instance.ChangeRoom(connectedDoor.cameraTargetPos);
            }

            StartCoroutine(FreezeMonstersRoutine());
        }
    }

    private IEnumerator FreezeMonstersRoutine()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                MonoBehaviour ai = (MonoBehaviour)enemy.GetComponent("EnemyAI");
                if (ai != null) ai.enabled = false;

                Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }
        }

        yield return new WaitForSeconds(0.8f);

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                Collider2D col = enemy.GetComponent<Collider2D>();
                if (col != null && col.enabled)
                {
                    MonoBehaviour ai = (MonoBehaviour)enemy.GetComponent("EnemyAI");
                    if (ai != null) ai.enabled = true;
                }
            }
        }
    }

    // 몬스터 스포너가 몬스터를 감지하고 문을 "잠글 때" 부르는 함수
    public void CloseDoor()
    {
        isLocked = true;
        if (animator != null) animator.SetBool("isOpen", false);
        if (solidCollider != null) solidCollider.enabled = true; // 벽 생성 (못 지나감)
    }

    // 몬스터를 다 잡고 문을 "열 때" 부르는 함수
    public void OpenDoor()
    {
        isLocked = false;
        if (animator != null) animator.SetBool("isOpen", true);
        if (solidCollider != null) solidCollider.enabled = false; // 벽 제거 (지나갈 수 있음)
    }
}