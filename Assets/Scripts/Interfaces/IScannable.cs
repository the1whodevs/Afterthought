using UnityEngine;

public interface IScannable
{
    float GetCurrentHP();
    float GetMaxHP();
    float GetHPPercentage();

    bool CheckIsFriendlyToPlayer();

    Color GetCrosshairColor();
}
