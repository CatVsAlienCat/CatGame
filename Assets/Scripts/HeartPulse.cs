using UnityEngine;

public class HeartPulse : MonoBehaviour
{
    // Settings you can change in the Inspector
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    public float speed = 2.0f;

    void Update()
    {
        // This math creates a smooth wave between -1 and 1
        float wave = Mathf.Sin(Time.time * speed); 
        
        // We map that wave to your min/max scale
        float scale = Mathf.Lerp(minScale, maxScale, (wave + 1f) / 2f);
        
        // Apply the scale
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}