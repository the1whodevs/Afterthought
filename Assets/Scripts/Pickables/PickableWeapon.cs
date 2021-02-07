using UnityEngine;

public class PickableWeapon : PickableObj
{
    [SerializeField] private WeaponData weaponToPickup;

    private void OnEnable()
    {
        name = weaponToPickup.name;
    }

    public override void Pickup()
    {
        Player.Active.Loadout.ReplaceCurrentWeapon(weaponToPickup);

        Debug.Log("TODO: Also spawn pickable of weapon that is replaced!");

        Destroy(gameObject);
    }
}
