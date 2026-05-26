using Unity.VectorGraphics;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public GameObject OptionPanel;
    public void ButtonLog()
    {
        Debug.Log("BUTTON CLICKED!");
    }

    public void GameStart()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void OpenOption()
    {
        OptionPanel.SetActive(true);
    }
    public void CloseOption()
    {
        OptionPanel.SetActive(false);
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

    // 에디터에서 CanvasGroup을 드래그 앤 드롭으로 연결해줍니다.
    public CanvasGroup targetCanvasGroup;
    // 서서히 나타나는 데 걸릴 시간 (초)
    public float fadeDuration = 2.0f;

    void Start()
    {
        // 게임 시작 시 코루틴 실행
        StartCoroutine(FadeInUI());
    }

    IEnumerator FadeInUI()
    {
        float currentTime = 0f;

        // 알파값이 1이 될 때까지 반복
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            // 시간에 따라 알파값을 0에서 1로 선형 보간(Lerp)합니다.
            targetCanvasGroup.alpha = Mathf.Lerp(0f, 1f, currentTime / fadeDuration);
            yield return null; // 다음 프레임까지 대기
        }

        // 확실하게 최종 값인 1로 고정
        targetCanvasGroup.alpha = 1f;
    }

}
