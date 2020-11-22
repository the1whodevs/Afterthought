using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Weapon", fileName = "Weapon")]
public class WeaponData : ScriptableObject
{
    public enum WeaponHandType { OneHanded, TwoHanded }
    public enum WeaponTechType { Gun, Sword, Magic }
    
    public GameObject prefab;

    public WeaponHandType HandType = WeaponHandType.TwoHanded;
    public WeaponTechType TechType = WeaponTechType.Gun;
}