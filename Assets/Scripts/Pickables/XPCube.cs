using UnityEngine;

public class XPCube : PickableObj
{
    [SerializeField] private int xpToReward = 100;

    [SerializeField] private float despawnEffectLifetime = 2.0f;

    [SerializeField] private GameObject despawnEffect;

    public override void Pickup()
    {
        Player.Active.Experience.GetXP(xpToReward);

        if (despawnEffect) Destroy(Instantiate(despawnEffect, transform.position, Quaternion.identity, null), despawnEffectLifetime);

        Destroy(gameObject);
    }
}
