using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameObject audioSettings;
    public GameObject textSettings;

    void Start()
    {
        LoadVolumeSettings();
        LoadTextSettings();
    }

    public void LoadVolumeSettings()
    {
        var sliders = audioSettings.GetComponentsInChildren<Slider>();
        foreach (var slider in sliders)
        {
            var settingName = slider.transform.parent.name;
            var value = PlayerPrefs.GetFloat(settingName, 1f);
            slider.SetValueWithoutNotify(value);

            if (value > 0) audioMixer.SetFloat(settingName, Mathf.Log10(value) * 40);
            else audioMixer.SetFloat(settingName, -80);

            slider.onValueChanged.AddListener(delegate {SaveVolumeSetting(slider);});
        }
    }

    public void SaveVolumeSetting(Slider slider)
    {
        var settingName = slider.transform.parent.name;
        var value = slider.value;
        PlayerPrefs.SetFloat(settingName, value);

        if (value > 0) audioMixer.SetFloat(settingName, Mathf.Log10(value) * 40);
        else audioMixer.SetFloat(settingName, -80);
    }

    public void LoadTextSettings()
    {
        var toggles = textSettings.GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            var settingName = toggle.transform.parent.name;
            var value = PlayerPrefs.GetInt(settingName, 0) == 1;
            toggle.SetIsOnWithoutNotify(value);
        }
    }

    public void SaveTextSetting(Toggle toggle)
    {
        var settingName = toggle.transform.parent.name;
        var value = toggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt(settingName, value);
    }
}
