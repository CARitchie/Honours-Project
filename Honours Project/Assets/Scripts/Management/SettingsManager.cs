using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SettingsManager : MonoBehaviour
{
    public delegate void ChangesAction();
    public static event ChangesAction OnChangesMade;
    public static bool loaded = false;

    private void Start()
    {
        if (loaded) return;
        loaded = true;

        ApplyChanges();
    }

    public static void ApplyChanges()
    {
        // Load the player's display settings
        if (PlayerPrefs.HasKey("Screen_Width") && PlayerPrefs.HasKey("Screen_Height") && PlayerPrefs.HasKey("Fullscreen"))
        {
            Screen.SetResolution(PlayerPrefs.GetInt("Screen_Width"), PlayerPrefs.GetInt("Screen_Height"), (FullScreenMode)PlayerPrefs.GetInt("Fullscreen"));
        }

        // Set the quality level to match the player's settings
        if (PlayerPrefs.HasKey("Graphics"))
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Graphics"));
        }

        // Determine whether VSync should be active based on the player's settings
        if (PlayerPrefs.HasKey("VSync"))
        {
            QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync");
        }


        OnChangesMade?.Invoke();
    }
}
