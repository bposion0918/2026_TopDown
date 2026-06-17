using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject OptionPanel;
    public GameObject MenuPanel;
    public GameObject SoundPanel;
    public GameObject SettingPanel;

    public bool TimeStop = false;

    private void Update()
    {
        // 기존 ESC 일시정지 로직
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (TimeStop)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // [신규] R키를 누르면 데이터 초기화 후 즉시 재시작
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetAndRestartGame();
        }
    }

    public void PauseGame()
    {
        MenuPanel.SetActive(true);
        TimeStop = true;
        Time.timeScale = 0f;
        Debug.Log("게임이 일시정지되었습니다.");
    }

    public void ResumeGame()
    {
        MenuPanel.SetActive(false);

        if (OptionPanel != null) OptionPanel.SetActive(false);
        if (SoundPanel != null) SoundPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);

        TimeStop = false;
        Time.timeScale = 1f;
        Debug.Log("게임이 다시 재개되었습니다.");
    }

    public void OpenOption() { OptionPanel.SetActive(true); }
    public void CloseOption() { OptionPanel.SetActive(false); }
    public void OpenMenu() { PauseGame(); }
    public void CloseMenu() { ResumeGame(); }

    public void GameBack()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level_1");
    }

    public void ToggleSound() { SoundPanel.SetActive(!SoundPanel.activeSelf); }
    public void ToggleSetting() { SettingPanel.SetActive(!SettingPanel.activeSelf); }
    public void ButtonLog() { Debug.Log("BUTTON CLICKED!"); }
    public void QuitGame() { Debug.Log("게임 종료"); Application.Quit(); }

    // [수정됨] 기존 데이터를 지우고 바로 게임을 처음(타이틀)으로 되돌리는 함수
    public void ResetAndRestartGame()
    {
        if (GameDataManager.instance != null)
        {
            // 1. JSON 파일 삭제 및 데이터 리셋
            GameDataManager.instance.ResetData();
        }
        else
        {
            Debug.LogError("GameDataManager를 찾을 수 없습니다.");
        }

        // 2. 만약 게임오버 창이나 일시정지 창에서 눌렀을 경우를 대비해 시간의 흐름을 원상복구
        Time.timeScale = 1f;

        // 3. 타이틀 씬으로 이동하여 완전히 초기 상태로 재시작
        // (만약 타이틀을 거치지 않고 바로 1스테이지로 가고 싶다면 "TitleScene" 대신 "Level_1"을 적으세요)
        SceneManager.LoadScene("TitleScene");

        Debug.Log("데이터가 초기화되고 게임이 재시작되었습니다.");
    }
}