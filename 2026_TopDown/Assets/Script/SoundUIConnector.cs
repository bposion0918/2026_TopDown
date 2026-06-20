using UnityEngine;
using UnityEngine.UI;

public class SoundUIConnector : MonoBehaviour
{
    [Header("이 씬에 있는 슬라이더 연결")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        // 씬이 시작될 때마다 안 죽고 살아있는 AudioManager를 찾아서 세팅합니다.
        if (AudioManager.instance == null) return;

        float savedBgmVol = PlayerPrefs.GetFloat("BgmVolume", 10f);
        float savedSfxVol = PlayerPrefs.GetFloat("SfxVolume", 10f);

        if (bgmSlider != null)
        {
            bgmSlider.value = savedBgmVol;
            bgmSlider.onValueChanged.AddListener(AudioManager.instance.SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSfxVol;
            sfxSlider.onValueChanged.AddListener(AudioManager.instance.SetSFXVolume);
        }
    }

    // 버튼들이 클릭될 때 AudioManager가 끊기는 걸 막기 위한 중간 다리 함수
    public void OnButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonClickSFX();
        }
    }
}