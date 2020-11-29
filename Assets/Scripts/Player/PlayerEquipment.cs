﻿using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    /// <summary>
    /// The current weapon equipped.
    /// </summary>
    public WeaponData CurrentWeapon { get; private set; }
    public GameObject CurrentWeaponObject { get; private set; }

    /// <summary>
    /// The player's loadout.
    /// </summary>
    public LoadoutData Loadout => loadout;

    public Animator CurrentAnimator { get; private set; }
    
    [SerializeField] private LoadoutData loadout;
    
    [SerializeField] private List<WeaponData> allWeaponData = new List<WeaponData>();
    [SerializeField] private List<EquipmentData> allEquipmentData = new List<EquipmentData>();
    [SerializeField] private List<TalentData> allTalentData = new List<TalentData>();

    [SerializeField] private Transform WeaponR;
    
    public void Init()
    {
        EquipPrimaryWeapon();
    }

    public void EquipPrimaryWeapon()
    {
        Equip(loadout.PrimaryWeapon);
    }

    public void EquipSecondaryWeapon()
    {
        Equip(loadout.SecondaryWeapon);
    }

    private void Equip(WeaponData weaponToEquip)
    {
        if (weaponToEquip == CurrentWeapon) return;

        if (CurrentWeaponObject)
        {
            StartCoroutine(SwitchWeapon(weaponToEquip));
        }
        else EquipWeapon(weaponToEquip);
    }

    private IEnumerator SwitchWeapon(WeaponData weaponToEquip)
    {
        const float delay = 1.0f;

        var hash = Player.instance.Animator.unequipAnimHash;
        
        CurrentAnimator.ResetTrigger(hash);
        CurrentAnimator.SetTrigger(hash);
        
        yield return new WaitForSeconds(delay);
        
        Unequip();
        EquipWeapon(weaponToEquip);
    }

    private void Unequip()
    {
        if (CurrentWeapon) CurrentWeapon = null;
        if (CurrentWeaponObject) Destroy(CurrentWeaponObject);
    }

    /// <summary>
    /// This should be called AFTER the specified hand has unequipped any weapons,
    /// and checking that the player CAN equip on that hand.
    /// </summary>
    /// <param name="handToEquipOn"></param>
    /// <param name="weaponToEquip"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void EquipWeapon(WeaponData weaponToEquip)
    {
        CurrentWeaponObject = Instantiate(weaponToEquip.wepPrefab, WeaponR.position, WeaponR.rotation, WeaponR);
        CurrentWeapon = weaponToEquip;
        CurrentAnimator = CurrentWeaponObject.GetComponent<Animator>();
    }
}