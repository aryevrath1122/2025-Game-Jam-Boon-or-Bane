using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public TMPro.TMP_Dropdown resolutionDropdown;
    public Button closeButton;
    public Button resetButton;

    private void Start()
    {
        // Initialize sliders with current volume settings
        masterVolumeSlider.value = AudioListener.volume;  // Master volume
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f); // Default: 1f
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Initialize resolution dropdown
        InitializeResolutionDropdown();

        // Add listener for sliders to update volume levels
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        // Add listener for resolution change
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        // Add listeners for buttons
        closeButton.onClick.AddListener(CloseSettingsPanel);
        resetButton.onClick.AddListener(ResetToDefaultSettings);
    }

    private void SetMasterVolume(float value)
    {
        AudioListener.volume = value;  // Controls the master volume
        PlayerPrefs.SetFloat("MasterVolume", value); // Save to PlayerPrefs
    }

    private void SetBGMVolume(float value)
    {
        PlayerPrefs.SetFloat("BGMVolume", value); // Save BGM volume
        // Your logic for controlling BGM volume here (e.g., audio mixer)
    }

    private void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value); // Save SFX volume
        // Your logic for controlling SFX volume here (e.g., audio mixer)
    }

    private void InitializeResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();

        // List of supported resolutions
        Resolution[] resolutions = Screen.resolutions;
        List<string> resolutionOptions = new List<string>();

        foreach (Resolution res in resolutions)
        {
            resolutionOptions.Add(res.width + "x" + res.height);
        }

        resolutionDropdown.AddOptions(resolutionOptions);

        // Set the dropdown to the current screen resolution
        int currentResolutionIndex = System.Array.FindIndex(resolutions, res => res.width == Screen.width && res.height == Screen.height);
        resolutionDropdown.value = currentResolutionIndex;
    }

    private void SetResolution(int index)
    {
        Resolution selectedResolution = Screen.resolutions[index];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
    }

    private void CloseSettingsPanel()
    {
        gameObject.SetActive(false);  // Close the settings panel
    }

    private void ResetToDefaultSettings()
    {
        // Reset audio settings
        float defaultMasterVolume = 1f;
        float defaultBGMVolume = 1f;
        float defaultSFXVolume = 1f;

        masterVolumeSlider.value = defaultMasterVolume;
        bgmVolumeSlider.value = defaultBGMVolume;
        sfxVolumeSlider.value = defaultSFXVolume;

        SetMasterVolume(defaultMasterVolume);
        SetBGMVolume(defaultBGMVolume);
        SetSFXVolume(defaultSFXVolume);

        // Reset resolution to default
        Resolution defaultResolution = Screen.resolutions[0]; // Choose the first resolution as default
        resolutionDropdown.value = 0;
        SetResolution(0);

        // Save the reset defaults
        PlayerPrefs.SetFloat("MasterVolume", defaultMasterVolume);
        PlayerPrefs.SetFloat("BGMVolume", defaultBGMVolume);
        PlayerPrefs.SetFloat("SFXVolume", defaultSFXVolume);

        // Optionally, reset other PlayerPrefs values
        PlayerPrefs.Save(); // Ensure changes persist
    }
}