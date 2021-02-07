using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Ammo", fileName = "AmmoData")]
public class AmmoData : IDisplayableItem
{
    public int currentAmmo = 300;
    public int maxAmmo = 300;

    private string AVAILABLE_AMMO => $"{name}_AVAILABLE_AMMO";

    public override void SaveData()
    {
        PlayerPrefs.SetInt(AVAILABLE_AMMO, currentAmmo);
    }

    public override void LoadData()
    {
        currentAmmo = PlayerPrefs.GetInt(AVAILABLE_AMMO, currentAmmo);
        if (currentAmmo < 0 && maxAmmo >= 0) currentAmmo = 0;
    }
}