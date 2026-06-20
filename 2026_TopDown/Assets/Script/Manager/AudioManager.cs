using UnityEngine;
using UnityEngine.SceneManagement; // 씬(Scene) 관리를 위해 추가

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("오디오 소스 연결")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    // (주의!) 이제 여기서 슬라이더를 연결하지 않습니다. 에러 방지를 위해 제거했습니다.

    [Header("시스템 효과음")]
    public AudioClip buttonClickClip;
    public AudioClip playerHitClip;

    [Header("공격 효과음 3종 + 경고음")]
    public AudioClip normalAttackClip;
    public AudioClip chargedAttackClip;
    public AudioClip chargeReadyClip;
    public AudioClip warningClip;

    [Header("배경음악 목록 (0:타이틀, 1:인게임)")]
    public AudioClip[] bgmList;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 파괴 방지
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로드될 때마다 이벤트 발생
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // 시작할 때 저장된 볼륨값만 오디오 소스에 바로 적용합니다.
        float savedBgmVol = PlayerPrefs.GetFloat("BgmVolume", 10f);
        float savedSfxVol = PlayerPrefs.GetFloat("SfxVolume", 10f);

        SetBGMVolume(savedBgmVol);
        SetSFXVolume(savedSfxVol);
    }

    // [추가됨] 씬이 바뀔 때마다 자동으로 실행되는 함수 (BGM 교체)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 주의: "TitleScene", "Level_1" 등은 본인 게임의 실제 씬 이름과 똑같이 맞춰야 합니다!
        if (scene.name == "TitleScene")
        {
            PlayBGM(0); // 0번에 넣은 타이틀 BGM 재생
        }
        else if (scene.name == "Level_1")
        {
            PlayBGM(1); // 1번에 넣은 인게임 BGM 재생
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
            // 이미 같은 브금이 틀어져 있으면 다시 처음부터 틀지 않음 (자연스러운 유지)
            if (bgmSource.clip == bgmList[bgmIndex] && bgmSource.isPlaying) return;

            bgmSource.clip = bgmList[bgmIndex];
            bgmSource.Play();
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayButtonClickSFX() => PlaySFX(buttonClickClip);
    public void PlayHitSFX() => PlaySFX(playerHitClip);
    public void PlayNormalAttack() => PlaySFX(normalAttackClip);
    public void PlayChargedAttack() => PlaySFX(chargedAttackClip);
    public void PlayChargeReady() => PlaySFX(chargeReadyClip);
    public void PlayWarningSound() => PlaySFX(warningClip);
}