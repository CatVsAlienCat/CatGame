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
        if (previousHealth == -1)
        {
            previousHealth = currentHealth;
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
                hearts[i].SetActive(true);
            }
            else if (i < previousHealth)
            {
                // El corazón estaba lleno, ahora vacío -> Reproducir destrucción
                hearts[i].PlayDestruction();
            }
            else
            {
                hearts[i].SetActive(false);
            }
        }

        previousHealth = currentHealth;
    }
}
