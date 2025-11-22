// This comment is added to force re-import
using UnityEngine;
using System.Collections;

public abstract class Enemies_behavior : MonoBehaviour
{
   public  Transform Player;
   public int Health = 3;
   public GameObject bulletPrefab;

   public Transform player_pos;

   public Transform firePoint;
   public Vector2 Direction;
   private float LastShoot;
   public float shootCooldown = 2f;
   
   private SpriteRenderer spriteRenderer;

   [Header("Sprites")]
   public Sprite spriteUp;
   public Sprite spriteDown;
   public Sprite spriteRight;



    protected void Start()
    {
        player_pos = GameObject.FindWithTag("Player")?.transform;
        if (Player == null)
        {
            Player = player_pos;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(ShootWithCooldown());
    }

    protected virtual void MoveTowardsPlayer(float speed)
    {
        Vector2 direction = (Player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position,new Vector2(Player.position.x,Player.position.y),speed*Time.deltaTime);
        UpdateOrientation(direction);
    }
    
    void UpdateOrientation(Vector2 moveInput)
    {
        if (moveInput == Vector2.zero)
        {
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
    
    public virtual void Hit(int damage)
    {
        this.Health -= damage;
        if (this.Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    
    protected virtual void shoot()
    {
       GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
       bullet.GetComponent<Bullet>().isPlayerBullet = false; // Explicitly set as an enemy bullet
    }

    IEnumerator ShootWithCooldown()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootCooldown);
            shoot();
        }
    }
    
    
    
}
