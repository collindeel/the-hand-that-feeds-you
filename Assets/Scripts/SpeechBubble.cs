using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    TextMeshProUGUI _textBlock;
    GameObject _player;

    void Start()
    {
        _textBlock = GetComponentInChildren<TextMeshProUGUI>();
        _player = GameObject.FindWithTag("MainCamera");
        print("New speech bubble!");
    }

    void Update()
    {
        var playerPosition2d = new Vector2(_player.transform.position.x, _player.transform.position.z);
        var speakerPosition2d = new Vector2(transform.parent.position.x, transform.parent.position.z);
        var angle = Vector2.SignedAngle(Vector2.up,  playerPosition2d - speakerPosition2d);
        print($"Positions: {playerPosition2d}, {speakerPosition2d} Difference: {playerPosition2d - speakerPosition2d} Angle: {angle}");
        transform.eulerAngles = new Vector3(0, 180 - angle, 0);
    }

    public void SetText(string text) {
        _textBlock.text = text;
    }
}
