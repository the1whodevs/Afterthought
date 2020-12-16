using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Veejay/Weapon", fileName = "Weapon")]
public class WeaponData : ScriptableObject
{
    public enum WeaponType {Firearm, Projectile, Melee}
    public enum FireType { FullAuto, Burst, SemiAuto, BoltAction }

    [FormerlySerializedAs("rightHandPrefab")] public new string name;
    public string description;
    public Sprite image;
    
    public GameObject wepPrefab;
    public GameObject projectilePrefab;
    public GameObject muzzleEffect;
    public GameObject bulletHole;

    public WeaponType weaponType = WeaponType.Firearm;
    
    public FireType fireType = FireType.FullAuto;

    public int currentAmmo = 30;
    public int magazineCapacity = 30;

    public float projectileSpeed = 100.0f;
    public float weaponDamage = 0.0f;
    public float fireRate = 3.0f;
    public float reloadSpeed = 1.0f;
    public float mobilityPenalty = 0.2f;
    public float adsMultiplier = 1.0f;

    private string AMMO_IN_MAG => $"{name}_AMMOinMAG";
    
    public void ReloadMag(ref int availableAmmo)
    {
        if (availableAmmo >= magazineCapacity)
        {
            availableAmmo -= magazineCapacity;
            currentAmmo = magazineCapacity;
        }
        else
        {
            currentAmmo = availableAmmo;
            availableAmmo = 0;
        }
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt(AMMO_IN_MAG, currentAmmo);
    }

    public void LoadData()
    {
        currentAmmo = PlayerPrefs.GetInt(AMMO_IN_MAG, magazineCapacity);
    }
}