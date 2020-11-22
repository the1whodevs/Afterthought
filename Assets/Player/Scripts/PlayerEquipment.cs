using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    /// <summary>
    /// The current weapon equipped.
    /// </summary>
    public WeaponData CurrentWeapon { get; private set; }
    
    /// <summary>
    /// The current weapon equipped gameobject.
    /// </summary>
    public GameObject CurrentWeaponObject { get; private set; }
    
    /// <summary>
    /// The current weapon equipped gameobject.
    /// </summary>
    public WeaponIKPointers CurrentWeaponIKPointers { get; private set; }

    /// <summary>
    /// The player's loadout.
    /// </summary>
    public LoadoutData Loadout => loadout;
    
    [SerializeField] private LoadoutData loadout;
    
    [SerializeField] private List<WeaponData> weapons = new List<WeaponData>();

    public void Init()
    {
        
    }
}
