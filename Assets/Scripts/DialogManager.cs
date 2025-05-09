using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public Dialog dialogPrefab;

    public void CreateDialog(string title, string text)
    {
        var dialog = Instantiate(dialogPrefab);
        dialog.SetContent(title, text);
    }

    public void CreateDialog(string contents)
    {
        var splitContents = contents.Split('|');
        CreateDialog(splitContents[0], splitContents[1]);
    }
}
