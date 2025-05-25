using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class TextSettings : MonoBehaviour
{
    public TMP_FontAsset dyslexiaFriendlyFont;
    public bool alwaysAdhdFriendly;

    TextMeshProUGUI textComponent;
    string defaultText;
    TMP_FontAsset defaultFont;
    bool defaultIsBold;

    bool currentDyslexiaFriendlyFontSetting = false;
    bool currentAdhdFriendlyTextSetting = false;

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        defaultText = textComponent.text;
        defaultFont = textComponent.font;
        defaultIsBold = (textComponent.fontStyle & FontStyles.Bold) == FontStyles.Bold;

        UpdateTextSettings();
    }

    public void ReinitializeDefaultText()
    {
        defaultText = textComponent.text;
        if (defaultIsBold && (textComponent.fontStyle & FontStyles.Bold) != FontStyles.Bold) textComponent.fontStyle ^= FontStyles.Bold;
        currentAdhdFriendlyTextSetting = false;

        UpdateTextSettings();
    }

    public void UpdateTextSettings()
    {
        var newDyslexiaFriendlyFontSetting = PlayerPrefs.GetInt("Dyslexia-friendly Font", currentDyslexiaFriendlyFontSetting ? 1 : 0) == 1;
        if (newDyslexiaFriendlyFontSetting != currentDyslexiaFriendlyFontSetting)
            SetDyslexiaFriendlyFontSetting(newDyslexiaFriendlyFontSetting);

        var newAdhdFriendlyFontSetting = PlayerPrefs.GetInt("ADHD-friendly Text", currentAdhdFriendlyTextSetting ? 1 : 0) == 1 | alwaysAdhdFriendly;
        if (newAdhdFriendlyFontSetting != currentAdhdFriendlyTextSetting)
            SetAdhdFriendlyTextSetting(newAdhdFriendlyFontSetting);
    }

    void SetDyslexiaFriendlyFontSetting(bool isEnabled)
    {
        textComponent.font = isEnabled ? dyslexiaFriendlyFont : defaultFont;
        currentDyslexiaFriendlyFontSetting = isEnabled;
    }

    void SetAdhdFriendlyTextSetting(bool isEnabled)
    {
        textComponent.text = isEnabled ? GenerateAdhdFriendlyText(textComponent.text) : defaultText;
        textComponent.fontStyle ^= defaultIsBold ? FontStyles.Bold : FontStyles.Normal;
        currentAdhdFriendlyTextSetting = isEnabled;
    }

    string GenerateAdhdFriendlyText(string text)
    {
        var regex = new Regex("[A-Za-z]+(?:'[A-Za-z]+)*");
        var adhdFriendlyText = string.Empty;
        var currentIndex = 0;
        foreach (Match match in regex.Matches(text))
        {
            adhdFriendlyText += text.Substring(currentIndex, match.Index - currentIndex)
                + "<b>"
                + match.Value[..(int)Math.Ceiling((float)match.Length / 2)]
                + "</b>"
                + match.Value[(int)Math.Ceiling((float)match.Length / 2)..];
            currentIndex = match.Index + match.Length;
        }
        adhdFriendlyText += text[currentIndex..];
        return adhdFriendlyText;
    }
}
