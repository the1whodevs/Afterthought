using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobLoot : LootCube
{
    private EmeraldAI.EmeraldAISystem attachedSystem;
    [SerializeField] private Collider lootCollider;

    protected override void Start()
    {
        base.Start();
        lootCollider.enabled = false;
        attachedSystem = GetComponent<EmeraldAI.EmeraldAISystem>();
        attachedSystem.DeathEvent.AddListener(() => { lootCollider.enabled = true; });
    }

    public override void Interact()
    {
        if (attachedSystem.CurrentHealth <= 0)
        {
            base.Interact();
        }
    }

    public override void Loot()
    {
        IsLooted = true;
    }
}
