using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // You can change this speed in the Unity Inspector
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;

    private Vector2 Direction;

    [Header("Health")]
    public int maxHealth = 5;
    public int Health { get; private set; }

    public static event System.Action<int, int> OnHealthChanged;


    [Header("Sprites")]
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteRight;

    public GameObject bulletPrefab; // The prefab you just made
    public Transform firePoint;     // An empty GameObject at the "tip" of your player

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody component we attached to this player
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Health = maxHealth;
        OnHealthChanged?.Invoke(Health, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from keyboard (Arrow keys or WASD)
        float moveX = Input.GetAxisRaw("Horizontal"); // -1 for left, 1 for right
        float moveY = Input.GetAxisRaw("Vertical");   // -1 for down, 1 for up

        moveInput = new Vector2(moveX, moveY).normalized; // .normalized stops you from moving faster diagonally

        UpdateOrientation();

        // Check for Left-Click
        if (Input.GetButtonDown("Fire1")) // "Fire1" is Left-Click by default
        {
            Shoot();
        }
    }

    void UpdateOrientation()
    {
        if (moveInput == Vector2.zero)
        {
            // Optional: Don't change orientation when standing still
            return;
        }

        // --- Sprite Logic ---
        // Prioritize vertical sprites
        if (moveInput.y > 0.5f)
        {
            spriteRenderer.sprite = spriteUp;
            spriteRenderer.flipX = false;
        }
        else if (moveInput.y < -0.5f)
        {
            spriteRenderer.sprite = spriteDown;
            spriteRenderer.flipX = false;
        }
        // Horizontal sprites only if not moving much vertically
        else
        {
            if (moveInput.x > 0)
            {
                spriteRenderer.sprite = spriteRight;
                spriteRenderer.flipX = false;
            }
            else if (moveInput.x < 0)
            {
                spriteRenderer.sprite = spriteRight;
                spriteRenderer.flipX = true;
            }
        }

        // --- Rotation Logic for Shooting ---
        float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg - 90f;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    // FixedUpdate is called on a fixed physics timer (better for physics)
    void FixedUpdate()
    {
        // Apply the movement to the Rigidbody's velocity
        rb.linearVelocity = moveInput * moveSpeed;


    }

    void Shoot()
    {
        // Create a new bullet from the prefab, at the firePoint's position and rotation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().isPlayerBullet = true;
    }

    public void Hit(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0;
        OnHealthChanged?.Invoke(Health, maxHealth);

        if (this.Health <= 0)
        {
            Destroy(gameObject);
        }
    }

}