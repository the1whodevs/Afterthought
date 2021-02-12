using UnityEngine;

public class PickableWeapon : InteractableObject
{
    [SerializeField] private WeaponData weaponToPickup;

    private void OnEnable()
    {
        name = weaponToPickup.name;
    }

    public override void Interact()
    {
        if (Player.Active.Loadout.CurrentWeapon.Equals(weaponToPickup)) return;

        var secondaryWeapon = Player.Active.Loadout.Loadout.Weapons[1];

        if (secondaryWeapon && weaponToPickup.Equals(secondaryWeapon)) return;

        // 'Drop' current weapon if it's possible (i.e. not possible for no weapon weapon)
        //if (Player.Active.Loadout.CurrentWeapon.pickablePrefab)
        //    Instantiate(Player.Active.Loadout.CurrentWeapon.pickablePrefab, transform.position, Quaternion.identity, null);

        // Equip this weapon.
        Player.Active.Loadout.PickupWeapon(weaponToPickup);

        // Destroy pickable.
        Destroy(gameObject);
    }
}
