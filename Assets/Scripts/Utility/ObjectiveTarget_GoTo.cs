using UnityEngine;

public class ObjectiveTarget_GoTo : MonoBehaviour
{
    [SerializeField] private ObjectiveData objective;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (!objective) return;

        if (objective.objectiveType != ObjectiveData.ObjectiveType.GoToArea)
            Debug.LogError("Objective given is not of type 'GoToArea'!");
    }

    private void OnDrawGizmos()
    {
        if (objective) Gizmos.DrawSphere(transform.position, objective.distanceTolerance);
    }
#endif

    private void Awake()
    {
        objective.AddTargetPosition(transform);
    }
}
