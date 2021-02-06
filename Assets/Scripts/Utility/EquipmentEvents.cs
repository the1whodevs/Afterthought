using UnityEngine;

public class EquipmentEvents : MonoBehaviour
{
    public void SpawnThrowPrefab()
    {
        Player.Active.Loadout.SpawnEquipmentThrowable();
    }

    public void ThrowSpawnedPrefab()
    {
        Player.Active.Loadout.ThrowEquipmentThrowable();
    }
}
