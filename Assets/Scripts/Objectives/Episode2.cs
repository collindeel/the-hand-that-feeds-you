using System;
using UnityEngine;

public class Episode2Prelude : Objective
{
    public DialogueManager dialogueManager;

    readonly (string, string)[] _dialogue = {
            ("Misaki-Chan! You're doing so well! All of the rabbits love you! (u/w/u) ", "Unity-Chan"),
            ("Let's see... You've earned $" + + ScoreTracker.GetScore() + " on your first day!!", "Unity-Chan"),
            ($"Sugoi... That's amazing, Misaki-Chan!! (*0*)/ ","Unity-Chan"),
            ("Anyway! For today, do the same as normal! (uwu)", "Unity-Chan"),
            ("But be careful though... The rabbits are a little feisty today hehe. They stole some of my carrots! (._.;)>", "Unity-Chan")
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
