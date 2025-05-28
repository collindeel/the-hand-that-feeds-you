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
        if (globalVariablesObject == null)
        {
            globalVariablesObject = Instantiate(globalVariablesPrefab);
            globalVariablesObject.tag = "GlobalVariables";
            DontDestroyOnLoad(globalVariablesObject);
        }
        var playerName = globalVariablesObject.GetComponent<GlobalVariables>().playerName;

        _dialogue = new (string, string)[] {
            ("...", playerName),
            ("... U-Unity-Chan?", playerName),
            (playerName + "...", "Unity-Chan"),
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
