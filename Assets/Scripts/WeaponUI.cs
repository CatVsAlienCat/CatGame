using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public Image iconImage;
    public WeaponController weaponController;

    void Start()
    {
        if (weaponController != null)
        {
            weaponController.OnWeaponChanged += UpdateUI;
            
            // Manually update UI on startup to ensure icon shows
            if (weaponController.weapons != null && weaponController.weapons.Count > 0)
            {
                UpdateUI(weaponController.weapons[0]);
            }
        }
    }

    void OnDestroy()
    {
        if (weaponController != null)
        {
            weaponController.OnWeaponChanged -= UpdateUI;
        }
    }

    void UpdateUI(WeaponData weapon)
    {
        if (iconImage != null)
        {
            if (weapon != null && weapon.icon != null)
            {
                iconImage.sprite = weapon.icon;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.enabled = false;
            }
        }
    }
}
