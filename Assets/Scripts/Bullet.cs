using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public int damage = 1;
    public bool isPlayerBullet; // This flag will distinguish player bullets from enemy bullets
    public float knockbackForce = 0f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime); // lifeTime is now set by WeaponController

        rb.linearVelocity = transform.up * speed; // speed is now set by WeaponController
    }

     void OnTriggerEnter2D(Collider2D other)
    {
        // Player bullets damage enemies
        if (isPlayerBullet)
        {
            Enemies_behavior enemy = other.GetComponent<Enemies_behavior>();
            if (enemy != null)
            {
                enemy.Hit(damage);
                
                // Apply Knockback
                if (knockbackForce > 0)
                {
                    Vector2 direction = (enemy.transform.position - transform.position).normalized;
                    // Or use bullet velocity direction for more realistic impact:
                    // Vector2 direction = rb.linearVelocity.normalized; 
                    enemy.ApplyKnockback(direction, knockbackForce);
                }

                Destroy(gameObject);
            }
        }
        // Enemy bullets damage the player
        else
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.Hit(damage);
                Destroy(gameObject);
            }
        }
        // Also destroy bullet if it hits something else, like a wall
        if (other.GetComponent<PlayerMovement>() == null && other.GetComponent<Enemies_behavior>() == null){
            Destroy(gameObject);
        }
    }
}