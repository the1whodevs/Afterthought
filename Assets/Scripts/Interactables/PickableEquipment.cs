using UnityEngine;

public class PickableEquipment : InteractableObject
{
    [SerializeField] private EquipmentData relatedEquipment;

    public override string GetActionVerb()
    {
        return "pickup";
    }

    public override void Interact()
    {
        // Only used as ammo pickup. Player can change equipment
        // only through the loadout editor!
        if (Player.Active.Loadout.GetEquipmentAmmo(relatedEquipment)) 
        {
            Loot();
        }
    }
}
