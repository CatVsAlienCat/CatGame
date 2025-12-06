using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon System/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject bulletPrefab;
    public float fireRate = 0.5f;
    public int damage = 1;
    public float bulletSpeed = 20f;
    public float bulletLifeTime = 2f;
    public bool isMelee = false;
    public float spawnDistance = 0f; // Distance from player center
    public float knockbackForce = 5f;
    public Sprite icon;
    public AudioClip shootSound;
    [Range(0f, 1f)]
    public float shootVolume = 1f;
}
