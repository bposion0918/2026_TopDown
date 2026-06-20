using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("오디오 소스 연결")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("슬라이더 UI 연결")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("시스템 효과음")]
    public AudioClip buttonClickClip;
    public AudioClip playerHitClip;

    [Header("공격 효과음 3종")]
    public AudioClip normalAttackClip;   // 1. 일반 평타
    public AudioClip chargedAttackClip;  // 2. 풀차지 공격
    public AudioClip chargeReadyClip;    // 3. 기 모으는 소리 (또는 타격음)

    [Header("배경음악 목록")]
    public AudioClip[] bgmList; // 여러 브금을 담아둘 공간

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // 씬을 넘어가도 배경음악이 끊기지 않게 유지해 줍니다.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        float savedBgmVol = PlayerPrefs.GetFloat("BgmVolume", 10f);
        float savedSfxVol = PlayerPrefs.GetFloat("SfxVolume", 10f);

        SetBGMVolume(savedBgmVol);
        SetSFXVolume(savedSfxVol);

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

    // --- 볼륨 조절 (이 함수 하나로 전체 사운드가 조절됩니다) ---
    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null) bgmSource.volume = volume * 0.1f;
        PlayerPrefs.SetFloat("BgmVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume * 0.1f;
        PlayerPrefs.SetFloat("SfxVolume", volume);
    }

    // --- BGM 재생 ---
    public void PlayBGM(int bgmIndex)
    {
        // 배열 범위 에러 방지
        if (bgmSource != null && bgmList.Length > 0 && bgmIndex < bgmList.Length)
        {
            bgmSource.clip = bgmList[bgmIndex];
            bgmSource.Play();
        }
    }

    // --- SFX 재생 공통 도우미 ---
    // 여러 개의 오디오 클립을 한 소스에서 겹치게 재생해 줍니다.
    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // --- 개별 효과음 실행 ---
    public void PlayButtonClickSFX() => PlaySFX(buttonClickClip);
    public void PlayHitSFX() => PlaySFX(playerHitClip);
    public void PlayNormalAttack() => PlaySFX(normalAttackClip);
    public void PlayChargedAttack() => PlaySFX(chargedAttackClip);
    public void PlayChargeReady() => PlaySFX(chargeReadyClip);
}