using UnityEngine;
using System;                         // for Action<>

public class RabbitEyeController : MonoBehaviour
{
    // --------------------------------------------------------------------
    // 1) Plug these in once in the prefab inspector
    // --------------------------------------------------------------------
    [SerializeField] Renderer eyeRenderer;
    [SerializeField] Material normalMat;
    [SerializeField] Material evilMat;

    // --------------------------------------------------------------------
    // 2) One global event that flips eyes for EVERY rabbit
    // --------------------------------------------------------------------
    public static event Action<bool> OnEyesToggle;     // true = evil, false = normal

    void OnEnable()  => OnEyesToggle += ApplyEyeState;
    void OnDisable() => OnEyesToggle -= ApplyEyeState;

    void ApplyEyeState(bool makeEvil)
    {
        // sharedMaterial so they ALL flip in perfect sync (no extra instances)
        eyeRenderer.sharedMaterial = makeEvil ? evilMat : normalMat;
    }

    // --------------------------------------------------------------------
    // 3) Static helper your game code can call from anywhere
    // --------------------------------------------------------------------
    public static void SetEyesAll(bool makeEvil) => OnEyesToggle?.Invoke(makeEvil);
}
