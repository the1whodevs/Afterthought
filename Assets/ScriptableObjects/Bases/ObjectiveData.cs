using EmeraldAI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Veejay/Objectives/Objective", fileName = "NewObjectiveData")]
public class ObjectiveData : ScriptableObject
{
    public System.Action onObjectiveComplete;

    public enum ObjectiveType { KillTargets, GoToArea, Interact, EquipWeaponOnSlot, EquipEquipmentOnSlot, EquipTalentOnSlot }

    public ObjectiveType objectiveType;

    public int order;

    public string objectiveText = "Player should do something";

    [Range(0, 2)] public int equipObjectiveSlot = 0;

    private List<EmeraldAISystem> targetsToKill = new List<EmeraldAISystem>();

    private InteractableObject objToInteract;

    private Transform targetPosition;
    [SerializeField] private float distanceTolerance;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (targetsToKill.Count > 0) targetsToKill.Clear();
    }

#endif

    public void AddTargetToKill(EmeraldAISystem target)
    {
        if (targetsToKill.Contains(target)) return;

        targetsToKill.Add(target);

        for (int i = targetsToKill.Count - 1; i >= 0; i--)
        {
            if (!targetsToKill[i]) targetsToKill.RemoveAt(i);
        }
    }

    public void AddTargetPosition(Transform targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    public void AddTargetInteractable(InteractableObject targetInteractable)
    {
        objToInteract = targetInteractable;
    }

    public void CheckObjective()
    {
        switch (objectiveType)
        {
            case ObjectiveType.KillTargets:
                foreach (var target in targetsToKill)
                    if (target.CurrentHealth > 0) return;
                onObjectiveComplete?.Invoke();
                break;

            case ObjectiveType.GoToArea:
                if (!targetPosition)
                {
                    Debug.LogWarning("targetPosition is null!");
                    return;
                }
                var pos = targetPosition.position;
                var dist = Vector3.Distance(pos, Player.Active.transform.position);
                if (dist > distanceTolerance) return;
                onObjectiveComplete?.Invoke();
                break;
        }
    }

    public void CheckObjective(InteractableObject toCompare)
    {
        switch (objectiveType)
        {
            case ObjectiveType.Interact:
                if (toCompare.Equals(objToInteract)) onObjectiveComplete?.Invoke();
                break;
        }
    }

    public void CheckObjective(int slot)
    {
        switch (objectiveType)
        {
            case ObjectiveType.EquipWeaponOnSlot:
                if (slot == equipObjectiveSlot) onObjectiveComplete?.Invoke();
                    break;

            case ObjectiveType.EquipEquipmentOnSlot:
                if (slot == equipObjectiveSlot) onObjectiveComplete?.Invoke();
                break;

            case ObjectiveType.EquipTalentOnSlot:
                if (slot == equipObjectiveSlot) onObjectiveComplete?.Invoke();
                break;
        }
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
}

[System.Serializable]
public class OnObjectiveCompleteWrapper
{
    public ObjectiveData data;
    public UnityEvent onObjectiveComplete;

    public void Initialize()
    {
        data.onObjectiveComplete += OnDataObjectiveComplete;
    }

    private void OnDataObjectiveComplete()
    {
        onObjectiveComplete?.Invoke();
    }
}
