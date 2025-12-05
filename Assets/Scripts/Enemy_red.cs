// This comment is added to force re-import
using UnityEngine;

public class Enemy_red : Enemies_behavior
{
    private float red_speed=4f;
   
    void Update()
    {
        MoveTowardsPlayer(red_speed);

    }
    
}
