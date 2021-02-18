﻿using EmeraldAI;
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

    public void AddTargetToKill(EmeraldAISystem target)
    {
        targetsToKill.Add(target);
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
                var pos = targetPosition.position;
                if (Vector3.Distance(pos, targetPosition.position) > distanceTolerance) return;
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
public class Objective
{
    [Header("Go-To Settings")]
    public Transform targetArea;
    public float distanceTolerance = 1.0f;

    [Space]

    [Header("Interact-With Settings")]
    public InteractableObject targetInteractable;

    [Space]

    [Header("Kill Targets Settings")]
    public EmeraldAISystem[] targets;

    [Space]

    [Header("Global Settings")]
    public UnityEvent onObjectiveComplete;
    public ObjectiveData data;

    public void Initialize()
    {
        data.onObjectiveComplete += OnDataObjectiveComplete;
    }

    private void OnDataObjectiveComplete()
    {
        onObjectiveComplete?.Invoke();
    }
}
