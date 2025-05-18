using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public float immunityDuration = 2f;  // 2 seconds of invincibility
    private float immunityTimer = 0f;
    public HealthBarController hbc;
    public Image fillImage;

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
        if (immunityTimer <= 0f)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
            hbc.SetHealth(currentHealth);
            Debug.Log($"Player took {damage} damage. Remaining health: {currentHealth}");

            // Trigger immunity window
            immunityTimer = immunityDuration;

            if (currentHealth <= 0)
            {
                Debug.Log("Player is dead!");
                // Handle player death here
            }
        }
    }

    public bool IsImmune()
    {
        return immunityTimer > 0f;
    }
}

