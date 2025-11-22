using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public int damage = 1;
    public bool isPlayerBullet; // This flag will distinguish player bullets from enemy bullets

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);

        rb.linearVelocity = transform.up * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Player bullets damage enemies
        if (isPlayerBullet)
        {
            Enemies_behavior enemy = collision.collider.GetComponent<Enemies_behavior>();
            if (enemy != null)
            {
                enemy.Hit(damage);
                Destroy(gameObject);
            }
        }
        // Enemy bullets damage the player
        else
        {
            PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.Hit(damage);
                Destroy(gameObject);
            }
        }
        // Also destroy bullet if it hits something else, like a wall
        if (collision.collider.GetComponent<PlayerMovement>() == null && collision.collider.GetComponent<Enemies_behavior>() == null){
            Destroy(gameObject);
        }
    }
}