using UnityEngine;
using System.Collections;

public class RabbitReaction : MonoBehaviour
{
    //public Renderer rabbitRenderer;
    //public Color glowColor = Color.yellow;
    //public float glowDuration = 0.5f;
    public ParticleSystem heartParticles;

    private Color originalColor;

    void Start()
    {

        if (heartParticles == null)
        {
            heartParticles = GetComponentInChildren<ParticleSystem>();
        }


        /*if (rabbitRenderer == null)
        {
            rabbitRenderer = GetComponentInChildren<Renderer>();
        }

        originalColor = rabbitRenderer.material.color;
        */
    }

    public void ReactToFeeding()
    {

        if (heartParticles != null)
        {
            heartParticles.Play();
        }
        //StartCoroutine(GlowRoutine());
    }

    /*IEnumerator GlowRoutine()
    {
        rabbitRenderer.material.color = glowColor;
        yield return new WaitForSeconds(glowDuration);
        rabbitRenderer.material.color = originalColor;
    }*/
}
