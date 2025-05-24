using UnityEngine;
using Unity.Sentis;                 // ModelAsset
using Unity.MLAgents.Policies;

[RequireComponent(typeof(BehaviorParameters))]
public class RabbitModelSwitcher : MonoBehaviour
{
    [Header("Sentis ModelAssets generated from ONNX files")]
    public ModelAsset timidModel;
    public ModelAsset mediumModel;
    public ModelAsset aggressiveModel;

    [Tooltip("Current temperament shown in the Inspector")]
    public RabbitBehaviorLevel level = RabbitBehaviorLevel.Timid;

    public bool isIdleOnly = false;

    BehaviorParameters bp;
    

    void Awake()
    {
        bp = GetComponent<BehaviorParameters>();
        ApplyModel(); // make sure the prefab spawns with the right brain
    }

    /// <summary>Call this from EpisodeController (or anywhere) to change temperament.</summary>
    public void SetLevel(RabbitBehaviorLevel newLevel)
    {
        //Debug.Log($"{name} switching to {newLevel}");
        if (level == newLevel) return;   // already using that brain
        level = newLevel;
        ApplyModel();
        GetComponent<RabbitAgent>()?.ApplyStats(newLevel);
    }
    public void HeuristicOnly()
    {
        HeuristicOnly(true);
    }
    public void HeuristicOnly(bool isHeuristic)
    {
        if (isHeuristic) bp.BehaviorType = BehaviorType.HeuristicOnly;
        else bp.BehaviorType = BehaviorType.InferenceOnly;
    }

    void ApplyModel()
    {
        // choose the correct ModelAsset and BehaviorName
        ModelAsset chosen = timidModel;
        string name = "RabbitTimid";

        switch (level)
        {
            case RabbitBehaviorLevel.Medium:
                chosen = mediumModel;
                name = "RabbitMedium";
                break;
            case RabbitBehaviorLevel.Aggressive:
                chosen = aggressiveModel;
                name = "RabbitAggressive";
                break;
        }

        bp.Model = chosen;
        bp.BehaviorName = name;
#if !UNITY_EDITOR
        bp.BehaviorType = BehaviorType.InferenceOnly;
#endif
    }

    /* ---------- static registry  ---------- */
    public static readonly System.Collections.Generic.List<RabbitModelSwitcher> All
        = new System.Collections.Generic.List<RabbitModelSwitcher>();

    void OnEnable() => All.Add(this);
    void OnDisable() => All.Remove(this);
}
