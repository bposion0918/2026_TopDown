using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayManage : MonoBehaviour
{
    public GameObject OptionPanel;
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
            OptionPanel.SetActive(!OptionPanel.activeSelf);
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
    public void GameBack()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
