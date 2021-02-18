using UnityEngine;

public class ObjectiveTarget_Kill : MonoBehaviour
{
    [SerializeField] private ObjectiveData objective;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (!objective) return;

        if (objective.objectiveType != ObjectiveData.ObjectiveType.KillTargets)
            Debug.LogError("Objective given is not of type 'KillTargets'!");
    }

#endif

    private void Start()
    {
        objective.AddTargetToKill(GetComponent<EmeraldAI.EmeraldAISystem>());
    }
}
