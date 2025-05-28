using System;
using UnityEngine;

public class Episode2Prelude : Objective
{
    public DialogueManager dialogueManager;
    public GameObject globalVariablesPrefab;

    (string, string)[] _dialogue;


    /*readonly (string, string)[] _dialogue = {
            (playerName + "-chan! You're doing so well! All of the rabbits love you! (u/w/u) ", "Unity-Chan"),
            ("Let's see... You've earned $" + + ScoreTracker.GetScore() + " on your first day!!", "Unity-Chan"),
            ($"Sugoi... That's amazing, Misaki-Chan!! (*0*)/ ","Unity-Chan"),
            ("Anyway! For today, do the same as normal! (uwu)", "Unity-Chan"),
            ("But be careful though... The rabbits are a little feisty today hehe. They stole some of my carrots! (._.;)>", "Unity-Chan"),
            ("Double pay this time! Isn't that nice? Just... try not to upset them. If they chase you... I'll, um, take some of your money. Sorry!! That's just the rule. Haha. Haha.", "Unity-Chan"),
        }; */


    void Start()
    {
        gameObject.name = "Episode 2 Objective";


        var globalVariablesObject = GameObject.FindWithTag("GlobalVariables");
        if (globalVariablesObject == null)
        {
            globalVariablesObject = Instantiate(globalVariablesPrefab);
            globalVariablesObject.tag = "GlobalVariables";
            DontDestroyOnLoad(globalVariablesObject);
        }
        var playerName = globalVariablesObject.GetComponent<GlobalVariables>().playerName;

        _dialogue = new (string, string)[] {
            (playerName + "-chan! You're doing so well! All of the rabbits love you! (u/w/u) ", "Unity-Chan"),
            ("Let's see... You've earned $" + +ScoreTracker.GetScore() + " on your first day!!", "Unity-Chan"),
            ($"Sugoi... That's amazing, " + playerName + "!! (*0*)/ ", "Unity-Chan"),
            ("Anyway! For today, do the same as normal! (uwu)", "Unity-Chan"),
            ("But be careful though... The rabbits are a little feisty today hehe. They stole some of my carrots! (._.;)>", "Unity-Chan"),
            ("Double pay this time! Isn't that nice? Just... try not to upset them. If they chase you... I'll, um, take some of your money. Sorry!! That's just the rule~ (^p^)>. Teehee.", "Unity-Chan"),
        };
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
