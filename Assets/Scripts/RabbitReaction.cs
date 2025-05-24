using UnityEngine;

public class RabbitReaction : MonoBehaviour
{
    //public Renderer rabbitRenderer;
    //public Color glowColor = Color.yellow;
    //public float glowDuration = 0.5f;
    public ParticleSystem heartParticles;
    public ParticleSystem moguParticles;

    private Color originalColor;

    void Start()
    {

        if (heartParticles == null) {
            heartParticles = transform.Find("hearts").GetComponentInChildren<ParticleSystem>();
        }
        if (moguParticles == null) {
            moguParticles = transform.Find("mogu mogu").GetComponentInChildren<ParticleSystem>();

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

        if (heartParticles != null) {
            heartParticles.Play();
        }
        if (moguParticles != null) {
            moguParticles.Play();
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
