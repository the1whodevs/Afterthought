using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Loadout", fileName = "Loadout")]
public class LoadoutData : ScriptableObject
{
    public string LoadoutName = "Loadout 1";

    public WeaponData[] Weapons = new WeaponData[2];

    public EquipmentData[] Equipment = new EquipmentData[2];

    public TalentData[] Talents = new TalentData[3];
}