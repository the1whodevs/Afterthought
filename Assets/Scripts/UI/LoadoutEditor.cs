﻿using System;
using UnityEngine;

public class LoadoutEditor : MonoSingleton<LoadoutEditor>
{
    /// <summary>
    /// Passes the index of the talent that was switched (0, 1, 2).
    /// </summary>
    public Action<int> OnTalentSwitched;

    /// <summary>
    /// Passes the index of the weapon that was switched (0, 1).
    /// </summary>
    public Action<int> OnWeaponSwitched;

    /// <summary>
    /// Passes the index of the equipment that was switched (0, 1).
    /// </summary>
    public Action<int> OnEquipmentSwitched;

    public LoadoutData[] AllLoadouts => playerLoadouts;
    [SerializeField] private LoadoutData[] playerLoadouts;

    public WeaponData[] AllWeapons => allWeapons;
    [SerializeField] private WeaponData[] allWeapons;

    public EquipmentData[] AllEquipment => allEquipment;
    [SerializeField] private EquipmentData[] allEquipment;

    public TalentData[] AllTalents => allTalents;
    [SerializeField] private TalentData[] allTalents;

    [SerializeField] private WeaponDisplay wepDisplayA;
    [SerializeField] private WeaponDisplay wepDisplayB;

    [SerializeField] private EquipmentDisplay equipDisplayA;
    [SerializeField] private EquipmentDisplay equipDisplayB;

    [SerializeField] private TalentDisplay talentDisplayA;
    [SerializeField] private TalentDisplay talentDisplayB;
    [SerializeField] private TalentDisplay talentDisplayC;

    private LoadoutData displayedLoadout;

    public void Init()
    {
        Player.Active.Objectives.LoadoutEditorInit();

        HideWindow();
    }

    public void LoadData(int[] loadoutsWepAindex, int[] loadoutsWepBindex, int[] loadoutsEqAindex, int[] loadoutsEqBindex, int[] loadoutsTalAindex, int[] loadoutsTalBindex, int[] loadoutsTalCindex, int[] weaponsLootStatus, int[] weaponsAmmoInMag, int[] equipmentAmmo)
    {

        for (var i = 0; i < playerLoadouts.Length; i++)
        {
            var loadout = playerLoadouts[i];

            if (loadoutsWepAindex[i] > -1) loadout.Weapons[0] = allWeapons[loadoutsWepAindex[i]];
            if (loadoutsWepBindex[i] > -1) loadout.Weapons[1] = allWeapons[loadoutsWepBindex[i]];

            if (loadoutsEqAindex[i] > -1) loadout.Equipment[0] = allEquipment[loadoutsEqAindex[i]];
            if (loadoutsEqBindex[i] > -1) loadout.Equipment[1] = allEquipment[loadoutsEqBindex[i]];

            if (loadoutsTalAindex[i] > -1) loadout.Talents[0] = allTalents[loadoutsTalAindex[i]];
            if (loadoutsTalBindex[i] > -1) loadout.Talents[1] = allTalents[loadoutsTalBindex[i]];
            if (loadoutsTalCindex[i] > -1) loadout.Talents[2] = allTalents[loadoutsTalCindex[i]];
        }

        for (var i = 0; i < allWeapons.Length; i++)
        {
            allWeapons[i].isLooted = weaponsLootStatus[i] == 1;
            allWeapons[i].ammoInMagazine = weaponsAmmoInMag[i];
        }

        for (var i = 0; i < allEquipment.Length; i++)
            allEquipment[i].currentAmmo = equipmentAmmo[i];
    }

    private void InitWeaponsList()
    {
        var weaponsFill = wepDisplayA.AllItemDisplay.transform.GetComponentsInChildren<WeaponDisplay>();

        allWeapons = Sorter.SortWeaponData(allWeapons);

        for (var i = 0; i < allWeapons.Length; i++)
        {
            weaponsFill[i].RelatedSlot = 0;
            weaponsFill[i].SetItemToDisplay(allWeapons[i]);
        }

        weaponsFill = wepDisplayB.AllItemDisplay.transform.GetComponentsInChildren<WeaponDisplay>();

        for (var i = 0; i < allWeapons.Length; i++)
        {
            weaponsFill[i].RelatedSlot = 1;
            weaponsFill[i].SetItemToDisplay(allWeapons[i]);
        }
    }

