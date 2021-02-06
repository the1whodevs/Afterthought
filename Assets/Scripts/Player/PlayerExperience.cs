using Five.MoreMaths;
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    public int Level { get; private set; }

    public float XPRequired => xpRequired.GetFloatValue(Level);
    public float CurrentXP = 0.0f;

    [SerializeField] private IncrementalEquation xpRequired;

    private const string PLAYER_LEVEL_KEY = "PLAYER_LEVEL";
    private const string PLAYER_XP_KEY = "PLAYER_CURRENT_XP";

    private void Start()
    {
        Level = PlayerPrefs.GetInt(PLAYER_LEVEL_KEY, 1);
        CurrentXP = PlayerPrefs.GetFloat(PLAYER_XP_KEY, 0.0f);
    }

    public void GetXP(float value)
    {
        var requiredXP = XPRequired;

        CurrentXP += value;

        if (CurrentXP >= requiredXP)
        {
            CurrentXP -= requiredXP;
            Level++;

            //PlayerPrefs.SetInt(PLAYER_LEVEL_KEY, Level);
            //PlayerPrefs.SetFloat(PLAYER_XP_KEY, CurrentXP);
        }
    }
}