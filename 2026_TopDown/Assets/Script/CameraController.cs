using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [Header("카메라 이동 속도")]
    public float speed = 5f;

    private Vector3 targetPosition;
    
    // 흔들림 효과를 위한 내부 변수
    private Vector3 shakeOffset = Vector3.zero;

    void Awake()
    {
        instance = this;
        targetPosition = transform.position;
    }

    void Update()
    {
        // 핵심: 현재 위치에서 흔들림 오프셋을 뺀 값으로 Lerp를 계산해야 카메라가 한쪽으로 밀리지 않습니다.
        Vector3 basePosition = Vector3.Lerp(
            new Vector3(transform.position.x - shakeOffset.x, transform.position.y - shakeOffset.y, transform.position.z), 
            targetPosition, 
            speed * Time.deltaTime
        );
        
        // 최종 카메라 위치 = 부드럽게 이동한 위치 + 흔들림 오프셋
        transform.position = basePosition + shakeOffset;
    }

    public void ChangeRoom(Vector3 newRoomPosition)
    {
        targetPosition = new Vector3(newRoomPosition.x, newRoomPosition.y, transform.position.z);
    }

    // 외부(무기 스크립트 등)에서 호출할 카메라 흔들림 함수
    public void TriggerShake(float duration, float magnitude)
    {
        // 기존에 흔들리고 있던 코루틴이 겹치지 않도록 안전하게 새로 시작합니다.
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 무작위로 -1부터 1 사이의 값에 세기(magnitude)를 곱해 오프셋을 만듭니다.
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            shakeOffset = new Vector3(x, y, 0f);
            
            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 흔들림 시간이 끝나면 오프셋을 원상태로 초기화합니다.
        shakeOffset = Vector3.zero;
    }
}