using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // "up" is now forward for our rotated player
        rb.linearVelocity = transform.up * bulletSpeed; 

        // Destroy the bullet after 3 seconds so it doesn't fly forever
        Destroy(gameObject, 3f); 
    }
}