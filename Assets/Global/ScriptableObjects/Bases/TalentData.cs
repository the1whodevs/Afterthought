using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Talent", fileName = "Talent")]
public class TalentData : ScriptableObject
{
    public new string name = "Bulletproof Vest";

    public string description = "Bullets have a 50% chance to reflect and deal no damage.";
}
