using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    void Start()
    {
        var dialogManager = GetComponent<DialogManager>();
        if (dialogManager != null)
        {
            dialogManager.CreateDialog("Notice", "Trigger warning info available at the bottom right of the main menu.");
        }
    }


    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
