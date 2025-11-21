using UnityEngine;

public abstract class Enemies_behavior : MonoBehaviour
{
    private Rigidbody2D rb;
   public  Transform Player;
   private int Health;
   public GameObject bulletPrefab;

   public Transform player_pos;

   private float speedBullet;

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
       Vector3 direction = Player.position - transform.position;
       direction.Normalize(); 
       GameObject bullet = Instantiate(bulletPrefab, transform.position + direction + 0.1f,Quaternion.identity);
       bullet.GetComponent<Bullet>().SetDirection(direction * bulletSpeed);
       
    }  
    
    
}
