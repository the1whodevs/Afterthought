using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class Objective_Old
{
    public int order;

    public enum ObjectiveType { PickupWeapon, PickupEquipment, KillTargets, GoToArea, Interact }

    public ObjectiveType objectiveType { get; protected set; }

    public string objectiveText = "Player should do something";

    public UnityEvent onObjectiveComplete;

    public abstract void CheckObjective(Object toCompare);
}

public class ObjectiveComparer : IComparer<Objective_Old>
{
    public int Compare(Objective_Old x, Objective_Old y)
    {
        if (x.order < y.order) return -1;
        if (x.order > y.order) return 1;
        return 0;
    }
}

[System.Serializable]
public class ObjectiveInteractWithInteractable : Objective_Old
{
    public InteractableObject objectiveItem;

    public ObjectiveInteractWithInteractable()
    {
        objectiveType = ObjectiveType.Interact;
    }

    public override void CheckObjective(Object relatedItem)
    {
        if (objectiveItem.Equals((InteractableObject)relatedItem))
        {
            onObjectiveComplete?.Invoke();
            return;
        }
    }
}

[System.Serializable]
public class ObjectivePickupWeapon : Objective_Old
{
    public WeaponData objectiveItem;

    public ObjectivePickupWeapon()
    {
        objectiveType = ObjectiveType.PickupWeapon;
    }

    public override void CheckObjective(Object relatedItem)
    {
        if (objectiveItem.Equals((WeaponData)relatedItem))
        {
            onObjectiveComplete?.Invoke();
            return;
        }
    }
}

[System.Serializable]
public class ObjectivePickupEquipment : Objective_Old
{
    public EquipmentData objectiveItem;

    public ObjectivePickupEquipment()
    {
        objectiveType = ObjectiveType.PickupEquipment;
    }

    public override void CheckObjective(Object relatedItem)
    {
        if (objectiveItem.Equals((EquipmentData)relatedItem))
        {
            onObjectiveComplete?.Invoke();
            return;
        }
    }
}

[System.Serializable]
public class ObjectiveGoToArea : Objective_Old
{
    public Transform targetPosition;
    public float distTolerance;

    public ObjectiveGoToArea()
    {
        objectiveType = ObjectiveType.GoToArea;
    }

    public override void CheckObjective(Object relatedItem)
    {
        var pos = ((Transform)relatedItem).position;

        if (Vector3.Distance(pos, targetPosition.position) <= distTolerance)
        {
            onObjectiveComplete?.Invoke();
            return;
        }
    }
}

[System.Serializable]
public class ObjectiveKillTargets : Objective_Old
{
    public EmeraldAI.EmeraldAISystem[] targetsToKill;

    public ObjectiveKillTargets()
    {
        objectiveType = ObjectiveType.KillTargets;
    }

    public override void CheckObjective(Object relatedItem)
    {
        for (int i = 0; i < targetsToKill.Length; i++)
        {
            if (targetsToKill[i].CurrentHealth > 0) return;
        }

        onObjectiveComplete?.Invoke();
    }
}