using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDrop;
    [SerializeField] TMP_Dropdown fullscreenDrop;
    [SerializeField] TMP_Dropdown vSyncDrop;
    [SerializeField] Slider audioSlider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider shipSlider;
    [SerializeField] TMP_Dropdown graphicsDrop;
    [SerializeField] TMP_Dropdown processingDrop;
    [SerializeField] TMP_Dropdown atmosphereDrop;
    List<Resolution> resolutions;

    // Start is called before the first frame update
    void Start()
    {
        LoadButtons();    
    }

    void LoadButtons()
    {
        resolutionDrop.ClearOptions();
        LoadResolutions();
        LoadFullscreen();
        LoadVSync();
        LoadAudio();
        LoadSensitivity();
        LoadShip();
        LoadGraphics();
        LoadProcessing();
        LoadAtmosphere();
    }

    void LoadResolutions()
    {
        resolutions = new List<Resolution>();

        Resolution[] allRes = Screen.resolutions;
        for(int i = 0; i < allRes.Length; i++)
        {
            if (ResolutionExists(allRes[i]) == -1) resolutions.Add(allRes[i]);
        }

        foreach (Resolution res in resolutions)
        {
            resolutionDrop.options.Add(new TMP_Dropdown.OptionData(res.width + " x " + res.height));
        }

        bool resLoaded = false;

        if(PlayerPrefs.HasKey("Screen_Width") && PlayerPrefs.HasKey("Screen_Height"))
        {
            int index = ResolutionExists(PlayerPrefs.GetInt("Screen_Width"), PlayerPrefs.GetInt("Screen_Width"));
            if (index != -1)
            {
                resolutionDrop.SetValueWithoutNotify(index);
                resLoaded = true;
            }
        }

        if (!resLoaded)
        {
            int current = ResolutionExists(Screen.currentResolution);
            if (current != -1)
            {
                resolutionDrop.SetValueWithoutNotify(current);
            }
        }
    }

    bool ResolutionEqual(Resolution res1, Resolution res2)
    {
        return res1.height == res2.height && res1.width == res2.width;
    }

    int ResolutionExists(Resolution res)
    {
        for(int i = 0; i < resolutions.Count; i++)
        {
            if (ResolutionEqual(resolutions[i], res)) return i;
        }
        return -1;
    }

    int ResolutionExists(int width, int height)
    {
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == width && resolutions[i].height == height) return i;
        }
        return -1;
    }

    void LoadFullscreen()
    {
        fullscreenDrop.ClearOptions();
        List<string> options = new List<string>() { "Exclusive Fullscreen", "Fullscreen Window", "Maximised Window", "Windowed" };
        fullscreenDrop.AddOptions(options);

        int current;
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            current = PlayerPrefs.GetInt("Fullscreen");
        }
        else
        {
            current = (int)Screen.fullScreenMode;
        }

        if (current < options.Count) fullscreenDrop.SetValueWithoutNotify(current);
    }

    void LoadVSync()
    {
        vSyncDrop.ClearOptions();
        List<string> options = new List<string>() { "Off" , "On" };
        vSyncDrop.AddOptions(options);

        int current;
        if (PlayerPrefs.HasKey("VSync"))
        {
            current = PlayerPrefs.GetInt("VSync");
        }
        else
        {
            current = QualitySettings.vSyncCount;
        }   
        if (current < options.Count) vSyncDrop.SetValueWithoutNotify(current);
    }

    void LoadAudio()
    {
        if (PlayerPrefs.HasKey("Audio"))
        {
            audioSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("Audio"));
        }
    }

    void LoadSensitivity()
    {
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            sensitivitySlider.SetValueWithoutNotify(PlayerPrefs.GetInt("Sensitivity"));
        }
    }

    void LoadShip()
    {
        if (PlayerPrefs.HasKey("Ship"))
        {
            shipSlider.SetValueWithoutNotify(PlayerPrefs.GetInt("Ship"));
        }
    }

    void LoadGraphics()
    {
        graphicsDrop.ClearOptions();
        List<string> options = new List<string>() { "Very Low", "Low", "Medium", "High", "Very High", "Ultra" };
        graphicsDrop.AddOptions(options);

        int current;
        if (PlayerPrefs.HasKey("Graphics"))
        {
            current = PlayerPrefs.GetInt("Graphics");
        }
        else
        {
            current = QualitySettings.GetQualityLevel();
        }
        if (current < options.Count) graphicsDrop.SetValueWithoutNotify(current);
    }

    void LoadProcessing()
    {
        processingDrop.ClearOptions();
        List<string> options = new List<string>() { "Off", "On" };
        processingDrop.AddOptions(options);

        int current = 1;
        if (PlayerPrefs.HasKey("Processing"))
        {
            current = PlayerPrefs.GetInt("Processing");
        }
        if (current < options.Count) processingDrop.SetValueWithoutNotify(current);
    }

    void LoadAtmosphere()
    {
        atmosphereDrop.ClearOptions();
        List<string> options = new List<string>() { "Off", "On" };
        atmosphereDrop.AddOptions(options);

        int current = 1;
        if (PlayerPrefs.HasKey("Atmosphere"))
        {
            current = PlayerPrefs.GetInt("Atmosphere");
        }
        if (current < options.Count) atmosphereDrop.SetValueWithoutNotify(current);
    }

    void SaveSettings()
    {
        if (SettingsManager.loaded)
        {
            PlayerPrefs.SetInt("Screen_Width", resolutions[resolutionDrop.value].width);
            PlayerPrefs.SetInt("Screen_Height", resolutions[resolutionDrop.value].height);
        }

        PlayerPrefs.SetInt("Fullscreen", fullscreenDrop.value);
        PlayerPrefs.SetInt("VSync", vSyncDrop.value);
        PlayerPrefs.SetInt("Audio", (int)audioSlider.value);
        PlayerPrefs.SetInt("Sensitivity", (int)sensitivitySlider.value);
        PlayerPrefs.SetInt("Ship", (int)shipSlider.value);
        PlayerPrefs.SetInt("Graphics", graphicsDrop.value);
        PlayerPrefs.SetInt("Processing", processingDrop.value);
        PlayerPrefs.SetInt("Atmosphere", atmosphereDrop.value);

        SettingsManager.ApplyChanges();
    }

    public void CloseSettings()
    {
        SaveSettings();
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
}
