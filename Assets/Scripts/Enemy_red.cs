// This comment is added to force re-import
using UnityEngine;

public class Enemy_red : Enemies_behavior
{
    private float red_speed=1f;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer(red_speed);
    }
}
