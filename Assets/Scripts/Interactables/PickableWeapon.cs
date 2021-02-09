﻿using UnityEngine;

public class PickableWeapon : InteractableObject
{
    [SerializeField] private WeaponData weaponToPickup;

    private void OnEnable()
    {
        name = weaponToPickup.name;
    }

    public override void Interact()
    {
        // 'Drop' current weapon if it's possible (i.e. not possible for no weapon weapon)
        if (Player.Active.Loadout.CurrentWeapon.pickablePrefab)
            Instantiate(Player.Active.Loadout.CurrentWeapon.pickablePrefab, transform.position, Quaternion.identity, null);

        // Equip this weapon.
        Player.Active.Loadout.ReplaceCurrentWeapon(weaponToPickup);

        // Destroy pickable.
        Destroy(gameObject);
    }
}
