using System.Collections;
using UnityEngine;

public class Tutorial : Objective
{
    public DialogueManager dialogueManager;

    readonly (string, string)[] _tutorialDialogue = {
            ($"Hey there! You must be Misaki! I'm Unity-Chan. It's so nice to meet you!", "Unity-Chan"),
            ("Are you ready to get to work?", "Unity-Chan"),
            ("Are you ready to ", "Unity-Chan"),
            ("If you see a carrot, you can pick it up by pressing E. If you walk up to a bunny, you can use the same button to feed it!", "Unity-Chan"),
            ("Are you ready to ", "Unity-Chan"),
            ("Be careful though, the bunnies can get a little feisty when they get hungry.", "Unity-Chan"),
            ("Good luck!", "Unity-Chan")
        };


    void Start()
    {
        gameObject.name = "Tutorial Objective";
    }

    public static void Initialize(GameObject questPrefab, GameObject unityChan, DialogueManager dialogueManager)
    {
        var objectiveObject = Instantiate(questPrefab, unityChan.transform);
        var tutorial = objectiveObject.AddComponent<Tutorial>();
        tutorial.dialogueManager = dialogueManager;
    }

    public override void Trigger()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        dialogueManager.PlayDialogue(_tutorialDialogue);

        Destroy(gameObject);
    }
}
