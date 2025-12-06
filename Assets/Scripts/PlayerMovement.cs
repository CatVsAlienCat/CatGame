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

    [Header("Audio")]
    public AudioClip[] footstepSounds;
    [Range(0f, 1f)]
    public float footstepVolume = 1f;

    [Header("Combat")]
    // public GameObject bulletPrefab; // Removed - handled by WeaponController
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
        // Shooting input removed - handled by WeaponController
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

                AudioManager.Instance.PlayRandomSFX(footstepSounds, footstepVolume);
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
        // Snap to 8 directions (45 degrees)
        float rawAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(rawAngle / 45f) * 45f;
        firePoint.rotation = Quaternion.Euler(0, 0, snappedAngle - 90f);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    // Shoot method removed - handled by WeaponController

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