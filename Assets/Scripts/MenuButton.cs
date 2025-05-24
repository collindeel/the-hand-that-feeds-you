using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    void Start()
    {
        var button = GetComponent<Button>();

        var colors = button.colors;
        colors.normalColor = Color.lightPink;
        colors.selectedColor = colors.highlightedColor = Color.hotPink;
        colors.pressedColor = Color.white;

        button.colors = colors;
    }
}
