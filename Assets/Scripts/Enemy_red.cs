using UnityEngine;

public class Enemy_red : Enemies_behavior
{
    private float red_speed=3f;
    private float distanceRange_red = 10f;
    private float visionRange_red = 10f; 

    void Update()
    {
        MoveTowardsPlayer(red_speed, distanceRange_red, visionRange_red);
    }
}
