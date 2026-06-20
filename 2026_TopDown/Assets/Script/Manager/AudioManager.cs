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

    [Header("공격 효과음 3종 + 경고음")]
    public AudioClip normalAttackClip;
    public AudioClip chargedAttackClip;
    public AudioClip chargeReadyClip;
    public AudioClip warningClip;        // [추가됨] 한계 초과 경고음

    [Header("배경음악 목록")]
    public AudioClip[] bgmList;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
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

    public void PlayBGM(int bgmIndex)
    {
        if (bgmSource != null && bgmList.Length > 0 && bgmIndex < bgmList.Length)
        {
            bgmSource.clip = bgmList[bgmIndex];
            bgmSource.Play();
        }
    }

    // --- SFX 재생 공통 도우미 ---
    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // 아래 부분을 이렇게 전통적인 괄호 형태로 바꿔주세요!
    public void PlayButtonClickSFX()
    {
        PlaySFX(buttonClickClip);
    }

    public void PlayHitSFX()
    {
        PlaySFX(playerHitClip);
    }

    public void PlayNormalAttack()
    {
        PlaySFX(normalAttackClip);
    }

    public void PlayChargedAttack()
    {
        PlaySFX(chargedAttackClip);
    }

    public void PlayChargeReady()
    {
        PlaySFX(chargeReadyClip);
    }

    public void PlayWarningSound()
    {
        PlaySFX(warningClip);
    }
}