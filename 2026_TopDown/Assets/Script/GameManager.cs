using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject OptionPanel;
    public GameObject MenuPanel;
    public GameObject SoundPanel;
    public GameObject SettingPanel;
    public void ButtonLog()
    {
        Debug.Log("BUTTON CLICKED!");
    }
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuPanel.SetActive(!MenuPanel.activeSelf);
        }
    }
    public void OpenOption()
    {
        OptionPanel.SetActive(true);
    }
    public void CloseOption()
    {
        OptionPanel.SetActive(false);
    }
    public void OpenMenu()
    {
        MenuPanel.SetActive(true);
    }
    public void CloseMenu()
    {
        MenuPanel.SetActive(false);
    }

    public void GameBack()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void ToggleSound()
    {
        SoundPanel.SetActive(!SoundPanel.activeSelf);
    }

    public void ToggleSetting()
    {
        SettingPanel.SetActive(!SettingPanel.activeSelf);
    }

    public void Retry()
    {
        SceneManager.LoadScene("Level_1");
    }
}
