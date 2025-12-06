// This comment is added to force re-import
using UnityEngine;
using System.Collections;

public abstract class Enemies_behavior : MonoBehaviour
{
   public  Transform Player;
   public int Health;
  public float shootCooldown = 2f;

    public float distanceRange = 5.0f;
    public float visionRange = 5.0f; // New variable for following distance
   
   public GameObject bulletPrefab;

   public Transform player_pos;

   public Transform firePoint;
   public Vector2 Direction;
   private float LastShoot;
   
   
   private SpriteRenderer spriteRenderer;

   [Header("Sprites")]
   public Sprite spriteUp;
   public Sprite spriteDown;
    public Sprite spriteRight;

    [Header("Audio")]
    public AudioClip hitSound;
    [Range(0f, 1f)]
    public float hitVolume = 1f;
    public AudioClip shootSound;
    [Range(0f, 1f)]
    public float shootVolume = 1f;

    [Header("Drops")]
    public GameObject healthPickupPrefab;
    [Range(0f, 1f)]
    public float dropChance = 0.2f;

    [Header("Death Animation")]
    public Sprite[] deathSprites;
    public float deathAnimationSpeed = 0.1f;
    protected bool isDying = false;

    private Rigidbody2D rb;
    private bool isKnockedBack = false;

    protected void Awake()
    {
        // Intenta encontrar al player si no está asignado
        if (Player == null)
        {
            FindPlayer();
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        // Validar referencias críticas
        if (firePoint == null) Debug.LogWarning($"FirePoint no asignado en {gameObject.name}");
        if (bulletPrefab == null) Debug.LogWarning($"BulletPrefab no asignado en {gameObject.name}");

        StartCoroutine(ShootWithCooldown());
    }


    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            Player = playerObj.transform;
        }
    }


    protected virtual void MoveTowardsPlayer(float speed)
    {
        if (isKnockedBack || isDying) return;

        // Si no hay player, intentamos buscarlo de nuevo o salimos
        if (Player == null)
        {
            FindPlayer();
            if (Player == null) {
                // Debug.Log("Enemy: Player null, searching..."); // Commented to avoid spam
                return;
            } else {
                 Debug.Log("Enemy: Player found!");
            }
        }

        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
        
        // Debug.Log($"Enemy {gameObject.name}: Distance={distanceToPlayer}, Vision={visionRange}"); // Debug distance

        if (distanceToPlayer < visionRange)
        {
            Vector2 direction = (Player.position - transform.position).normalized;
            Vector2 target = new Vector2(Player.position.x, Player.position.y);
            Vector2 newPos = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            
            transform.position = newPos; // Explicit assignment
            
            UpdateOrientation(direction);
        }
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (rb != null)
        {
            isKnockedBack = true;
            rb.linearVelocity = Vector2.zero; // Reset velocity before applying force
            rb.AddForce(direction * force, ForceMode2D.Impulse);
            StartCoroutine(ResetKnockback());
        }
    }

    private IEnumerator ResetKnockback()
    {
        yield return new WaitForSeconds(0.2f); // Knockback duration
        isKnockedBack = false;
        if (rb != null) rb.linearVelocity = Vector2.zero; // Stop sliding
    }
    
    void UpdateOrientation(Vector2 moveInput)
    {
        if (moveInput == Vector2.zero) return;
        if (spriteRenderer == null) return; 

        // --- Sprite Logic ---
        // Prioritize vertical sprites
        if (moveInput.y > 0.5f)
        {
            if(spriteUp != null) spriteRenderer.sprite = spriteUp;
            spriteRenderer.flipX = false;
        }
        else if (moveInput.y < -0.5f)
        {
            if(spriteDown != null) spriteRenderer.sprite = spriteDown;
            spriteRenderer.flipX = false;
        }
        // Horizontal sprites only if not moving much vertically
        else
        {
            if (moveInput.x > 0)
            {
                if(spriteRight != null) spriteRenderer.sprite = spriteRight;
                spriteRenderer.flipX = false;
            }
            else if (moveInput.x < 0)
            {
                if(spriteRight != null) spriteRenderer.sprite = spriteRight;
                spriteRenderer.flipX = true;
            }
        }
        
        // --- Rotation Logic for Shooting ---
        if (firePoint != null)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg - 90f;
            firePoint.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    
    public virtual void Hit(int damage)
    {
        if (isDying) return; // Ignore hits while dying

        AudioManager.Instance.PlaySFX(hitSound, hitVolume);
        this.Health -= damage;
        if (this.Health <= 0)
        {
            StartCoroutine(DieRoutine());
        }
    }

    protected IEnumerator DieRoutine()
    {
        isDying = true;
        
        // Disable physics and collision to prevent further interaction
        if (rb != null) rb.simulated = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Play drop logic immediately
        TryDropItem();

        // Play clean animation if sprites are assigned
        if (deathSprites != null && deathSprites.Length > 0 && spriteRenderer != null)
        {
            foreach (var sprite in deathSprites)
            {
                spriteRenderer.sprite = sprite;
                yield return new WaitForSeconds(deathAnimationSpeed);
            }
        }

        Destroy(gameObject);
    }

    protected virtual void TryDropItem()
    {
        if (healthPickupPrefab != null && Random.value <= dropChance)
        {
            Instantiate(healthPickupPrefab, transform.position, Quaternion.identity);
        }
    }

    
    protected virtual void shoot()
    {
        if (Player == null || firePoint == null || bulletPrefab == null || isDying) return;

        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        if (distanceToPlayer < distanceRange)
        {
            AudioManager.Instance.PlaySFX(shootSound, shootVolume);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.isPlayerBullet = false; // Explicitly set as an enemy bullet
            }
        }
    }

    IEnumerator ShootWithCooldown()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootCooldown);
            shoot();
        }
    }
    
    
    
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
