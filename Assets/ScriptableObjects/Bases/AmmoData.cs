using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Weapons/Ammo", fileName = "AmmoData")]
public class AmmoData : IDisplayableItem
{
    public int currentAmmo = 300;
    public int startingAmmo = 300;
    public int maxAmmo = 300;

    private string AVAILABLE_AMMO => $"{name}_AVAILABLE_AMMO";

    public override void SaveData()
    {
        PlayerPrefs.SetInt(AVAILABLE_AMMO, currentAmmo);
    }

    public override void LoadData()
    {
        currentAmmo = PlayerPrefs.GetInt(AVAILABLE_AMMO, startingAmmo);
        if (currentAmmo < 0 && maxAmmo >= 0) currentAmmo = 0;
    }
}