using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessControl : MonoBehaviour
{
    public static PostProcessControl Instance;

    PostProcessVolume volume;
    Vignette vignette;
    DepthOfField depth;
    float defaultVignette;

    private void Awake()
    {
        Instance = this;

        volume = GetComponent<PostProcessVolume>();

        vignette = volume.profile.GetSetting<Vignette>();
        defaultVignette = vignette.smoothness.value;

        depth = volume.profile.GetSetting<DepthOfField>();
    }

    private void Start()
    {
        SettingsManager.OnChangesMade += LoadSetting;
        LoadSetting();
    }

    private void OnDestroy()
    {
        SettingsManager.OnChangesMade -= LoadSetting;
    }

    public static void SetVignette(float val)
    {
        if (Instance == null) return;
        Instance.vignette.smoothness.value = val;
    }

    public static void RestoreVignette()
    {
        if (Instance == null) return;
        Instance.vignette.smoothness.value = Instance.defaultVignette;
    }

    // Function to interpolate the focal length of the depth of field
    public static void SetDepth(float from, float to, float t)
    {
        if (Instance == null) return;

        Instance.depth.focalLength.Interp(from, to, t);
    }

    // Function to set the focal length of the depth of field
    public static void SetDepth(float value)
    {
        if (Instance == null) return;

        Instance.depth.focalLength.value = value;
    }

    public void LoadSetting()
    {
        if (PlayerPrefs.HasKey("Processing"))
        {
            gameObject.SetActive(PlayerPrefs.GetInt("Processing") == 1);        // Disable/enable post processing depending upon user settings
        }
    }
}
