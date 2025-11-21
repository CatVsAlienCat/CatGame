using UnityEngine;

public class Enemy_red : Enemies_behavior
{
    private Rigidbody2D rb;
    private float red_speed=1f;
    private int Health=5;

    private float speedBullet=20f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer(red_speed);
        Hit(Health);
        //shoot( speedBullet);
    }
}
