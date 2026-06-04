using UnityEngine;

public class ScreenSettingManager : MonoBehaviour
{
    // 해상도는 게임에 맞게 수정 가능합니다. (현재 1920x1080 기준)
    private int width = 1920;
    private int height = 1080;

    void Start()
    {
        // 게임이 시작될 때 저장된 화면 모드를 불러옵니다. (기본값: 0 = 전체화면)
        int savedScreenMode = PlayerPrefs.GetInt("ScreenMode", 0);
        ApplyScreenMode(savedScreenMode);
    }

    // 1. 전체화면 버튼에 연결할 함수
    public void SetFullScreen()
    {
        ApplyScreenMode(0);
    }

    // 2. 창 화면 버튼에 연결할 함수
    public void SetWindowed()
    {
        ApplyScreenMode(1);
    }

    // 3. 테두리 없는 창 화면 버튼에 연결할 함수
    public void SetBorderlessWindow()
    {
        ApplyScreenMode(2);
    }

    // 실제 화면 모드를 적용하고 저장하는 핵심 함수
    private void ApplyScreenMode(int modeIndex)
    {
        // 씬이 넘어가거나 게임을 재시작해도 유지되도록 PlayerPrefs에 저장
        PlayerPrefs.SetInt("ScreenMode", modeIndex);
        PlayerPrefs.Save();

        switch (modeIndex)
        {
            case 0: // 전체화면 (ExclusiveFullScreen)
                Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
                Debug.Log("전체화면 적용됨");
                break;
            case 1: // 창 화면 (Windowed)
                Screen.SetResolution(width, height, FullScreenMode.Windowed);
                Debug.Log("창 화면 적용됨");
                break;
            case 2: // 테두리 없는 창 화면 (FullScreenWindow)
                Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
                Debug.Log("테두리 없는 창 적용됨");
                break;
        }
    }
}
