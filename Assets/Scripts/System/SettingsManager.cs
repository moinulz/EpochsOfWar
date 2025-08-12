using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI (optional)")]
    public Dropdown qualityDropdown;
    public Slider musicSlider;
    public Toggle vibrationToggle;

    [Header("Audio (optional)")]
    public AudioSource musicSource;

    void Start()
    {
        // Populate quality levels
        if (qualityDropdown)
        {
            qualityDropdown.ClearOptions();
            var names = QualitySettings.names;
            var opts = new System.Collections.Generic.List<string>(names);
            qualityDropdown.AddOptions(opts);
            qualityDropdown.value = PlayerPrefs.GetInt("quality", QualitySettings.GetQualityLevel());
            QualitySettings.SetQualityLevel(qualityDropdown.value);
            qualityDropdown.RefreshShownValue();
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }

        float musicVol = PlayerPrefs.GetFloat("musicVol", 0.5f);
        if (musicSlider)
        {
            musicSlider.value = musicVol;
            musicSlider.onValueChanged.AddListener(OnMusicVolume);
        }
        if (musicSource) musicSource.volume = musicVol;

        bool vib = PlayerPrefs.GetInt("vibration", 1) == 1;
        if (vibrationToggle)
        {
            vibrationToggle.isOn = vib;
            vibrationToggle.onValueChanged.AddListener(OnVibrationToggled);
        }
    }

    void OnQualityChanged(int idx)
    {
        QualitySettings.SetQualityLevel(idx, true);
        PlayerPrefs.SetInt("quality", idx);
    }

    void OnMusicVolume(float v)
    {
        if (musicSource) musicSource.volume = v;
        PlayerPrefs.SetFloat("musicVol", v);
    }

    void OnVibrationToggled(bool on)
    {
        PlayerPrefs.SetInt("vibration", on ? 1 : 0);
        // Hook platform haptics later if you like
    }
}