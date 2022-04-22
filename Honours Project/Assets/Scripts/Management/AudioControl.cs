using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{
    static AudioControl Instance;

    [SerializeField] AudioMixer gameAudio;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SettingsManager.OnChangesMade += LoadAudio;
        LoadAudio();
    }

    private void OnDestroy()
    {
        SettingsManager.OnChangesMade -= LoadAudio;
    }

    // Function to muffle audio depending upon the current atmosphere strength
    public static void AtmosphereInterpolation(float val)
    {
        if (Instance == null) return;
        Instance.gameAudio.SetFloat("Frequency", val * (1 - 0.05f) + 0.05f);
    }

    // Function to load the volume from user settings
    void LoadAudio()
    {
        if (PlayerPrefs.HasKey("Audio"))
        {
            int value = PlayerPrefs.GetInt("Audio");
            value -= 8;
            value *= 10;

            gameAudio.SetFloat("MasterVolume", value);
        }
    }

}
