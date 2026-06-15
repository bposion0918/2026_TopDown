using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerVision : MonoBehaviour
{
    [Header("조명 연결")]
    public Light2D globalLight;
    public Light2D surroundLight;

    [Header("시야 제한 설정 (20% 이하)")]
    public float visionDropPercentage = 0.2f;
    public float darkGlobalIntensity = 0.05f;   // 20%일 때의 완전한 어둠

    [Header("시야 경고 설정 (50% 이하)")]
    public float warningDropPercentage = 0.5f;
    public float warningGlobalIntensity = 0.5f; // 50%일 때 어두워지는 최대치 (0.05보다 밝음)
    public float warningPulseSpeed = 1.5f;      // 밝아졌다 어두워지는 깜빡임 속도

    [Header("기본 설정")]
    public float normalGlobalIntensity = 1.0f;
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

        // 1단계: 산소가 20% 이하일 때 (완전한 어둠 + 손전등)
        if (oxygenPercent <= visionDropPercentage)
        {
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, darkGlobalIntensity, Time.deltaTime * fadeSpeed);

            if (surroundLight != null)
                surroundLight.intensity = Mathf.Lerp(surroundLight.intensity, 1f, Time.deltaTime * fadeSpeed);
        }
        // 2단계: 산소가 50% 이하일 때 (어두워졌다 밝아졌다를 반복)
        else if (oxygenPercent <= warningDropPercentage)
        {
            // Time.time과 PingPong을 이용해 0과 1 사이를 부드럽게 왕복하는 값을 만듭니다.
            float pulseValue = Mathf.PingPong(Time.time * warningPulseSpeed, 1f);

            // warningGlobalIntensity(0.5)와 normalGlobalIntensity(1.0) 사이를 왕복합니다.
            float targetIntensity = Mathf.Lerp(warningGlobalIntensity, normalGlobalIntensity, pulseValue);

            globalLight.intensity = Mathf.Lerp(globalLight.intensity, targetIntensity, Time.deltaTime * fadeSpeed);

            if (surroundLight != null)
                surroundLight.intensity = Mathf.Lerp(surroundLight.intensity, 0f, Time.deltaTime * fadeSpeed);
        }
        // 3단계: 산소가 50%를 초과할 때 (정상 밝기)
        else
        {
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, normalGlobalIntensity, Time.deltaTime * fadeSpeed);

            if (surroundLight != null)
                surroundLight.intensity = Mathf.Lerp(surroundLight.intensity, 0f, Time.deltaTime * fadeSpeed);
        }
    }
}