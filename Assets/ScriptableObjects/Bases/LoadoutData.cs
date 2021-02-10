using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Loadout", fileName = "Loadout")]
public class LoadoutData : ScriptableObject
{
    public string LoadoutName = "Loadout 1";

    public WeaponData[] Weapons = new WeaponData[2];

    public EquipmentData[] Equipment = new EquipmentData[2];

    public TalentData[] Talents = new TalentData[3];

    public void CopyTo(LoadoutData target)
    {
        target.Weapons = new WeaponData[] { Weapons[0], Weapons[1] };
        target.Equipment = new EquipmentData[] { Equipment[0], Equipment[1] };
        target.Talents = new TalentData[] { Talents[0], Talents[1], Talents[2] };
    }
}