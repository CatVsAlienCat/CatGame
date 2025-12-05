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
