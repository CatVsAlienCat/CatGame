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

    // Override Lists (from Weapon)
    private Sprite[] currentWalkUp;
    private Sprite[] currentWalkDown;
    private Sprite[] currentWalkSide;
    
    // Attack Sprite Lists
    private Sprite[] attackUp;
    private Sprite[] attackDown;
    private Sprite[] attackSide;
    
    private bool isAttacking = false;
    private float attackTimer = 0f;

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

        // Initialize overrides with defaults
        ResetWeaponSprites();

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
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                isAttacking = false;
            }
        }
        
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
        
        // --- Sprite Logic ---
        
        // Determine which set of sprites to use
        Sprite[] targetUp = isAttacking && attackUp != null && attackUp.Length > 0 ? attackUp : (currentWalkUp ?? walkUpSprites);
        Sprite[] targetDown = isAttacking && attackDown != null && attackDown.Length > 0 ? attackDown : (currentWalkDown ?? walkDownSprites);
        Sprite[] targetSide = isAttacking && attackSide != null && attackSide.Length > 0 ? attackSide : (currentWalkSide ?? walkSideSprites);

        // UP
        if (moveInput.y > 0.5f)
        {
            spriteRenderer.flipX = false;
            if (targetUp.Length > 0) 
            {
                spriteRenderer.sprite = targetUp[currentFrame % targetUp.Length];
            }
        }
        // DOWN
        else if (moveInput.y < -0.5f)
        {
            spriteRenderer.flipX = false;
            if (targetDown.Length > 0)
            {
                spriteRenderer.sprite = targetDown[currentFrame % targetDown.Length];
            }
        }
        // SIDEWAYS (Right or Left)
        else
        {
            if (targetSide.Length > 0)
            {
                spriteRenderer.sprite = targetSide[currentFrame % targetSide.Length];
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

    public void Heal(int amount)
    {
        Health += amount;
        if (Health > maxHealth) Health = maxHealth;
        OnHealthChanged?.Invoke(Health, maxHealth);
    }

    public void SetWeaponSprites(WeaponData weapon)
    {
        if (weapon != null)
        {
            // If weapon has sprites, use them. Else null (fallback to default in UpdateOrientation)
            currentWalkUp = (weapon.walkUp != null && weapon.walkUp.Length > 0) ? weapon.walkUp : null;
            currentWalkDown = (weapon.walkDown != null && weapon.walkDown.Length > 0) ? weapon.walkDown : null;
            currentWalkSide = (weapon.walkSide != null && weapon.walkSide.Length > 0) ? weapon.walkSide : null;

            attackUp = weapon.attackUp;
            attackDown = weapon.attackDown;
            attackSide = weapon.attackSide;
        }
        else
        {
            ResetWeaponSprites();
        }
    }

    public void ResetWeaponSprites()
    {
        currentWalkUp = null;
        currentWalkDown = null;
        currentWalkSide = null;
        attackUp = null;
        attackDown = null;
        attackSide = null;
    }

    public void TriggerAttackAnimation(float duration)
    {
        isAttacking = true;
        attackTimer = duration;
        // Reset frame to 0 to start attack animation from start? 
        // Or keep walking frame? Usually attack is distinct. 
        // check if we have attack sprites first
        if (attackUp != null || attackDown != null || attackSide != null)
        {
             // Optional: reset frame index if you want animation to restart
             // currentFrame = 0; 
        }
    }
}