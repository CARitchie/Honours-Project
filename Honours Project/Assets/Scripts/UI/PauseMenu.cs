using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject buttons;
    [SerializeField] GameObject crosshair;
    [SerializeField] GameObject weaponWheel;
    [SerializeField] GameObject saveWarning;

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

        CloseWarning();
        crosshair.SetActive(false);
        weaponWheel.SetActive(false);
        HUD.SetActive(false);

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
        saveWarning.SetActive(false);
        crosshair.SetActive(true);
        weaponWheel.SetActive(true);
        HUD.SetActive(true);

        PlayerController.SetPaused(false);
    }

    public void Save()
    {
        if (!SaveManager.AttemptSave())
        {
            buttons.SetActive(false);
            saveWarning.SetActive(true);
            Debug.Log("Could not save");
        }
        else HUD.SpinSaveIcon(true);
    }

    public void Restart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

    public void CloseWarning()
    {
        saveWarning.SetActive(false);
        buttons.SetActive(true);
    }
}
