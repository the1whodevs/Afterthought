﻿using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Veejay/Weapon", fileName = "Weapon")]
public class WeaponData : IDisplayableItem
{
    public enum WeaponType { Firearm, Projectile, Melee }
    
    public enum FireType { FullAuto, Burst, SemiAuto, BoltAction }

    public GameObject RandomHitDecal => hitDecal[Random.Range(0, hitDecal.Length)];
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
    
    public WeaponType weaponType = WeaponType.Firearm;

    public PlayerWeaponAnimator.WeaponAnimatorType weaponAnimType = PlayerWeaponAnimator.WeaponAnimatorType.Pistol;

    public FireType fireType = FireType.FullAuto;

    public AmmoData ammoType;

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

    public float recoil_horizontal = 1.5f;
    public float recoil_vertical = 3.0f;

    private string AMMO_IN_MAG => $"{name}_AMMOinMAG";

    public void ReloadMag(ref int availableAmmo)
    {
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

    public override void SaveData()
    {
        base.SaveData();
        PlayerPrefs.SetInt(AMMO_IN_MAG, ammoInMagazine);
    }

    public override void LoadData()
    {
        base.LoadData();
        ammoInMagazine = PlayerPrefs.GetInt(AMMO_IN_MAG, magazineCapacity);
    }
}