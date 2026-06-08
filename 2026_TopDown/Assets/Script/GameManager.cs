using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject OptionPanel;
    public GameObject MenuPanel;
    public GameObject SoundPanel;
    public GameObject SettingPanel;

    // 외부 스크립트에서도 현재 멈춤 상태를 쉽게 읽을 수 있도록 public 유지
    public bool TimeStop = false;

    private void Update()
    {
        // 1. ESC 키를 누르면 현재 TimeStop 상태에 따라 알맞은 함수를 실행합니다.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (TimeStop)
            {
                ResumeGame(); // 멈춘 상태면 게임 재개
            }
            else
            {
                PauseGame();  // 흘러가는 상태면 일시정지
            }
        }
    }

    //  [핵심 기능 1] 게임을 일시정지하는 단 하나의 창구
    public void PauseGame()
    {
        MenuPanel.SetActive(true);
        TimeStop = true;
        Time.timeScale = 0f;
        Debug.Log("게임이 일시정지되었습니다.");
    }

    //  [핵심 기능 2] 게임을 다시 시작(재개)하는 단 하나의 창구 (START 버튼도 이걸 호출!)
    public void ResumeGame()
    {
        MenuPanel.SetActive(false);

        // 메뉴를 닫을 때 열려있던 옵션, 사운드, 세팅 패널도 함께 안전하게 닫아줍니다.
        if (OptionPanel != null) OptionPanel.SetActive(false);
        if (SoundPanel != null) SoundPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);

        TimeStop = false;
        Time.timeScale = 1f; // 시간을 다시 정상으로!
        Debug.Log("게임이 다시 재개되었습니다.");
    }

    // 기존 UI 열고 닫는 함수들을 새로 만든 깔끔한 함수들과 연결해줍니다.
    public void OpenOption() { OptionPanel.SetActive(true); }
    public void CloseOption() { OptionPanel.SetActive(false); }
    public void OpenMenu() { PauseGame(); }
    public void CloseMenu() { ResumeGame(); } //  START 버튼이나 닫기 버튼에 연결될 함수

    public void GameBack()
    {
        Time.timeScale = 1f; //  중요: 시간이 멈춘 상태(0)에서 씬을 이동하면 다음 씬도 멈추므로 원상복구 필수!
        SceneManager.LoadScene("TitleScene");
    }

    public void Retry()
    {
        Time.timeScale = 1f; //  중요: 일시정지 중 재시작할 때도 시간을 다시 흘러가게 해야 합니다.
        SceneManager.LoadScene("Level_1");
    }

    public void ToggleSound() { SoundPanel.SetActive(!SoundPanel.activeSelf); }
    public void ToggleSetting() { SettingPanel.SetActive(!SettingPanel.activeSelf); }
    public void ButtonLog() { Debug.Log("BUTTON CLICKED!"); }
    public void QuitGame() { Debug.Log("게임 종료"); Application.Quit(); }
}