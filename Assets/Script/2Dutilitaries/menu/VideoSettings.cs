using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VideoSettings : MonoBehaviour
{
    public static VideoSettings Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Dropdown displayModeDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown vsyncDropdown;
    [SerializeField] private TMP_Dropdown fpsDropdown;
    [SerializeField] private GameObject mainpanel;
    [SerializeField] private GameObject videoPanelRoot;

    private Resolution[] resolutions;
    private List<Resolution> availableResolutions = new List<Resolution>();

    private const string ResolutionKey = "Resolution";
    private const string DisplayModeKey = "DisplayMode";
    private const string QualityKey = "Quality";
    private const string VSyncKey = "VSync";
    private const string FPSKey = "FPS";

    private void Start()
    {
        SetupDisplayModes();
        SetupResolutions();
        SetupQuality();
        SetupVSync();
        SetupFPS();

        LoadSettings();
    }

    private void SetupDisplayModes()
    {
        displayModeDropdown.ClearOptions();

        List<string> options = new List<string>
        {
            "Fullscreen",
            "Borderless",
            "Windowed"
        };

        displayModeDropdown.AddOptions(options);
    }

    private void SetupResolutions()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        availableResolutions.Clear();

        List<string> options = new List<string>();

        foreach (Resolution resolution in resolutions)
        {
            string option = resolution.width + " x " + resolution.height;

            // Avoid duplicate resolutions
            if (!options.Contains(option))
            {
                options.Add(option);
                availableResolutions.Add(resolution);
            }
        }

        resolutionDropdown.AddOptions(options);

        // Find highest available resolution
        int highestResolutionIndex = 0;

        for (int i = 1; i < availableResolutions.Count; i++)
        {
            if (availableResolutions[i].width > availableResolutions[highestResolutionIndex].width ||
                (availableResolutions[i].width == availableResolutions[highestResolutionIndex].width &&
                 availableResolutions[i].height > availableResolutions[highestResolutionIndex].height))
            {
                highestResolutionIndex = i;
            }
        }

        resolutionDropdown.value = highestResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void SetupQuality()
    {
        qualityDropdown.ClearOptions();

        List<string> options = new List<string>
        {
            "Low",
            "Medium",
            "High"
        };

        qualityDropdown.AddOptions(options);
    }

    private void SetupVSync()
    {
        vsyncDropdown.ClearOptions();

        List<string> options = new List<string>
        {
            "Off",
            "On"
        };

        vsyncDropdown.AddOptions(options);
    }


    private void SetupFPS()
    {
        fpsDropdown.ClearOptions();

        List<string> options = new List<string>
        {
            "30 FPS",
            "60 FPS"
        };

        fpsDropdown.AddOptions(options);
    }

    private void LoadSettings()
    {
        // If the player has never launched the game before
        if (!PlayerPrefs.HasKey(ResolutionKey))
        {
            SetDefaultSettings();
            return;
        }

        int resolutionIndex = PlayerPrefs.GetInt(ResolutionKey);
        int displayModeIndex = PlayerPrefs.GetInt(DisplayModeKey);
        int qualityIndex = PlayerPrefs.GetInt(QualityKey);
        int vsyncIndex = PlayerPrefs.GetInt(VSyncKey);
        int fpsIndex = PlayerPrefs.GetInt(FPSKey);

        // Safety checks
        resolutionIndex = Mathf.Clamp(
            resolutionIndex,
            0,
            availableResolutions.Count - 1
        );

        displayModeIndex = Mathf.Clamp(displayModeIndex, 0, 2);
        qualityIndex = Mathf.Clamp(qualityIndex, 0, 2);
        vsyncIndex = Mathf.Clamp(vsyncIndex, 0, 1);
        fpsIndex = Mathf.Clamp(fpsIndex, 0, 3);

        resolutionDropdown.value = resolutionIndex;
        displayModeDropdown.value = displayModeIndex;
        qualityDropdown.value = qualityIndex;
        vsyncDropdown.value = vsyncIndex;
        fpsDropdown.value = fpsIndex;

        resolutionDropdown.RefreshShownValue();
        displayModeDropdown.RefreshShownValue();
        qualityDropdown.RefreshShownValue();
        vsyncDropdown.RefreshShownValue();
        fpsDropdown.RefreshShownValue();

        ApplySettings(false);
    }


    private void SetDefaultSettings()
    {
        // Highest resolution
        int highestResolutionIndex = availableResolutions.Count - 1;

        resolutionDropdown.value = highestResolutionIndex;

        // Borderless
        displayModeDropdown.value = 1;

        // Medium quality
        qualityDropdown.value = 1;

        // VSync ON
        vsyncDropdown.value = 1;

        // 60 FPS
        fpsDropdown.value = 1;

        resolutionDropdown.RefreshShownValue();
        displayModeDropdown.RefreshShownValue();
        qualityDropdown.RefreshShownValue();
        vsyncDropdown.RefreshShownValue();
        fpsDropdown.RefreshShownValue();

        ApplySettings(true);
    }

    public void ApplySettings()
    {
        ApplySettings(true);
    }

    private void ApplySettings(bool save)
    {
        if (availableResolutions.Count == 0)
            return;


        Resolution resolution =
            availableResolutions[resolutionDropdown.value];

        FullScreenMode fullscreenMode;

        switch (displayModeDropdown.value)
        {
            case 0:
                // Exclusive fullscreen
                fullscreenMode = FullScreenMode.ExclusiveFullScreen;
                break;

            case 1:
                // Borderless fullscreen
                fullscreenMode = FullScreenMode.FullScreenWindow;
                break;

            default:
                // Windowed
                fullscreenMode = FullScreenMode.Windowed;
                break;
        }

        Screen.SetResolution(
            resolution.width,
            resolution.height,
            fullscreenMode,
            resolution.refreshRateRatio
        );

        int qualityLevel;

        switch (qualityDropdown.value)
        {
            case 0:
                qualityLevel = 0;
                break;

            case 1:
                qualityLevel = Mathf.Min(1, QualitySettings.names.Length - 1);
                break;

            default:
                qualityLevel = Mathf.Min(2, QualitySettings.names.Length - 1);
                break;
        }

        QualitySettings.SetQualityLevel(qualityLevel);


        if (vsyncDropdown.value == 1)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        switch (fpsDropdown.value)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;

            case 1:
                Application.targetFrameRate = 60;
                break;

            case 2:
                Application.targetFrameRate = 120;
                break;

            case 3:
                Application.targetFrameRate = -1;
                break;
        }


        if (save)
        {
            SaveSettings();
        }
    }


    private void SaveSettings()
    {
        PlayerPrefs.SetInt(
            ResolutionKey,
            resolutionDropdown.value
        );

        PlayerPrefs.SetInt(
            DisplayModeKey,
            displayModeDropdown.value
        );

        PlayerPrefs.SetInt(
            QualityKey,
            qualityDropdown.value
        );

        PlayerPrefs.SetInt(
            VSyncKey,
            vsyncDropdown.value
        );

        PlayerPrefs.SetInt(
            FPSKey,
            fpsDropdown.value
        );

        PlayerPrefs.Save();

        Debug.Log("Video settings saved.");
    }

    public void ResetSettings()
    {
        PlayerPrefs.DeleteKey(ResolutionKey);
        PlayerPrefs.DeleteKey(DisplayModeKey);
        PlayerPrefs.DeleteKey(QualityKey);
        PlayerPrefs.DeleteKey(VSyncKey);
        PlayerPrefs.DeleteKey(FPSKey);

        SetDefaultSettings();

        Debug.Log("Video settings reset.");
    }

    public void Retour()
    {
        videoPanelRoot.SetActive(false);
        mainpanel.SetActive(true);
    }
}