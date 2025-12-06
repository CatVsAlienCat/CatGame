using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // Only pickup if we need health
                if (player.Health < player.maxHealth)
                {
                    player.Heal(healAmount);
                    Destroy(gameObject);
                }
            }
        }
    }
}
