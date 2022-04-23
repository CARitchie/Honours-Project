using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuModel : MonoBehaviour
{
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject primaryButtons;
    [SerializeField] GameObject warning;
    [SerializeField] OptionsMenu options;

    public delegate void GameStarted();
    public static event GameStarted gameStarted;

    bool saveExists = false;

    private void Awake()
    {
        Time.timeScale = 1;             // Reset the timescale
        UnityEngine.SceneManagement.SceneManager.LoadScene("StoryIntro", UnityEngine.SceneManagement.LoadSceneMode.Additive);       // Load in the scene with the Arturius flying real fast
        Cursor.visible = true;                              // Make the cursor visible
        Cursor.lockState = CursorLockMode.None;             // Unlock cursor movement

        saveExists = SaveManager.SaveFileExists();          // Find out if a save file exists
        continueButton.SetActive(saveExists);               // Enable or disable the continue button depending upon the existance of a save file
    }

    // Function to continue playing from an existing save file
    public void ContinueGame()
    {
        SaveManager.LoadGame();
        if(SaveManager.GetState() > 0)                      // If the intro cutscene has already been played
        {
            DisableButtons();
            SceneManager.FadeToScene("Space");              // Load into the main game scene
        }
        else
        {
            CloseMenu();                                    // Close the menu and play the opening cutscene
            gameStarted?.Invoke();
        }
    }

    // Function to display a warning when creating a new game
    public void NewGameWarning()
    {
        if (saveExists)                             // If a save file exists, show the warning
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
        primaryButtons.SetActive(false);
        options.Activate();
    }

    public void CloseMenu()
    {
        DisableButtons();

        StartCoroutine(FadeOut());
    }

    // Function to make sure that none of the buttons can be pressed
    public void DisableButtons()
    {
        Cursor.visible = false;                         // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;       // Lock the cursor

        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            btn.interactable = false;
        }
    }

    // Function to gradually fade the buttons out
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
