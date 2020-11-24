using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public enum WeaponHand { Left, Right, Both }

    public WeaponHand whichHandsAreUsed = WeaponHand.Right;
    
    /// <summary>
    /// The current weapon equipped.
    /// </summary>
    public WeaponData CurrentWeaponR { get; private set; }
    public GameObject CurrentWeaponObjectR { get; private set; }

    /// <summary>
    /// The current weapon equipped.
    /// </summary>
    public WeaponData CurrentWeaponL { get; private set; }
    public GameObject CurrentWeaponObjectL { get; private set; }
    
    /// <summary>
    /// The player's loadout.
    /// </summary>
    public LoadoutData Loadout => loadout;
    
    [SerializeField] private LoadoutData loadout;
    
    [SerializeField] private List<WeaponData> allWeaponData = new List<WeaponData>();
    [SerializeField] private List<EquipmentData> allEquipmentData = new List<EquipmentData>();
    [SerializeField] private List<TalentData> allTalentData = new List<TalentData>();

    [SerializeField] private Transform WeaponL;
    [SerializeField] private Transform WeaponR;
    
    public void Init()
    {
        EquipWeapon(WeaponHand.Both, loadout.PrimaryWeapon);
    }

    private void EquipWeapon(WeaponHand handToEquipTo, WeaponData weaponToEquip)
    {
        // If the player is trying to equip a weapon on his left hand...
        var tryingToDualWield = handToEquipTo == WeaponHand.Left;
        
        // and he has a one handed weapon on his right hand...
        // and he's trying to equip a one-handed weapon.
        var canDualWield = CurrentWeaponR && CurrentWeaponR.HandType == WeaponData.WeaponHandType.OneHanded &&
        weaponToEquip.HandType == WeaponData.WeaponHandType.OneHanded;

        // the player will dual wield.
        if (tryingToDualWield && canDualWield)
        {
            UnequipFromHand(WeaponHand.Left);
            EquipOnHand(WeaponHand.Left, weaponToEquip);
            return;
        }

        // OR...
        
        // If the player has a weapon on his left hand.
        tryingToDualWield = (bool) CurrentWeaponL;
        
        // and is trying to equip a one handed weapon
        // and is trying to equip on his right hand
        canDualWield = weaponToEquip.HandType == WeaponData.WeaponHandType.OneHanded &&
                       handToEquipTo == WeaponHand.Right;
        
        
        // the player will dual wield.
        if (tryingToDualWield && canDualWield)
        {
            UnequipFromHand(WeaponHand.Right);
            EquipOnHand(WeaponHand.Right, weaponToEquip);
            return;
        }
        
        // ELSE...
        
        Debug.LogFormat("Player is trying to equip {0} which is a {1} weapon, on {2} hand(s). Is this done properly?",
            weaponToEquip.name, weaponToEquip.HandType.ToString(), handToEquipTo.ToString());
        
        // The player will equip the weapon on his right hand.

        UnequipFromHand(WeaponHand.Both);
        EquipOnHand(WeaponHand.Both, weaponToEquip);
    }

    private void UnequipFromHand(WeaponHand handToUnequipFrom)
    {
        switch (handToUnequipFrom)
        {
            case WeaponHand.Both:
                UnequipFromHand(WeaponHand.Left);
                UnequipFromHand(WeaponHand.Right);
                return;
            case WeaponHand.Right:
            {
                if (CurrentWeaponR) CurrentWeaponR = null;
                if (CurrentWeaponObjectR) Destroy(CurrentWeaponObjectR);
                break;
            }
            case WeaponHand.Left:
            {
                if (CurrentWeaponL) CurrentWeaponL = null;
                if (CurrentWeaponObjectL) Destroy(CurrentWeaponObjectL);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(handToUnequipFrom), handToUnequipFrom, null);
        }
    }

    /// <summary>
    /// This should be called AFTER the specified hand has unequipped any weapons,
    /// and checking that the player CAN equip on that hand.
    /// </summary>
    /// <param name="handToEquipOn"></param>
    /// <param name="weaponToEquip"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void EquipOnHand(WeaponHand handToEquipOn, WeaponData weaponToEquip)
    {
        switch (handToEquipOn)
        {
            case WeaponHand.Left:
                CurrentWeaponObjectL = Instantiate(weaponToEquip.prefab, WeaponL.position, WeaponL.rotation, WeaponL);
                CurrentWeaponObjectL.GetComponent<WeaponHandsHandler>().AdjustHands(handToEquipOn);
                CurrentWeaponL = weaponToEquip;
                break;
            case WeaponHand.Right:
                CurrentWeaponObjectR = Instantiate(weaponToEquip.prefab, WeaponR.position, WeaponR.rotation, WeaponR);
                CurrentWeaponObjectR.GetComponent<WeaponHandsHandler>().AdjustHands(handToEquipOn);
                CurrentWeaponR = weaponToEquip;
                break;
            case WeaponHand.Both:
                // This means it's a 2 handed weapon, on the right hand.
                CurrentWeaponObjectR = Instantiate(weaponToEquip.prefab, WeaponR.position, WeaponR.rotation, WeaponR);
                CurrentWeaponObjectR.GetComponent<WeaponHandsHandler>().AdjustHands(handToEquipOn);
                CurrentWeaponR = weaponToEquip;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(handToEquipOn), handToEquipOn, null);
        }
    }
}