using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("문 설정")]
    public Transform spawnPoint;
    public Animator animator;
    public Collider2D solidCollider;
    public bool isLocked = false;

    [Header("특수 문 디자인 (애니메이터 컨트롤러)")]
    public RuntimeAnimatorController normalDoorAnim;
    public RuntimeAnimatorController bossDoorAnim;
    public RuntimeAnimatorController shopDoorAnim;
    public RuntimeAnimatorController treasureDoorAnim;

    [HideInInspector] public Door connectedDoor;
    [HideInInspector] public Vector3 cameraTargetPos;

    private void Start()
    {
        if (solidCollider != null) solidCollider.enabled = false;
        if (animator != null) animator.SetBool("isOpen", true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

    public void CloseDoor()
    {
        isLocked = true;
        if (animator != null) animator.SetBool("isOpen", false);
        if (solidCollider != null) solidCollider.enabled = true;
    }

    public void OpenDoor()
    {
        isLocked = false;
        if (animator != null) animator.SetBool("isOpen", true);
        if (solidCollider != null) solidCollider.enabled = false;
    }

    // 연결될 방의 종류를 전달받아 문의 디자인(애니메이터)을 바꾸는 함수
    public void SetDoorAppearance(RoomType targetRoomType)
    {
        if (animator == null) return;

        switch (targetRoomType)
        {
            case RoomType.Boss:
                if (bossDoorAnim != null) animator.runtimeAnimatorController = bossDoorAnim;
                break;
            case RoomType.Shop:
                if (shopDoorAnim != null) animator.runtimeAnimatorController = shopDoorAnim;
                break;
            case RoomType.Treasure:
                if (treasureDoorAnim != null) animator.runtimeAnimatorController = treasureDoorAnim;
                break;
            default: // 시작방이나 노말방은 일반 문 디자인 사용
                if (normalDoorAnim != null) animator.runtimeAnimatorController = normalDoorAnim;
                break;
        }
    }
}