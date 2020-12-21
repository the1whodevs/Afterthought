using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Veejay/Weapon", fileName = "Weapon")]
public class WeaponData : ScriptableObject
{
    public enum WeaponType {Firearm, Projectile, Melee}
    public enum FireType { FullAuto, Burst, SemiAuto, BoltAction }

    public GameObject RandomHitDecal => hitDecal[Random.Range(0, hitDecal.Length)];
    public AudioClip RandomShotAudioFX => shotAudioFx[Random.Range(0, shotAudioFx.Length)];
    
    [FormerlySerializedAs("rightHandPrefab")] public new string name;
    public string description;
    public Sprite image;
    
    public GameObject wepPrefab;
    public GameObject projectilePrefab;
    public GameObject muzzleEffect;
    
    public AudioClip reloadAudioFx;
    public AudioClip emptyClipAudioFx;

    public bool hasScope = false;
    public MouseCamera.ZoomLevels scopeZoom = MouseCamera.ZoomLevels.X4;
    
    [FormerlySerializedAs("audioFx")] public AudioClip[] shotAudioFx;
    [FormerlySerializedAs("bulletHole")] public GameObject hitImpact;
    public GameObject[] hitDecal; 
    
    public WeaponType weaponType = WeaponType.Firearm;
    
    public FireType fireType = FireType.FullAuto;

    public int currentAmmo = 30;
    public int magazineCapacity = 30;

    public float projectileSpeed = 100.0f;
    public float weaponDamage = 0.0f;
    public float fireRate = 3.0f;
    public float shootAudioPitch = 2.0f;
    public float reloadSpeed = 1.0f;
    public float mobilityPenalty = 0.2f;
    public float adsMultiplier = 1.0f;

    private string AMMO_IN_MAG => $"{name}_AMMOinMAG";

    public void ReloadMag(ref int availableAmmo)
    {
        var bulletsForMax = magazineCapacity - currentAmmo;
        
        if (bulletsForMax == 0) Debug.LogError("Reloading while the mag is full!");
        
        if (availableAmmo >= bulletsForMax)
        {
            availableAmmo -= bulletsForMax;
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