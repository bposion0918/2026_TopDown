using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    [Header("오디오 소스 연결")]
    public AudioSource bgmSource; // 1번째 AudioSource (BGM용)
    public AudioSource sfxSource; // 2번째 AudioSource (SFX용)

    [Header("슬라이더 UI 연결")]
    public Slider bgmSlider;      // BGM 볼륨 슬라이더
    public Slider sfxSlider;      // SFX 볼륨 슬라이더

    [Header("효과음 파일")]
    public AudioClip buttonClickClip; // 버튼 누를 때 날 효과음 오디오 클립
    // ★ [추가] 피격 시 재생할 오디오 클립
    public AudioClip playerHitClip;

    void Awake()
    {
        // ★ [추가] 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // 1. 기기에 저장된 볼륨 값 불러오기 (저장된 기록이 없다면 기본값 10f 적용)
        float savedBgmVol = PlayerPrefs.GetFloat("BgmVolume", 10f);
        float savedSfxVol = PlayerPrefs.GetFloat("SfxVolume", 10f);

        // 2. 실제 오디오 소스에 불러온 볼륨 적용 (0.1을 곱해 0~1 사이 값으로 변환)
        if (bgmSource != null) bgmSource.volume = savedBgmVol * 0.1f;
        if (sfxSource != null) sfxSource.volume = savedSfxVol * 0.1f;

        // 3. 슬라이더 UI 막대 위치도 저장된 값으로 동기화 후 이벤트 연결
        if (bgmSlider != null)
        {
            bgmSlider.value = savedBgmVol;
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSfxVol;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    // BGM 볼륨 조절 함수
    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null) bgmSource.volume = volume * 0.1f;

        // ★ 슬라이더를 움직일 때마다 그 값을 기기에 영구 저장!
        PlayerPrefs.SetFloat("BgmVolume", volume);
    }

    // SFX 볼륨 조절 함수
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume * 0.1f;

        // ★ 슬라이더를 움직일 때마다 그 값을 기기에 영구 저장!
        PlayerPrefs.SetFloat("SfxVolume", volume);
    }

    // 버튼이 클릭될 때 호출할 효과음 재생 함수
    public void PlayButtonClickSFX()
    {
        if (sfxSource != null && buttonClickClip != null)
        {
            // PlayOneShot은 소리가 겹치더라도 끊기지 않고 중첩해서 자연스럽게 재생해줍니다.
            sfxSource.PlayOneShot(buttonClickClip);
        }
    }

    public void PlayHitSFX()
    {
        if (sfxSource != null && playerHitClip != null)
        {
            sfxSource.PlayOneShot(playerHitClip);
        }
    }
}