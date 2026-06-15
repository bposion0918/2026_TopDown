using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerVision : MonoBehaviour
{
    [Header("조명 연결")]
    public Light2D globalLight;
    public Light2D surroundLight;

    [Header("시야 제한 설정")]
    public float visionDropPercentage = 0.2f;
    public float darkGlobalIntensity = 0.05f;
    public float normalGlobalIntensity = 1.0f;

    [Header("조명 전환 속도")]
    public float fadeSpeed = 3f;
    public float rotationSpeed = 15f;

    private PlayerOxygen playerOxygen;
    private PlayerController playerController;

    void Start()
    {
        playerOxygen = GetComponent<PlayerOxygen>();
        playerController = GetComponent<PlayerController>();

        if (surroundLight != null) surroundLight.intensity = 0f;
    }

    void Update()
    {
        if (playerOxygen == null || playerController == null || globalLight == null) return;

        float oxygenPercent = playerOxygen.currentOxygen / playerOxygen.maxOxygen;

        if (oxygenPercent <= visionDropPercentage)
        {
            // 산소 부족: 맵은 어둡게, 개인 조명은 켜기
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, darkGlobalIntensity, Time.deltaTime * fadeSpeed);

            if (surroundLight != null)
                surroundLight.intensity = Mathf.Lerp(surroundLight.intensity, 1f, Time.deltaTime * fadeSpeed);
        }
        else
        {
            // 산소 충분: 맵은 밝게, 개인 조명은 끄기
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, normalGlobalIntensity, Time.deltaTime * fadeSpeed);

            if (surroundLight != null)
                surroundLight.intensity = Mathf.Lerp(surroundLight.intensity, 0f, Time.deltaTime * fadeSpeed);

        }
    }

    private void RotateSpotLight()
    {
        Vector2 aimDir = playerController.lastFacingDir;

        // 방향 벡터가 0일 때의 오류 방지
        if (aimDir.sqrMagnitude > 0.01f)
        {
            // -90f는 스프라이트의 기본 기준 방향(Right/Up)에 따라 달라질 수 있습니다.
            // 빛이 캐릭터가 보는 방향과 수직으로 돈다면 이 값을 0, 90, 180 등으로 수정해보세요.
            float targetAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        }
    }
}