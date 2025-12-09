using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public int damage = 1;
    public bool isPlayerBullet;
    public float knockbackForce = 0f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);

        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }
        else
        {
            Debug.LogError("Bullet: No se encontró Rigidbody2D en el objeto.");
        }
    }

     void OnTriggerEnter2D(Collider2D other)
    {
        // Las balas del jugador dañan a los enemigos
        if (isPlayerBullet)
        {
            Enemies_behavior enemy = other.GetComponent<Enemies_behavior>();
            if (enemy != null)
            {
                enemy.Hit(damage);
                
                // Aplicar retroceso
                if (knockbackForce > 0)
                {
                    Vector2 direction = (enemy.transform.position - transform.position).normalized;
                    enemy.ApplyKnockback(direction, knockbackForce);
                }

                Destroy(gameObject);
            }
        }
        // Las balas enemigas dañan al jugador
        else
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.Hit(damage);
                Destroy(gameObject);
            }
        }
        // Destruir bala si golpea algo más (ej: paredes)
        if (other.GetComponent<PlayerMovement>() == null && other.GetComponent<Enemies_behavior>() == null){
            Destroy(gameObject);
        }
    }
}