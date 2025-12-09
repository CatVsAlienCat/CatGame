using UnityEngine;

public class HeartPulse : MonoBehaviour
{
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    public float speed = 2.0f;

    void Update()
    {
        float wave = Mathf.Sin(Time.time * speed); 
        float scale = Mathf.Lerp(minScale, maxScale, (wave + 1f) / 2f);
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}