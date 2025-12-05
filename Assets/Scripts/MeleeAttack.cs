using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 0.2f;
    public bool isPlayerAttack = true;
    public float knockbackForce = 0f;

    [Header("Animation")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] animationFrames;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        if (animationFrames != null && animationFrames.Length > 0)
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            StartCoroutine(PlayAnimation());
        }
    }

    IEnumerator PlayAnimation()
    {
        float frameDuration = lifeTime / animationFrames.Length;
        for (int i = 0; i < animationFrames.Length; i++)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = animationFrames[i];
            }
            yield return new WaitForSeconds(frameDuration);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerAttack)
        {
            Enemies_behavior enemy = other.GetComponent<Enemies_behavior>();
            if (enemy != null)
            {
                enemy.Hit(damage);
                // Calculate knockback direction (away from player/hitbox center)
                Vector2 direction = (enemy.transform.position - transform.position).normalized;
                enemy.ApplyKnockback(direction, knockbackForce);
            }
        }
        else
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.Hit(damage);
            }
        }
    }
}
