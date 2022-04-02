using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuModel : MonoBehaviour
{
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject primaryButtons;
    [SerializeField] GameObject warning;

    public delegate void GameStarted();
    public static event GameStarted gameStarted;

    bool saveExists = false;

    private void Awake()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("StoryIntro", UnityEngine.SceneManagement.LoadSceneMode.Additive);

        saveExists = SaveManager.SaveFileExists();
        continueButton.SetActive(saveExists);
    }

    public void ContinueGame()
    {
        SaveManager.LoadGame();
        if(SaveManager.GetState() > 0)
        {
            DisableButtons();
            SceneManager.FadeToScene("Space");
        }
        else
        {
            CloseMenu();
            gameStarted?.Invoke();
        }
    }

    public void NewGameWarning()
    {
        if (saveExists)
        {
            primaryButtons.SetActive(false);
            warning.SetActive(true);
        }
        else
        {
            CreateNewGame();
        }
    }

    public void CancelNewGame()
    {
        warning.SetActive(false);
        primaryButtons.SetActive(true);
    }

    public void CreateNewGame()
    {
        SaveManager.CreateNewSave();
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
