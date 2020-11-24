using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Weapon", fileName = "Weapon")]
public class WeaponData : ScriptableObject
{
    public enum WeaponHandType { OneHanded, TwoHanded }
    public enum WeaponTechType { Gun, Sword, Magic }
    
    public WeaponHandType HandType = WeaponHandType.TwoHanded;
    public WeaponTechType TechType = WeaponTechType.Gun;

    [Header("This is the main prefab to be used, and should never be null.")]
    public GameObject rightHandPrefab;
    
    [Header("This is the prefab to be used when equipping the weapon on the left hand ONLY," +
            " and SHOULD be null if the weapon is 2-handed.")]
    public GameObject leftHandPrefab;
}