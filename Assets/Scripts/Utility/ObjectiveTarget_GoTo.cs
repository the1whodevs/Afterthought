using UnityEngine;

public class ObjectiveTarget_GoTo : MonoBehaviour
{
    [SerializeField] private ObjectiveData objective;

    public bool drawGizmo = true;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (!objective) return;

        if (objective.objectiveType != ObjectiveData.ObjectiveType.GoToArea)
            Debug.LogError("Objective given is not of type 'GoToArea'!");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.2f);
        if (objective && drawGizmo) Gizmos.DrawSphere(transform.position, objective.distanceTolerance);
    }
#endif

    private void Awake()
    {
        objective.AddTargetPosition(transform);
    }
}
