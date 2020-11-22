using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Loadout", fileName = "Loadout")]
public class LoadoutData : ScriptableObject
{
    public string LoadoutName = "Loadout 1";
    
    public WeaponData PrimaryWeapon;
    public WeaponData SecondaryWeapon;

    public EquipmentData Equipment1;
    public EquipmentData Equipment2;

    public TalentData TalentA;
    public TalentData TalentB;
    public TalentData TalentC;
}