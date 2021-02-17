using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Objective))]
public class ObjectivePropertyDrawer : PropertyDrawer
{
    private const int GLOBALSETTINGS_CONTROL_HEIGHT = 155;
    private const int GOTO_CONTROL_HEIGHT = 100;
    private const int INTERACTWITH_CONTROL_HEIGHT = 50;
    private const int KILLTARGETS_CONTROL_HEIGHT = 100;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var data = (ObjectiveData)property.FindPropertyRelative("data").objectReferenceValue;

        switch (data.objectiveType)
        {
            case ObjectiveData.ObjectiveType.KillTargets:
                var targetsSize = property.FindPropertyRelative("targetInteractable").arraySize;
                return (targetsSize * KILLTARGETS_CONTROL_HEIGHT) + GLOBALSETTINGS_CONTROL_HEIGHT;

            case ObjectiveData.ObjectiveType.GoToArea:
                return GLOBALSETTINGS_CONTROL_HEIGHT + GOTO_CONTROL_HEIGHT;

            case ObjectiveData.ObjectiveType.Interact:
                return GLOBALSETTINGS_CONTROL_HEIGHT + INTERACTWITH_CONTROL_HEIGHT;
        }

        return 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var data = (ObjectiveData)property.FindPropertyRelative("data").objectReferenceValue;

        //[Header("Global Settings")]
        //public UnityEvent onObjectiveComplete;
        //public ObjectiveData data;

        switch (data.objectiveType)
        {
            case ObjectiveData.ObjectiveType.KillTargets:
                var targetsSize = property.FindPropertyRelative("targetInteractable").arraySize;
                break;

            case ObjectiveData.ObjectiveType.GoToArea:
                break;

            case ObjectiveData.ObjectiveType.Interact:
                break;
        }

        //[Header("Go-To Settings")]
        //public Transform targetArea;
        //public float distanceTolerance = 1.0f;

        //[Header("Interact-With Settings")]
        //public InteractableObject targetInteractable;

        //[Header("Kill Targets Settings")]
        //public EmeraldAISystem[] targets;
    }
}
