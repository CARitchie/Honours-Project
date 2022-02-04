using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
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
    }

    public void Resume()
    {
        paused = false;

        Time.timeScale = 1;
    }
}
