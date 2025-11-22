using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    private Slider healthSlider;

    void Awake()
    {
        healthSlider = GetComponent<Slider>();
    }

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
        if (maxHealth > 0)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }
}
