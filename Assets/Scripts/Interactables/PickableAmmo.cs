using UnityEngine;

public class PickableAmmo : InteractableObject
{
    [SerializeField] private int ammoToGive = 10;

    [SerializeField] private AmmoData ammoTypeToGive;

    [SerializeField] private GameObject despawnEffect;

    [SerializeField] private float despawnEffectLifetime = 2.0f;

    private int ammoLeftInBox;

    private void OnEnable()
    {
        ammoLeftInBox = ammoToGive;
    }

    public override string GetActionVerb()
    {
        return "get";
    }

    public override void Interact()
    {
        Player.Active.Loadout.GetAmmo(ammoTypeToGive, ref ammoLeftInBox);

        if (ammoLeftInBox == 0)
        {
            if (despawnEffect) Destroy(Instantiate(despawnEffect, transform.position, Quaternion.identity, null), despawnEffectLifetime);

            Loot();
        }
    }
}
