using UnityEngine;

public interface ILootable
{
    GameObject gameObject { get; }

    bool IsLooted { get; }

    void Loot();
    void SetLootStatus(bool status);
}
