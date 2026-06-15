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

    [Header("효과음 파일")]
    public AudioClip buttonClickClip; 
    public AudioClip playerHitClip;

    void Awake()
    {
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
        
        float savedBgmVol = PlayerPrefs.GetFloat("BgmVolume", 10f);
        float savedSfxVol = PlayerPrefs.GetFloat("SfxVolume", 10f);

        if (bgmSource != null) bgmSource.volume = savedBgmVol * 0.1f;
        if (sfxSource != null) sfxSource.volume = savedSfxVol * 0.1f;

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

    public void PlayButtonClickSFX()
    {
        if (sfxSource != null && buttonClickClip != null)
        {
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