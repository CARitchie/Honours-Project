using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuModel : MonoBehaviour
{
    public delegate void GameStarted();
    public static event GameStarted gameStarted;

    private void Awake()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("StoryIntro", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void ContinueGame()
    {
        DisableButtons();
        SceneManager.FadeToScene("Space");
    }

    public void CreateNewGame()
    {
        CloseMenu();
        gameStarted?.Invoke();
    }

    public void OpenSettings()
    {

    }

    public void CloseMenu()
    {
        DisableButtons();

        StartCoroutine(FadeOut());
    }

    public void DisableButtons()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            btn.interactable = false;
        }
    }

    IEnumerator FadeOut()
    {
        Graphic[] graphics = GetComponentsInChildren<Graphic>(true);
        float timeDelay = 0.5f;
        float timer = timeDelay;
        while (timer > 0)
        {
            float lastTimer = timer / timeDelay;
            timer -= Time.deltaTime;

            foreach (Graphic graphic in graphics)
            {
                Color col = graphic.color;
                float origianlAlpha = col.a / lastTimer;
                col.a = Mathf.Lerp(0, origianlAlpha, timer/timeDelay);
                graphic.color = col;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
