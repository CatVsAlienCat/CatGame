using UnityEngine;

public class Enemy_green: Enemies_behavior
{
    private float green_speed=0.5f;

     void Update()
    {
        MoveTowardsPlayer(green_speed);
    }
}

