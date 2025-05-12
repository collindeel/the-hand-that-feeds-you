using UnityEngine;
using System.Collections;

public class RabbitReaction : MonoBehaviour
{
    public Renderer rabbitRenderer;
    public Color glowColor = Color.yellow;
    public float glowDuration = 0.5f;

    private Color originalColor;

    void Start()
    {
        if (rabbitRenderer == null)
        {
            rabbitRenderer = GetComponentInChildren<Renderer>();
        }

        originalColor = rabbitRenderer.material.color;
    }

    public void ReactToFeeding()
    {
        StartCoroutine(GlowRoutine());
    }

    IEnumerator GlowRoutine()
    {
        rabbitRenderer.material.color = glowColor;
        yield return new WaitForSeconds(glowDuration);
        rabbitRenderer.material.color = originalColor;
    }
}
