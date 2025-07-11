using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueBox : MonoBehaviour
{
    public GameObject textboxNormal;
    public GameObject textboxDyslexiaFriendly;
    public GameObject nameboxNormal;
    public GameObject nameboxDyslexiaFriendly;
    public PlayerInput playerInput;
    public TMP_SpriteAsset keyboardSprites;
    public TMP_SpriteAsset playstationSprites;
    public TMP_SpriteAsset xboxSprites;

    GameObject _textbox;
    GameObject _namebox;
    TextMeshProUGUI _textboxText;
    TextMeshProUGUI _nameboxText;
    TextSettings _textboxTextSettings;
    TextSettings _nameboxTextSettings;
    string _currentSpeaker;
    string _currentText;
    bool _displaying = false;

    void Awake()
    {
        UpdateTextSettings();
    }

    public void DisplayText(string text, string speaker = null)
    {
        if (!_displaying) InputSystem.onActionChange += UpdateSpriteAsset;
        _displaying = true;
        _currentSpeaker = speaker;
        _currentText = text;
        _textbox.SetActive(true);
        _textboxText.text = text;
        _textboxTextSettings.ReinitializeDefaultText();

        if (speaker != null)
        {
            _namebox.SetActive(true);
            _nameboxText.text = speaker;
            _nameboxTextSettings.ReinitializeDefaultText();
        }
    }

    public void DisplayText((string text, string speaker) line)
    {
        DisplayText(line.text, line.speaker);
    }

    public void Hide()
    {
        InputSystem.onActionChange -= UpdateSpriteAsset;
        _textbox.SetActive(false);
        _namebox.SetActive(false);
        _displaying = false;
    }

    public void UpdateSpriteAsset(object obj, InputActionChange change)
    {
        if (playerInput.currentControlScheme == "Gamepad")
        {
            var gamepadName = Gamepad.current.displayName.ToLower();
            if (gamepadName.StartsWith("ps") || gamepadName.StartsWith("dualsense") || gamepadName.StartsWith("dualshock"))
                _textboxText.spriteAsset = playstationSprites;
            else _textboxText.spriteAsset = xboxSprites;
        }
        else _textboxText.spriteAsset = keyboardSprites;
    }

    public void UpdateTextSettings()
    {
        _textbox?.SetActive(false);
        _namebox?.SetActive(false);

        if (PlayerPrefs.GetInt("Dyslexia-friendly Font", 0) == 1)
        {
            _textbox = textboxDyslexiaFriendly;
            _namebox = nameboxDyslexiaFriendly;
        }
        else
        {
            _textbox = textboxNormal;
            _namebox = nameboxNormal;
        }

        _nameboxText = _namebox.GetComponentInChildren<TextMeshProUGUI>();
        _nameboxTextSettings = _nameboxText.GetComponent<TextSettings>();
        _textboxText = _textbox.GetComponentInChildren<TextMeshProUGUI>();
        _textboxTextSettings = _textboxText.GetComponent<TextSettings>();

        if (_displaying)
            DisplayText(_currentText, _currentSpeaker);
    }
}
