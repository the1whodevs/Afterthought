using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponIKPointers IKPointers => ikPointers;
    public WeaponData WeaponData => weaponData;
    
    [SerializeField] private WeaponIKPointers ikPointers;

    [SerializeField] private WeaponData weaponData;
}
