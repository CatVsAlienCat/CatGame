using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Heart : MonoBehaviour
{
    [Header("UI References")]
    public Image heartImage;

    [Header("Sprites")]
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;
    public Sprite[] destructionFrames;

    [Header("Animation Settings")]
    public float animationSpeed = 0.1f;

    private void Awake()
    {
        if (heartImage == null)
        {
            heartImage = GetComponent<Image>();
        }
    }

    public void SetActive(bool active)
    {
        StopAllCoroutines();
        if (active)
        {
            heartImage.sprite = fullHeartSprite;
            heartImage.enabled = true;
        }
        else
        {
            if (emptyHeartSprite != null)
            {
                heartImage.sprite = emptyHeartSprite;
                heartImage.enabled = true;
            }
            else
            {
                heartImage.enabled = false;
            }
        }
    }

    public void PlayDestruction()
    {
        StartCoroutine(DestructionCoroutine());
    }

    private IEnumerator DestructionCoroutine()
    {
        heartImage.enabled = true;
        
        foreach (Sprite frame in destructionFrames)
        {
            heartImage.sprite = frame;
            yield return new WaitForSeconds(animationSpeed);
        }

        // After animation, set to empty state
        SetActive(false);
    }
}
