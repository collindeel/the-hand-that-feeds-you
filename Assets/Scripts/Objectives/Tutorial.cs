using UnityEngine;

public class Tutorial : Objective
{
    public DialogueManager dialogueManager;
    public GameObject globalVariablesPrefab;

    (string, string)[] _tutorialDialogue;

    /*readonly (string, string)[] _tutorialDialogue = {
            ($"Ohayo gozaimasu~!", "Unity-Chan"),
            ("You must be " + "! I'm Unity-Chan. It's so nice to meet you!\n(* ^ w ^)", "Unity-Chan"),
            ("Since it's your first day on the job, I'll show you the ropes so we can get you ready!!", "Unity-Chan"),
            (@"If you see a carrot, you can pick it up by pressing <sprite name=""Interact"">. If you walk up to a bunny, you can press <sprite name=""Feed""> to feed it!", "Unity-Chan"),
            ("Every time you feed a bunny, you'll earn points! By the end of the day, whatever your score is, I'll pay you that amount. (^v^)/ ", "Unity-Chan"),
            ("Be careful though, the bunnies can get a little feisty when they get hungry. (._.;)>", "Unity-Chan"),
            ("Good luck, Misaki-Chan, nyah~! (^w^)", "Unity-Chan")
        }; */


    void Start()
    {
        gameObject.name = "Tutorial Objective";

        var globalVariablesObject = GameObject.FindWithTag("GlobalVariables");
        if (globalVariablesObject == null)
        {
            globalVariablesObject = Instantiate(globalVariablesPrefab);
            globalVariablesObject.tag = "GlobalVariables";
            DontDestroyOnLoad(globalVariablesObject);
        }
        var playerName = globalVariablesObject.GetComponent<GlobalVariables>().playerName;

        _tutorialDialogue = new (string, string)[] {
            ($"Ohayo gozaimasu~!", "Unity-Chan"),
            ("You must be " + playerName + "-chan! I'm Unity-Chan. It's so nice to meet you!\n(* ^ w ^)", "Unity-Chan"),
            ("Since it's your first day on the job, I'll show you the ropes so we can get you ready!!", "Unity-Chan"),
            (@"If you see a carrot, you can pick it up by pressing <sprite name=""Interact"">. If you walk up to a bunny, you can press <sprite name=""Feed""> to feed it!", "Unity-Chan"),
            ("Every time you feed a bunny, you'll earn points! By the end of the day, whatever your score is, I'll pay you that amount. (^v^)/ ", "Unity-Chan"),
            ("Be careful though, the bunnies can get a little feisty when they get hungry. (._.;)>", "Unity-Chan"),
            ("Good luck" + playerName + ", nyah~! (^w^)", "Unity-Chan")
        };
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
