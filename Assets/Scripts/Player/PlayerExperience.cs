using Five.MoreMaths;
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    public int Level { get; private set; }

    public int XPRequired => xpRequired.GetIntValue(Level);
    public int CurrentXP = 0;

    [SerializeField] private IncrementalEquation xpRequired;

    private const string PLAYER_LEVEL_KEY = "PLAYER_LEVEL";
    private const string PLAYER_XP_KEY = "PLAYER_CURRENT_XP";

    private void Start()
    {
        Level = PlayerPrefs.GetInt(PLAYER_LEVEL_KEY, 1);
        CurrentXP = PlayerPrefs.GetInt(PLAYER_XP_KEY, 0);
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

            PlayerPrefs.SetInt(PLAYER_LEVEL_KEY, Level);
            PlayerPrefs.SetInt(PLAYER_XP_KEY, CurrentXP);
        }
    }
}