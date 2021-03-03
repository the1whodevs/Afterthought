using Five.MoreMaths;
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    public int Level { get; private set; } = 1;

    public int XPRequired => xpRequired.GetIntValue(Level);
    public int CurrentXP = 0;

    [SerializeField] private IncrementalEquation xpRequired;

    public void LoadData(int level, int xp)
    {
        Level = level;
        CurrentXP = xp;
    }

    public void GetXP(int value)
    {
        var requiredXP = XPRequired;

        CurrentXP += value;

        Debug.LogFormat("XP Received! New XP: {0} // Required: {1}", CurrentXP, XPRequired);

        if (CurrentXP >= requiredXP)
        {
            CurrentXP -= requiredXP;
            Level++;

            Debug.LogFormat("Level Up! New Level: {0} // CurrentXP: {1} // XPRequired: {1}", Level, CurrentXP, XPRequired);
        }
    }
}