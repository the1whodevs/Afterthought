using UnityEngine;

public class ObjectiveTarget_Interact : MonoBehaviour
{
    [SerializeField] private ObjectiveData objective;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (!objective) return;

        if (objective.objectiveType != ObjectiveData.ObjectiveType.Interact)
            Debug.LogError("Objective given is not of type 'Interact'!");
    }

#endif

    private void Start()
    {
        objective.AddTargetInteractable(GetComponent<InteractableObject>());
    }
}
