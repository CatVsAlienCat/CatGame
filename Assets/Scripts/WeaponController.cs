using UnityEngine;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public List<WeaponData> weapons;
    public Transform firePoint;
    
    public event System.Action<WeaponData> OnWeaponChanged;

    private int currentWeaponIndex = 0;
    private float nextFireTime = 0f;

    void Start()
    {
        if (weapons == null) weapons = new List<WeaponData>();
        if (weapons.Count > 0)
        {
            OnWeaponChanged?.Invoke(weapons[currentWeaponIndex]);
        }
    }

    public void AddWeapon(WeaponData newWeapon)
    {
        if (!weapons.Contains(newWeapon))
        {
            weapons.Add(newWeapon);
            // Optionally switch to new weapon immediately
            currentWeaponIndex = weapons.Count - 1;
            OnWeaponChanged?.Invoke(weapons[currentWeaponIndex]);
        }
    }

    void Update()
    {
        HandleShooting();
        HandleWeaponSwap();
    }

    void HandleShooting()
    {
        if (weapons.Count == 0) return;

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + weapons[currentWeaponIndex].fireRate;
        }
    }

    void HandleWeaponSwap()
    {
        if (weapons.Count <= 1) return;

        // Previous Weapon
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0) currentWeaponIndex = weapons.Count - 1;
            OnWeaponChanged?.Invoke(weapons[currentWeaponIndex]);
        }

        // Next Weapon
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentWeaponIndex++;
            if (currentWeaponIndex >= weapons.Count) currentWeaponIndex = 0;
            OnWeaponChanged?.Invoke(weapons[currentWeaponIndex]);
        }

        // Number Keys 1-9
        for (int i = 0; i < weapons.Count && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                currentWeaponIndex = i;
                OnWeaponChanged?.Invoke(weapons[currentWeaponIndex]);
            }
        }
    }

    void Shoot()
    {
        WeaponData currentWeapon = weapons[currentWeaponIndex];
        if (currentWeapon.bulletPrefab == null || firePoint == null) return;

        AudioManager.Instance.PlaySFX(currentWeapon.shootSound, currentWeapon.shootVolume);

        if (currentWeapon.isMelee)
        {
            // Melee Attack Logic
            // Instantiate as child of firePoint
            GameObject hitbox = Instantiate(currentWeapon.bulletPrefab, firePoint);
            // Apply spawn distance (move forward along Y axis)
            hitbox.transform.localPosition = new Vector3(0, currentWeapon.spawnDistance, 0);
            // Reset rotation to match parent (optional, but good for consistency)
            hitbox.transform.localRotation = Quaternion.identity;
            
            MeleeAttack meleeScript = hitbox.GetComponent<MeleeAttack>();
            if (meleeScript != null)
            {
                meleeScript.damage = currentWeapon.damage;
                meleeScript.lifeTime = currentWeapon.bulletLifeTime; // Use lifetime for duration
                meleeScript.isPlayerAttack = true;
                meleeScript.knockbackForce = currentWeapon.knockbackForce;
            }
        }
        else
        {
            // Ranged Attack Logic
            GameObject bullet = Instantiate(currentWeapon.bulletPrefab, firePoint.position, firePoint.rotation);
            
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.speed = currentWeapon.bulletSpeed;
                bulletScript.lifeTime = currentWeapon.bulletLifeTime;
                bulletScript.damage = currentWeapon.damage;
                bulletScript.isPlayerBullet = true;
                bulletScript.knockbackForce = currentWeapon.knockbackForce;
            }

            // Ignore collision between bullet and player
            Collider2D playerCollider = GetComponent<Collider2D>();
            Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
            if (playerCollider != null && bulletCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, bulletCollider);
            }
        }
    }
}
