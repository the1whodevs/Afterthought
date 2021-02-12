using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class ObjectiveData
{
    public int order;

    public enum ObjectiveType { PickupWeapon, PickupEquipment, KillTargets, GoToArea, None }

    public ObjectiveType objectiveType { get; protected set; }

    public string objectiveText = "Player should do something";

    public UnityEvent onObjectiveComplete;

    public abstract void CheckObjective(Object toCompare);
}

public class ObjectiveDataComparer : IComparer<ObjectiveData>
{
    public int Compare(ObjectiveData x, ObjectiveData y)
    {
        if (x.order < y.order) return -1;
        if (x.order > y.order) return 1;
        return 0;
    }
}

[System.Serializable]
public class ObjectiveDataPickupWeapon : ObjectiveData
{
    public WeaponData objectiveItem;

    public ObjectiveDataPickupWeapon()
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
public class ObjectiveDataPickupEquipment : ObjectiveData
{
    public EquipmentData objectiveItem;

    public ObjectiveDataPickupEquipment()
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
public class ObjectiveDataGoToArea : ObjectiveData
{
    public Transform targetPosition;
    public float distTolerance;

    public ObjectiveDataGoToArea()
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
public class ObjectiveDataKillTargets : ObjectiveData
{
    public EmeraldAI.EmeraldAISystem[] targetsToKill;

    public ObjectiveDataKillTargets()
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