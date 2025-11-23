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
        // Make the bullet a trigger to avoid physics interactions
        GetComponent<Collider2D>().isTrigger = true;
        Destroy(gameObject, lifeTime);

        rb.linearVelocity = transform.up * speed;

        // If this is a player bullet, ignore collisions with the player
        if (isPlayerBullet)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // Player bullets damage enemies
        if (isPlayerBullet)
        {
            Enemies_behavior enemy = otherCollider.GetComponent<Enemies_behavior>();
            if (enemy != null)
            {
                enemy.Hit(damage);
                Destroy(gameObject);
            }
        }
        // Enemy bullets damage the player
        else
        {
            PlayerMovement player = otherCollider.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.Hit(damage);
                Destroy(gameObject);
            }
        }
        // Also destroy bullet if it hits something else, like a wall
        if (otherCollider.GetComponent<PlayerMovement>() == null && otherCollider.GetComponent<Enemies_behavior>() == null){
            Destroy(gameObject);
        }
    }
}