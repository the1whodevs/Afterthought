using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Weapons/Talent", fileName = "Talent")]
public class TalentData : IDisplayableItem
{
    // Each talent must be added to the enum list AT THE END, and have the behaviour scripted wherever needed!
    public enum TalentType
    {
        IncreaseWeaponTypeDmg, // Implemented!
        IncreaseEquipmentDamage, // Implemented!
        IncreaseDamageWhileCrouching, // Implemented!
        IncreaseMaxHealth,  // Implemented!
        IncreaseMaxStamina, // wip: Stamina not implemented yet!
        NoMobilityPenalty, // Implemented!
        MinimalVerticalRecoil, // Implemented!
        MinimalHorizontalRecoil, // Implemented!
        FasterReloading, // wip: Reload speed not implemented yet!
        FasterCrouchSpeed, // Implemented!
        Empty,
    }

    public TalentType Talent;

    public WeaponData.WeaponType WeaponTypeAffected = WeaponData.WeaponType.Melee;

    // This is used as:
    // a) Multiplier for 'increased' type talents
    // b) Recoil for all weapons for 'recoil' type talents
    // c) Speed for 'faster' type talents
    public float value = 1.0f;
}