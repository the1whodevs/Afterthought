using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Weapons/Ammo", fileName = "AmmoData")]
public class AmmoData : IDisplayableItem
{
    public int currentAmmo = 300;
    public int startingAmmo = 300;
    public int maxAmmo = 300;

    public void ResetAmmo()
    {
        currentAmmo = startingAmmo;
    }
}