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

    // [신규] 버튼에 연결할 데이터 초기화 함수
    public void ResetGameData()
    {
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.ResetData();
        }
        else
        {
            Debug.LogError("에러: GameDataManager 인스턴스를 찾을 수 없습니다.");
        }
    }
}