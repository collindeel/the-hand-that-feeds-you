using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public List<Canvas> canvases;

    public void UpdateTextSettings()
    {
        foreach (var canvas in canvases)
            canvas.BroadcastMessage("UpdateTextSettings");
    }
}
