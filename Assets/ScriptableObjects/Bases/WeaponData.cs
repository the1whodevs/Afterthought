using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Veejay/Weapon", fileName = "Weapon")]
public class WeaponData : ScriptableObject
{
    public enum WeaponType {Firearm, Projectile, Melee}
    public enum FireType {FullAuto, Burst, SemiAuto, BoltAction}

    [FormerlySerializedAs("rightHandPrefab")]

    public new string name;
    public string description;
    public Sprite image;
    
    public GameObject wepPrefab;
    public GameObject projectilePrefab;
    public GameObject muzzleEffect;
    public GameObject bulletHole;

    public WeaponType weaponType = WeaponType.Firearm;
    public FireType fireType = FireType.FullAuto;
    public int magazineCapacity = 30;
    public float weaponDamage = 0.0f;
    public float fireRate = 3.0f;
    public float reloadSpeed = 1.0f;
    public float mobilityPenalty = 0.2f;
    public float adsMultiplier = 1.0f;

}