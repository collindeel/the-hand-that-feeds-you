using UnityEngine;

public class Episode3AfterStart : Objective
{

    public DialogueManager dialogueManager;
    public GameObject globalVariablesPrefab;

    (string, string)[] _dialogue;


    /*readonly (string, string)[] _dialogue = {
            ("...", playerName),
            ("... U-Unity-Chan?", "Misaki"),
            ("Mi...Sa...Ki...", "Unity-Chan"),
            ("It... Seems... I have... Underestimated the rabbits..", "Unity-Chan"),
            ("Go... While... You still can...", "Unity-Chan"),
            ("...", "Unity-Chan"),
            ("<i><b><color=red>Run.</color></b></i>", "Unity-Chan")
        };
    */

    void Start()
    {
        gameObject.name = "Episode 3 Objective";

        var globalVariablesObject = GameObject.FindWithTag("GlobalVariables");
        var globalVaiables = globalVariablesObject.GetComponent<GlobalVariables>();

        _dialogue = new (string, string)[] {
            ("...", globalVaiables.playerName),
            ("... U-Unity-Chan?", globalVaiables.playerName),
            ($"{globalVaiables.playerName}...", "Unity-Chan"),
            ("It... Seems... I have... Underestimated the rabbits..", "Unity-Chan"),
            ("Go... While... You still can...", "Unity-Chan"),
            ("...", "Unity-Chan"),
            ("<i><b><color=red>Run.</color></b></i>", "Unity-Chan")
        };
    }

    public static void Initialize(GameObject questPrefab, GameObject unityChan, DialogueManager dialogueManager)
    {
        var objectiveObject = Instantiate(questPrefab, unityChan.transform);
        var tutorial = objectiveObject.AddComponent<Episode3AfterStart>();
        tutorial.dialogueManager = dialogueManager;
    }

    public override void Trigger()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        dialogueManager.PlayDialogue(_dialogue);

        Destroy(gameObject);
    }
}
