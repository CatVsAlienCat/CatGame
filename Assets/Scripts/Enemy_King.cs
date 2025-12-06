using UnityEngine;

public class Enemy_King : Enemies_behavior
{
    private float king_speed=3f;
    private float distanceRange_king = 10.0f;
    private float visionRange_king = 10.0f; 
   

     void Update()
    {
        MoveTowardsPlayer(king_speed, distanceRange_king, visionRange_king);
        

    }
}
