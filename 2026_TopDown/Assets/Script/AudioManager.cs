using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("오디오 소스 연결")]
    public AudioSource bgmSource; // 1번째 AudioSource (BGM용)
    public AudioSource sfxSource; // 2번째 AudioSource (SFX용)

    [Header("슬라이더 UI 연결")]
    public Slider bgmSlider;      // BGM 볼륨 슬라이더
    public Slider sfxSlider;      // SFX 볼륨 슬라이더

    [Header("효과음 파일")]
    public AudioClip buttonClickClip; // 버튼 누를 때 날 효과음 오디오 클립

    void Start()
    {
        // BGM 슬라이더 세팅
        if (bgmSource != null && bgmSlider != null)
        {
            bgmSlider.value = bgmSource.volume * 10f ;
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        // SFX 슬라이더 세팅
        if (sfxSource != null && sfxSlider != null)
            sfxSlider.value = sfxSource.volume * 10f ;
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }


    // BGM 볼륨 조절 함수
    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null) bgmSource.volume = volume * 0.1f;
    }

    // SFX 볼륨 조절 함수
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume * 0.1f;
    }

    // ★ 버튼이 클릭될 때 호출할 효과음 재생 함수
    public void PlayButtonClickSFX()
    {
        if (sfxSource != null && buttonClickClip != null)
        {
            // PlayOneShot은 소리가 겹치더라도 끊기지 않고 중첩해서 자연스럽게 재생해줍니다.
            sfxSource.PlayOneShot(buttonClickClip);
        }
    }
}