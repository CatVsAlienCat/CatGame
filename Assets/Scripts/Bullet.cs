using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    private Rigidbody2D rb;
    private Vector2 Direction;
    private int Health;

    // Start is called before the first frame update
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed;
        Destroy(gameObject, lifeTime);
    }
    public void DestroyBullet()
    {
        Destroy (gameObject);
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();
        Enemies_behavior enemy = collision.collider.GetComponent<Enemies_behavior>();
        if (enemy != null)
        {
            enemy.Hit(Health);
        }
        if (player != null)
        {
            player.Hit(Health);
        }
        DestroyBullet();

    }

/*
    public void FixedUpdate()
    {
        rb.linearVelocity = Direction*speed;
    }
    public void SetDirection(Vector2 direction)
    {
        Direction=direction;
    }
    */
}
