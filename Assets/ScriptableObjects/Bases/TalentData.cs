using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Talent", fileName = "Talent")]
public class TalentData : ScriptableObject
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
    }

    public new string name = "Bulletproof Vest";

    public string description = "Bullets have a 50% chance to reflect and deal no damage.";

    public TalentType Talent;

    public WeaponData.WeaponType WeaponTypeAffected = WeaponData.WeaponType.Melee;

    // This is used as:
    // a) Multiplier for 'increased' type talents
    // b) Recoil for all weapons for 'recoil' type talents
    // c) Speed for 'faster' type talents
    public float value = 1.0f;
}