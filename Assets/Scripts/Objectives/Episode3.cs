using UnityEngine;

public class Episode3AfterStart : Objective
{
    public DialogueManager dialogueManager;

    readonly (string, string)[] _dialogue = {
            ("i am dying omg it seems i have underestimated the rabbits", "Unity-Chan"),
            ("what a world what a world", "Unity-Chan"),
            ("you maniacs you blew it all up", "Unity-Chan"),
            ("omg what is happening", "You"),
        };


    void Start()
    {
        gameObject.name = "Episode 3 Objective";
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
