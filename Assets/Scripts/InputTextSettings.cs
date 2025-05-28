using TMPro;
using UnityEngine;

public class InputTextSettings : MonoBehaviour
{
    public TMP_FontAsset dyslexiaFriendlyFont;
    public bool alwaysAdhdFriendly;

    TMP_InputField inputField;
    TMP_FontAsset defaultFont;

    bool currentDyslexiaFriendlyFontSetting = false;

    void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        defaultFont = inputField.fontAsset;

        UpdateTextSettings();
    }

    public void UpdateTextSettings()
    {
        var newDyslexiaFriendlyFontSetting = PlayerPrefs.GetInt("Dyslexia-friendly Font", currentDyslexiaFriendlyFontSetting ? 1 : 0) == 1;
        if (newDyslexiaFriendlyFontSetting != currentDyslexiaFriendlyFontSetting)
            SetDyslexiaFriendlyFontSetting(newDyslexiaFriendlyFontSetting);
    }

    void SetDyslexiaFriendlyFontSetting(bool isEnabled)
    {
        inputField.SetGlobalFontAsset(isEnabled ? dyslexiaFriendlyFont : defaultFont);
        currentDyslexiaFriendlyFontSetting = isEnabled;
    }
}
