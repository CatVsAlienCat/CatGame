using UnityEngine;

public abstract class Enemies_behavior : MonoBehaviour
{
    private Rigidbody2D rb;
   public  Transform Player;
   private int Health;
   public GameObject bulletPrefab;

   public Transform player_pos;

   private float speedBullet;
 public Transform firePoint;
   public Vector2 Direction;
   private float LastShoot;


    protected virtual void MoveTowardsPlayer(float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position,new Vector2(Player.position.x,Player.position.y),speed*Time.deltaTime);
    }
    
    public virtual void Hit(int Health){
        Health--;
        if (Health==0)
        {
            Destroy (gameObject);
        }
    }

    void Start()
    {
        player_pos = GameObject.Find("Player").transform;
    }
    
    
    protected virtual void shoot(float bulletSpeed)
    {
        
        
        float angle = Mathf.Atan2(Player.position.x,Player.position.y) * Mathf.Rad2Deg - 90f;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
        Direction=new Vector2(Player.position.x,Player.position.y);
       
       GameObject bullet = Instantiate(bulletPrefab, transform.position ,firePoint.rotation);
       bullet.GetComponent<Bullet>().SetDirection(Direction);


       

    }  
    
    
    
}