    private void InitEquipmentList()
    {
        var equipmentsFill = equipDisplayA.AllItemDisplay.transform.GetComponentsInChildren<EquipmentDisplay>();

        allEquipment = Sorter.SortEquipmentData(allEquipment);

        for (var i = 0; i < allEquipment.Length; i++)
        {
            equipmentsFill[i].RelatedSlot = 0;
            equipmentsFill[i].SetItemToDisplay(allEquipment[i]);
        }

        equipmentsFill = equipDisplayB.AllItemDisplay.transform.GetComponentsInChildren<EquipmentDisplay>();

        for (var i = 0; i < allEquipment.Length; i++)
        {
            equipmentsFill[i].RelatedSlot = 1;
            equipmentsFill[i].SetItemToDisplay(allEquipment[i]);
        }

    }

    private void InitTalentList()
    {
        var talentsFill = talentDisplayA.AllItemDisplay.transform.GetComponentsInChildren<TalentDisplay>();

        allTalents = Sorter.SortTalentData(allTalents);

        for (var i = 0; i < allTalents.Length; i++)
        {
            talentsFill[i].RelatedSlot = 0;
            talentsFill[i].SetItemToDisplay(allTalents[i]);
        }

        talentsFill = talentDisplayB.AllItemDisplay.transform.GetComponentsInChildren<TalentDisplay>();

        for (var i = 0; i < allTalents.Length; i++)
        {
            talentsFill[i].RelatedSlot = 1;
            talentsFill[i].SetItemToDisplay(allTalents[i]);
        }

        talentsFill = talentDisplayC.AllItemDisplay.transform.GetComponentsInChildren<TalentDisplay>();

        for (var i = 0; i < allTalents.Length; i++)
        {
            talentsFill[i].RelatedSlot = 2;
            talentsFill[i].SetItemToDisplay(allTalents[i]);
        }
    }

    public void ShowWindow()
    {
        if (!displayedLoadout) displayedLoadout = playerLoadouts[0];


        InitWeaponsList();
        InitEquipmentList();
        InitTalentList();

        UpdateDisplays();

        gameObject.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseAllItemDisplays()
    {
        wepDisplayA.HideAllItemDisplay();
        wepDisplayB.HideAllItemDisplay();

        equipDisplayA.HideAllItemDisplay();
        equipDisplayB.HideAllItemDisplay();

        talentDisplayA.HideAllItemDisplay();
        talentDisplayB.HideAllItemDisplay();
        talentDisplayC.HideAllItemDisplay();
    }

    public void SelectLoadout()
    {
        if (Player.Active.Loadout.Loadout.Equals(displayedLoadout)) return;

        Player.Active.Loadout.ChangeActiveLoadout(displayedLoadout);

        UpdateDisplays();
    }

    public void SetLoadoutWeapon(WeaponData weapon, int slot)
    {
        foreach (var wep in displayedLoadout.Weapons)
            if (wep == weapon) return;

        displayedLoadout.Weapons[slot] = weapon;

        OnWeaponSwitched?.Invoke(slot);

        UpdateDisplays();
    }

    public void SetLoadoutEquipment(EquipmentData equipment, int slot)
    {
        foreach (var eq in displayedLoadout.Equipment)
            if (equipment == eq) return;

        displayedLoadout.Equipment[slot] = equipment;

        OnEquipmentSwitched?.Invoke(slot);

        UpdateDisplays();
    }

    public void SetLoadoutTalent(TalentData talent, int slot)
    {
        foreach (var tal in displayedLoadout.Talents)
            if (tal == talent) return;

        displayedLoadout.Talents[slot] = talent;

        OnTalentSwitched?.Invoke(slot);

        UpdateDisplays();
    }

    public void HideWindow()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1.0f;

        if (Player.Active) Player.Active.Controller.ExitUI();

        gameObject.SetActive(false);
    }

    public void UpdateDisplays()
    {
        if (!displayedLoadout) throw new Exception("Update Displays called but displayedLoadout is null!");

        wepDisplayA.SetItemToDisplay(displayedLoadout.Weapons[0]);
        wepDisplayB.SetItemToDisplay(displayedLoadout.Weapons[1]);

        equipDisplayA.SetItemToDisplay(displayedLoadout.Equipment[0]);
        equipDisplayB.SetItemToDisplay(displayedLoadout.Equipment[1]);

        talentDisplayA.SetItemToDisplay(displayedLoadout.Talents[0]);
        talentDisplayB.SetItemToDisplay(displayedLoadout.Talents[1]);
        talentDisplayC.SetItemToDisplay(displayedLoadout.Talents[2]);

        wepDisplayA.UpdateDisplay();
        wepDisplayB.UpdateDisplay();

        equipDisplayA.UpdateDisplay();
        equipDisplayB.UpdateDisplay();

        talentDisplayA.UpdateDisplay();
        talentDisplayB.UpdateDisplay();
        talentDisplayC.UpdateDisplay();
    }

    public void ChangeLoadout(int loadoutIndex)
    {
        displayedLoadout = playerLoadouts[loadoutIndex];

        UpdateDisplays();
    }
}