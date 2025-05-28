using System;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public Dialog dialogPrefab;

    public void CreateDialog(string title, string text, Action callback = null)
    {
        var dialog = Instantiate(dialogPrefab);
        dialog.SetContent(title, text, callback);
    }

    public void CreateDialog(string title, string text, string inputPlaceholder, Action<string> callback)
    {
        var dialog = Instantiate(dialogPrefab);
        dialog.SetContent(title, text, inputPlaceholder, callback);
    }

    public void CreateDialog(string contents)
    {
        var splitContents = contents.Split('|');
        CreateDialog(splitContents[0], splitContents[1]);
    }
}
