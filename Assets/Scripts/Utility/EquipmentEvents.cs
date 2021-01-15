using UnityEngine;

public class EquipmentEvents : MonoBehaviour
{
    public void SpawnThrowPrefab()
    {
        Player.Instance.Equipment.SpawnEquipmentThrowable();
    }

    public void ThrowSpawnedPrefab()
    {
        Player.Instance.Equipment.ThrowEquipmentThrowable();
    }
}
