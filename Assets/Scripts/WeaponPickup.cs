using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weapon;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponController controller = other.GetComponent<WeaponController>();
            if (controller != null && weapon != null)
            {
                controller.AddWeapon(weapon);
                Destroy(gameObject);
            }
        }
    }
}
