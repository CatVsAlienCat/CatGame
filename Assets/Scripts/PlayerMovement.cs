using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; 
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;

    [Header("Health")]
    public int maxHealth = 5;
    public int Health { get; private set; }

    public static event System.Action<int, int> OnHealthChanged;

    [Header("Animation Settings")]
    [Tooltip("Time in seconds between frames (lower is faster)")]
    public float animationSpeed = 0.2f;
    private float animationTimer;
    private int currentFrame;

    [Header("Sprite Lists")]
    // Changed these from single Sprites to Arrays []
    public Sprite[] walkUpSprites;
    public Sprite[] walkDownSprites;
    public Sprite[] walkSideSprites; // Used for both Right and Left

    [Header("Combat")]
    public GameObject bulletPrefab; 
    public Transform firePoint;     

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Health = maxHealth;
        OnHealthChanged?.Invoke(Health, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        // Get input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;

        // Calculate Animation Frame
        HandleAnimationFrame();

        // Update Sprite image and rotation based on input
        UpdateOrientation();

        // Check for Left-Click
        if (Input.GetButtonDown("Fire1")) 
        {
            Shoot();
        }
    }

    void HandleAnimationFrame()
    {
        // If we are moving, cycle through frames
        if (moveInput != Vector2.zero)
        {
            animationTimer += Time.deltaTime;
            
            if (animationTimer >= animationSpeed)
            {
                animationTimer = 0f;
                currentFrame++; 
                // We don't limit the frame here, we use modulo (%) later 
                // to wrap around whatever array length we are using
            }
        }
        else
        {
            // If standing still, reset to the first frame (Idle)
            currentFrame = 0;
            animationTimer = 0f;
        }
    }

    void UpdateOrientation()
    {
        if (moveInput == Vector2.zero)
        {
            // When standing still, we still want to show the idle frame (frame 0) 
            // of the last direction we faced, but for simplicity here we just return.
            // If you want "Idle" animations, remove this return and add logic to remember last direction.
            return;
        }

        // --- Sprite Logic ---
        
        // UP
        if (moveInput.y > 0.5f)
        {
            spriteRenderer.flipX = false;
            if (walkUpSprites.Length > 0) 
            {
                // The % operator ensures we loop back to 0 if currentFrame exceeds array length
                spriteRenderer.sprite = walkUpSprites[currentFrame % walkUpSprites.Length];
            }
        }
        // DOWN
        else if (moveInput.y < -0.5f)
        {
            spriteRenderer.flipX = false;
            if (walkDownSprites.Length > 0)
            {
                spriteRenderer.sprite = walkDownSprites[currentFrame % walkDownSprites.Length];
            }
        }
        // SIDEWAYS (Right or Left)
        else
        {
            if (walkSideSprites.Length > 0)
            {
                spriteRenderer.sprite = walkSideSprites[currentFrame % walkSideSprites.Length];
            }

            if (moveInput.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (moveInput.x < 0)
            {
                spriteRenderer.flipX = true;
            }
        }

        // --- Rotation Logic for Shooting ---
        float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg - 90f;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        // Ensure your Bullet script has this variable public, or remove this line if not needed yet
        if(bullet.GetComponent<Bullet>() != null)
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