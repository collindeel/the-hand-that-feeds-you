using System.Collections;
using UnityEngine;

public class Episode2Prelude : Objective
{
    public DialogueManager dialogueManager;

    readonly (string, string)[] _dialogue = {
            ("omg the rabbits have loved carrots so much be careful or they will steal them lorem ipsum etc etc etc", "Unity-Chan")
        };


    void Start()
    {
        gameObject.name = "Episode 2 Objective";
    }

    public static void Initialize(GameObject questPrefab, GameObject unityChan, DialogueManager dialogueManager)
    {
        var objectiveObject = Instantiate(questPrefab, unityChan.transform);
        var tutorial = objectiveObject.AddComponent<Episode2Prelude>();
        tutorial.dialogueManager = dialogueManager;
    }

    public override void Trigger()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        dialogueManager.PlayDialogue(_dialogue);

        Destroy(gameObject);
    }
}
