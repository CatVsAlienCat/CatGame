using UnityEngine;
using System.Collections;

public abstract class Enemies_behavior : MonoBehaviour
{
    public Transform Player;
    public int Health;
    public float shootCooldown;

    public float distanceRange;
    public float visionRange;
   
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
        if (Player == null)
        {
            FindPlayer();
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        if (firePoint == null) Debug.LogWarning($"FirePoint no asignado en {gameObject.name}");
        if (bulletPrefab == null) Debug.LogWarning($"BulletPrefab no asignado en {gameObject.name}");

        StartCoroutine(ShootWithCooldown(shootCooldown));
    }


    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            Player = playerObj.transform;
        }
    }


    protected virtual void MoveTowardsPlayer(float speed, float distanceRange, float visionRange)
    {
        if (isKnockedBack || isDying) return;

        if (Player == null)
        {
            FindPlayer();
            if (Player == null)
            {
                return;
            }
            else
            {
                Debug.Log("Enemigo: ¡Jugador encontrado!");
            }
        }

        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        if (distanceToPlayer < visionRange)
        {
            Vector2 direction = (Player.position - transform.position).normalized;
            Vector2 target = new Vector2(Player.position.x, Player.position.y);
            Vector2 newPos = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            
            transform.position = newPos;
            
            UpdateOrientation(direction);
        }
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (rb != null)
        {
            isKnockedBack = true;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(direction * force, ForceMode2D.Impulse);
            StartCoroutine(ResetKnockback());
        }
    }

    private IEnumerator ResetKnockback()
    {
        yield return new WaitForSeconds(0.2f);
        isKnockedBack = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }
    
    void UpdateOrientation(Vector2 moveInput)
    {
        if (moveInput == Vector2.zero) return;
        if (spriteRenderer == null) return; 

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
        
        if (firePoint != null)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg - 90f;
            firePoint.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    
    public virtual void Hit(int damage)
    {
        AudioManager.Instance.PlaySFX(hitSound);
        Health -= damage;
        if (Health <= 0)
        {
            StartCoroutine(DieRoutine());
        }
    }

    protected IEnumerator DieRoutine()
    {
        isDying = true;
        
        if (rb != null) rb.simulated = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        TryDropItem();

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
                bulletScript.isPlayerBullet = false;
            }
        }
    }

    protected virtual IEnumerator ShootWithCooldown(float shootCooldown)
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
