using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public float immunityDuration = 2f;  // 2 seconds of invincibility
    private float immunityTimer = 0f;
    public HealthBarController hbc;
    public CameraShake cs;
    public Image fillImage;
    public MenuManager mm;
    public CanvasGroup cg;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (immunityTimer > 0f)
        {
            immunityTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage == 0) // No damage, but controlled here
        {
            cs.Shake(0.05f, 0.1f);
        }
        else if (immunityTimer <= 0f)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
            hbc.SetHealth(currentHealth);
            float dur = 0.1f + (90 - currentHealth) * 0.01f;
            float mag = 0.2f + (90 - currentHealth) * 0.01f;
            cs.Shake(dur, mag);
            //Debug.Log($"Player took {damage} damage. Remaining health: {currentHealth}");

            // Trigger immunity window
            immunityTimer = immunityDuration;

            if (currentHealth <= 0)
            {
                StartCoroutine(DoQuit());
            }
        }
    }

    public IEnumerator DoQuit()
    {
        Time.timeScale = 0f;
        cg.alpha = 1;
        yield return new WaitForSecondsRealtime(3);
        mm.QuitGame();
    }

    public bool IsImmune()
    {
        return immunityTimer > 0f;
    }
}

