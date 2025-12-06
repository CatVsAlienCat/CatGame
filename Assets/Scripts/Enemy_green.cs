using UnityEngine;

public class Enemy_green: Enemies_behavior
{
    
    private float green_speed=1f;
    private float distanceRange_green = 5.0f;
    private float visionRange_green = 5.0f; 
    
     void Update()
    {
        MoveTowardsPlayer(green_speed, distanceRange_green, visionRange_green);
      
    }
}

