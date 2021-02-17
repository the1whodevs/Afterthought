using EmeraldAI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Veejay/Objective", fileName = "NewObjectiveData")]
public class ObjectiveData : ScriptableObject
{
    public System.Action onObjectiveComplete;

    public enum ObjectiveType { KillTargets, GoToArea, Interact }

    public ObjectiveType objectiveType;

    public int order;

    public string objectiveText = "Player should do something";

    private List<EmeraldAISystem> targetsToKill;

    private InteractableObject objToInteract;

    private Transform targetPosition;
    private float distanceTolerance;

    public void Initialize(List<EmeraldAISystem> targets)
    {
        targetsToKill = targets;
    }

    public void Initialize(InteractableObject interactTarget)
    {
        objToInteract = interactTarget;
    }

    public void Initialize(Transform positionTarget, float distanceTolerance)
    {
        targetPosition = positionTarget;
        this.distanceTolerance = distanceTolerance;
    }

    public void CheckObjective(Object toCompare = null)
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

            case ObjectiveType.Interact:
                if (((InteractableObject)toCompare).Equals(objToInteract)) onObjectiveComplete?.Invoke();
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
