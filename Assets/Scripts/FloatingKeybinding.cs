using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FloatingKeybinding : MonoBehaviour
{
    TextMeshProUGUI _textbox;
    GameObject _mainCamera;
    PlayerInput _playerInput;
    TMP_SpriteAsset _keyboardSprites;
    TMP_SpriteAsset _playstationSprites;
    TMP_SpriteAsset _xboxSprites;

    void Start()
    {
        _textbox = GetComponentInChildren<TextMeshProUGUI>();
        _mainCamera = GameObject.FindWithTag("MainCamera");

        var dialogueBox = GameObject.FindWithTag("DialogueBox").GetComponent<DialogueBox>();
        _keyboardSprites = dialogueBox.keyboardSprites;
        _playstationSprites = dialogueBox.playstationSprites;
        _xboxSprites = dialogueBox.xboxSprites;
        _playerInput = dialogueBox.playerInput;

        InputSystem.onActionChange += UpdateSpriteAsset;
    }

    void Update()
    {
        var cameraPosition2d = new Vector2(_mainCamera.transform.position.x, _mainCamera.transform.position.z);
        var speakerPosition2d = new Vector2(transform.parent.position.x, transform.parent.position.z);
        var angle = Vector2.SignedAngle(Vector2.up, cameraPosition2d - speakerPosition2d);
        transform.eulerAngles = new Vector3(0, 180 - angle, 0);
    }

    public void UpdateSpriteAsset(object obj, InputActionChange change)
    {
        if (_playerInput.currentControlScheme == "Gamepad")
        {
            var gamepadName = Gamepad.current.displayName.ToLower();
            if (gamepadName.StartsWith("ps") || gamepadName.StartsWith("dualsense") || gamepadName.StartsWith("dualshock"))
                _textbox.spriteAsset = _playstationSprites;
            else _textbox.spriteAsset = _xboxSprites;
        }
        else _textbox.spriteAsset = _keyboardSprites;
    }
}
