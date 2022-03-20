using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject buttons;
    [SerializeField] GameObject crosshair;

    public static PauseMenu Instance;

    bool paused = false;

    private void Awake()
    {
        Instance = this;
    }

    public static bool TogglePause()
    {
        if (Instance == null) return false;

        if (!Instance.paused) Instance.Pause();
        else Instance.Resume();

        return Instance.paused;
    }

    public void Pause()
    {
        paused = true;

        Time.timeScale = 0;

        PostProcessControl.SetVignette(1);
        PostProcessControl.SetDepth(10);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        buttons.SetActive(true);
        crosshair.SetActive(false);
        HUD.SetPlanetTextHolderActive(false);

        PlayerController.SetPaused(true);
    }

    public void Resume()
    {
        paused = false;

        Time.timeScale = 1;
        PostProcessControl.RestoreVignette();
        PostProcessControl.SetDepth(0);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        buttons.SetActive(false);
        crosshair.SetActive(true);
        HUD.SetPlanetTextHolderActive(true);

        PlayerController.SetPaused(false);
    }

    public void Restart()
    {
        SetButtonsInteractive(false);
        SceneManager.FadeToScene("Space");
    }

    public void OpenSettings()
    {

    }

    public void OpenMainMenu()
    {
        SetButtonsInteractive(false);
        SceneManager.FadeToScene("MainMenu");
    }

    public void SetButtonsInteractive(bool val)
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach(Button btn in buttons)
        {
            btn.interactable = val;
        }
    }
}
