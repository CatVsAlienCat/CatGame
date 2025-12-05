using UnityEngine;

public class Enemy_green: Enemies_behavior
{
    
    private float green_speed=2f;

     void Update()
    {
        MoveTowardsPlayer(green_speed);
    }
}

