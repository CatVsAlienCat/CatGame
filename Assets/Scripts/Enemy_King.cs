using UnityEngine;

public class Enemy_King : Enemies_behavior
{
    private float king_speed=3f;

     void Update()
    {
        MoveTowardsPlayer(king_speed);
    }
}
