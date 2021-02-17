using UnityEngine;

public class PickableWeapon : InteractableObject
{
    [SerializeField] private WeaponData weaponToPickup;

    private void OnEnable()
    {
        name = weaponToPickup.name;
    }

    public override string GetActionVerb()
    {
        return "equip";
    }

    public override void Interact()
    {
        if (Player.Active.Loadout.CurrentWeapon.Equals(weaponToPickup)) return;

        var secondaryWeapon = Player.Active.Loadout.Loadout.Weapons[1];

        if (secondaryWeapon && weaponToPickup.Equals(secondaryWeapon)) return;

        // Equip this weapon.
        Player.Active.Loadout.PickupWeapon(weaponToPickup);

        // Destroy pickable.
        Destroy(gameObject);
    }
}
