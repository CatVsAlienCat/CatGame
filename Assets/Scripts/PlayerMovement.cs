using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; 
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer weaponRenderer;

    [Header("Health")]
    public int maxHealth = 5;
    public int Health { get; private set; }

    public static event System.Action<int, int> OnHealthChanged;

    [Header("Animation Settings")]
    [Tooltip("Tiempo en segundos entre frames (menor es más rápido)")]
    public float animationSpeed = 0.2f;
    private float animationTimer;
    private int currentFrame;

    [Header("Sprite Lists")]
    public Sprite[] walkUpSprites;
    public Sprite[] walkDownSprites;
    public Sprite[] walkSideSprites;

    // Listas de sprites del arma actual
    private Sprite[] currentWalkUp;
    private Sprite[] currentWalkDown;
    private Sprite[] currentWalkSide;
    
    // Sprites de ataque
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
    public Transform firePoint;     

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Crear hijo para el sprite del arma
        Transform weaponTransform = transform.Find("WeaponSprite");
        if (weaponTransform == null)
        {
            GameObject weaponObj = new GameObject("WeaponSprite");
            weaponObj.transform.parent = transform;
            weaponObj.transform.localPosition = Vector3.zero;
            weaponRenderer = weaponObj.AddComponent<SpriteRenderer>();
            weaponRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
        }
        else
        {
            weaponRenderer = weaponTransform.GetComponent<SpriteRenderer>();
        }

        ResetWeaponSprites();
    }

    void Start()
    {
        Health = maxHealth;
        OnHealthChanged?.Invoke(Health, maxHealth);
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                isAttacking = false;
            }
        }
        
        HandleAnimationFrame();
        UpdateOrientation();
    }

    void HandleAnimationFrame()
    {
        if (moveInput != Vector2.zero)
        {
            animationTimer += Time.deltaTime;
            
            if (animationTimer >= animationSpeed)
            {
                animationTimer = 0f;
                currentFrame++;

                AudioManager.Instance.PlayRandomSFX(footstepSounds, footstepVolume);
            }
        }
        else
        {
            currentFrame = 0;
            animationTimer = 0f;
        }
    }

    void UpdateOrientation()
    {
        if (moveInput == Vector2.zero)
        {
            return;
        }

        // Lógica del sprite base (cuerpo del gato)
        Sprite baseSprite = null;
        bool flip = false;

        if (moveInput.y > 0.5f)
        {
            if (walkUpSprites.Length > 0) baseSprite = walkUpSprites[currentFrame % walkUpSprites.Length];
            flip = false;
        }
        else if (moveInput.y < -0.5f)
        {
            if (walkDownSprites.Length > 0) baseSprite = walkDownSprites[currentFrame % walkDownSprites.Length];
            flip = false;
        }
        else
        {
            if (walkSideSprites.Length > 0) baseSprite = walkSideSprites[currentFrame % walkSideSprites.Length];
            
            if (moveInput.x > 0) flip = false;
            else if (moveInput.x < 0) flip = true;
        }

        if (baseSprite != null)
        {
            spriteRenderer.sprite = baseSprite;
            spriteRenderer.flipX = flip;
        }

        // Lógica del sprite del arma
        if (weaponRenderer != null)
        {
            Sprite weaponSprite = null;
            Sprite[] targetWeaponSet = null;

            if (moveInput.y > 0.5f)
            {
                targetWeaponSet = isAttacking && attackUp != null && attackUp.Length > 0 ? attackUp : currentWalkUp;
            }
            else if (moveInput.y < -0.5f)
            {
                 targetWeaponSet = isAttacking && attackDown != null && attackDown.Length > 0 ? attackDown : currentWalkDown;
            }
            else
            {
                 targetWeaponSet = isAttacking && attackSide != null && attackSide.Length > 0 ? attackSide : currentWalkSide;
            }

            if (targetWeaponSet != null && targetWeaponSet.Length > 0)
            {
                weaponSprite = targetWeaponSet[currentFrame % targetWeaponSet.Length];
            }

            weaponRenderer.sprite = weaponSprite;
            weaponRenderer.flipX = flip;
            
            if (weaponRenderer.transform.parent != transform)
            {
                weaponRenderer.transform.parent = transform;
            }
            weaponRenderer.transform.localPosition = Vector3.zero; 
            weaponRenderer.transform.localScale = Vector3.one;
            weaponRenderer.transform.localRotation = Quaternion.identity;
        }

        // Rotación del punto de disparo (8 direcciones)
        float rawAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(rawAngle / 45f) * 45f;
        firePoint.rotation = Quaternion.Euler(0, 0, snappedAngle - 90f);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    public GameOverUI gameOverUI;

    public void Hit(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0;
        OnHealthChanged?.Invoke(Health, maxHealth);

        if (this.Health <= 0)
        {
            if (gameOverUI != null)
            {
                gameOverUI.ShowGameOver();
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
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
        if (attackUp != null || attackDown != null || attackSide != null)
        {
        }
    }
}