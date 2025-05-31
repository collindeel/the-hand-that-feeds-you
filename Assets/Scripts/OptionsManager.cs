using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameObject audioSettings;
    public GameObject textSettings;
    public GameObject resSettings;
    public Button backButton;


    InputAction _navigateAction;
    InputAction _pointAction;

    void Start()
    {
        SetColorsOfSelectables();
        LoadVolumeSettings();
        LoadTextSettings();
        LoadResSettings();

        _navigateAction = InputSystem.actions.FindAction("Navigate");
        _pointAction = InputSystem.actions.FindAction("Point");
    }

    void Update()
    {
        if (_navigateAction.WasPressedThisFrame())
        {
            if (GameObject.FindWithTag("DialogBox") == null)
            {
                Cursor.lockState = CursorLockMode.Locked;
                if (EventSystem.current.currentSelectedGameObject == null)
                    EventSystem.current.SetSelectedGameObject(backButton.gameObject);
            }
        }
        else if (_pointAction.WasPerformedThisFrame())
        {
            if (GameObject.FindWithTag("DialogBox") == null)
            {
                Cursor.lockState = CursorLockMode.None;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    void SetColorsOfSelectables()
    {
        var colors = backButton.colors;
        colors.normalColor = Color.lightPink;
        colors.selectedColor = colors.highlightedColor = Color.hotPink;
        colors.pressedColor = Color.white;

        backButton.colors = colors;

        var sliders = audioSettings.GetComponentsInChildren<Slider>();
        var toggles = textSettings.GetComponentsInChildren<Toggle>();

        foreach (var slider in sliders) slider.colors = colors;
        foreach (var toggle in toggles) toggle.colors = colors;
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

            slider.onValueChanged.AddListener(delegate { SaveVolumeSetting(slider); });
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
    public void LoadResSettings()
    {
        var toggles = resSettings.GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            var settingName = toggle.transform.parent.name;
            var value = PlayerPrefs.GetInt(settingName, 1) == 1;
            toggle.SetIsOnWithoutNotify(value);
        }
    }
    public void SaveResSetting(Toggle toggle)
    {
        var settingName = toggle.transform.parent.name;
        var value = toggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt(settingName, value);
    }

    public void SaveTextSetting(Toggle toggle)
    {
        var settingName = toggle.transform.parent.name;
        var value = toggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt(settingName, value);
    }
}
