using UnityEngine;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{
    [Header("Hearts")]
    public Heart[] hearts;

    private int previousHealth = -1;

    void OnEnable()
    {
        PlayerMovement.OnHealthChanged += UpdateHealthBar;
    }

    void OnDisable()
    {
        PlayerMovement.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        // Initialize previousHealth if it's the first update
        if (previousHealth == -1)
        {
            previousHealth = currentHealth;
            // Initial setup without animation
            for (int i = 0; i < hearts.Length; i++)
            {
                hearts[i].SetActive(i < currentHealth);
            }
            return;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                // Heart should be full
                hearts[i].SetActive(true);
            }
            else if (i < previousHealth)
            {
                // Heart was full, now empty -> Play destruction
                hearts[i].PlayDestruction();
            }
            else
            {
                // Heart was already empty or is outside range
                // Only set to empty if it's not currently playing animation?
                // Heart.SetActive(false) stops coroutines, so we should only call it if we are sure.
                // If i >= previousHealth, it was already empty.
                hearts[i].SetActive(false);
            }
        }

        previousHealth = currentHealth;
    }
}
