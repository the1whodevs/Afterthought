using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Veejay/Weapons/Weapon", fileName = "Weapon")]
public class WeaponData : IDisplayableItem
{
    public enum WeaponType { Firearm, Projectile, Melee }
    
    public enum FireType { FullAuto, Burst, SemiAuto, BoltAction }

    public GameObject RandomHitDecal 
    {
        get
        {
            if (hitDecal.Length > 0) return hitDecal[Random.Range(0, hitDecal.Length)];

            return null;
        }
    }
    public AudioClip RandomShotAudioFX => shotAudioFx[Random.Range(0, shotAudioFx.Length)];
        
    public GameObject wepPrefab;
    public GameObject pickablePrefab;
    public GameObject projectilePrefab;
    public GameObject muzzleEffect;
    
    public AudioClip reloadAudioFx;
    public AudioClip emptyClipAudioFx;

    public bool isShotgun = false;
    
    public bool hasScope = false;
    public PlayerCamera.ZoomLevels scopeZoom = PlayerCamera.ZoomLevels.X4;
    
    [FormerlySerializedAs("audioFx")] public AudioClip[] shotAudioFx;
    [FormerlySerializedAs("bulletHole")] public GameObject hitImpact;
    public GameObject[] hitDecal;

    public WeaponTypeData weaponTypeData;
    public WeaponType weaponType = WeaponType.Firearm;

    public PlayerWeaponAnimator.WeaponAnimatorType weaponAnimType = PlayerWeaponAnimator.WeaponAnimatorType.Pistol;

    public FireType fireType = FireType.FullAuto;

    public int ammoInMagazine = 30;
    public int magazineCapacity = 30;

    public float projectileSpeed = 100.0f;
    public float weaponDamage = 0.0f;
    public float fireRate = 3.0f;
    public float shootAudioPitch = 2.0f;
    public float reloadSpeed = 1.0f;
    public float mobilityPenalty = 0.2f;
    public float adsMultiplier = 1.0f;
    public float minRange = 150.0f;
    public float maxRange = 300.0f;

    [Tooltip("The offset for localPosition to use, when aiming down sights.")]
    public Vector3 ads_offset = Vector3.zero;

    public float recoil_horizontal = 1.5f;
    public float recoil_vertical = 3.0f;
    public float recoil_speed = 2.5f;
    public float recoil_recovery = 0.5f;

    private string AMMO_IN_MAG => $"{name}_AMMOinMAG";

    public void ReloadMag(ref int availableAmmo)
    {
        if (availableAmmo == 0)
        {
            Debug.Log("Trying to reload with 0 ammo left");
            return;
        }

        var bulletsForMax = magazineCapacity - ammoInMagazine;
        
        //  Removed warning as it will most likely hit on high reload speeds.
        // if (bulletsForMax == 0) Debug.LogError("Reloading while the mag is full!");
        
        if (availableAmmo >= bulletsForMax)
        {
            availableAmmo -= bulletsForMax;
            ammoInMagazine = magazineCapacity;
        }
        else
        {
            ammoInMagazine = availableAmmo;
            availableAmmo = 0;
        }
    }
}