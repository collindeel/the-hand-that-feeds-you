using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public Sprite[] sprites;

    Button _button;
    System.Random _random;

    void Awake()
    {
        _button = GetComponent<Button>();

        var colors = _button.colors;
        colors.normalColor = Color.lightPink;
        colors.selectedColor = colors.highlightedColor = Color.hotPink;
        colors.pressedColor = Color.white;

        _button.colors = colors;

        _random = new System.Random();
        if (sprites.Length > 0) _button.image.sprite = sprites[_random.Next(sprites.Length - 1)];
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (sprites.Length > 0) _button.image.sprite = sprites[_random.Next(sprites.Length - 1)];
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (sprites.Length > 0) _button.image.sprite = sprites[_random.Next(sprites.Length - 1)];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (sprites.Length > 0) _button.image.sprite = sprites[_random.Next(sprites.Length - 1)];
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (sprites.Length > 0) _button.image.sprite = sprites[_random.Next(sprites.Length - 1)];
    }
}
