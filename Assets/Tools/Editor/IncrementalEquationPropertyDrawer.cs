using Five.MoreMaths;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IncrementalEquation))]
public class IncrementalEquationPropertyDrawer : PropertyDrawer
{
    private const int ANIM_CURVE_CONTROL_HEIGHT = 155;
    private const int NOT_EXPANDED_HEIGHT = 20;
    private const int POST_CURVE_CONTROL_HEIGHT = 60;
    private const int DEBUG_LIST_HEIGHT = 100;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = 180;

        if (!property.isExpanded) return NOT_EXPANDED_HEIGHT;

        var clampWithinCurve = property.FindPropertyRelative("clampWithinCurve").boolValue;
        var overrideMasterCurve = property.FindPropertyRelative("overrideMasterCurve").boolValue;
        var showDebugValues = property.FindPropertyRelative("showDebugValues").boolValue;

        if (!clampWithinCurve) height += POST_CURVE_CONTROL_HEIGHT;
        if (overrideMasterCurve) height += ANIM_CURVE_CONTROL_HEIGHT;
        if (showDebugValues) height += DEBUG_LIST_HEIGHT;

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var originalPosition = position;
        originalPosition.xMin = 20;
        position.xMin = 20;

        //EditorGUI.BeginProperty(position, new GUIContent(""), property);

        position.height = 17;
        position.width = (EditorGUIUtility.currentViewWidth) - 30;
        // EditorGUI.DrawRect(position, Color.red);

        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded, label);

        if (property.isExpanded)
        {
            var levelRange = property.FindPropertyRelative("levelRange");
            var valueRange = property.FindPropertyRelative("valueRange");
            var clampWithinCurve = property.FindPropertyRelative("clampWithinCurve");
            var overrideMasterCurve = property.FindPropertyRelative("overrideMasterCurve");
            var levelRangeValuesCurve = property.FindPropertyRelative("levelRangeValuesCurve");
            var equationType = property.FindPropertyRelative("equationType");
            var debugLevelRange = property.FindPropertyRelative("debugLevelRange");
            var showDebugValues = property.FindPropertyRelative("showDebugValues");
            var xValue = property.FindPropertyRelative("xValue");

            // Draw label
            // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            position.x = originalPosition.x;

            // Calculate rects
            var levelRangeMinLabel = new Rect((EditorGUIUtility.currentViewWidth + 30) - EditorGUIUtility.currentViewWidth, position.y + 25, 30, 15);

            var levelRangeMaxLabel = new Rect(EditorGUIUtility.currentViewWidth - (EditorGUIUtility.currentViewWidth * 0.475f), position.y + 25, 30, 15);

            var levelRangeRectX = new Rect((EditorGUIUtility.currentViewWidth + 30) - EditorGUIUtility.currentViewWidth + 60, position.y + 27, (EditorGUIUtility.currentViewWidth / 2) - 100, 20);

            var levelRangeRectY = new Rect(EditorGUIUtility.currentViewWidth - (EditorGUIUtility.currentViewWidth * 0.475f) + 65, position.y + 27, (EditorGUIUtility.currentViewWidth / 2) - 100, 20);

            // Calculate rects
            var valueRangeMinLabel = new Rect((EditorGUIUtility.currentViewWidth + 30) - EditorGUIUtility.currentViewWidth, position.y + 60, 30, 15);

            var valueRangeMaxLabel = new Rect(EditorGUIUtility.currentViewWidth - (EditorGUIUtility.currentViewWidth * 0.475f), position.y + 60, 30, 15);

            var valueRangeRectX = new Rect((EditorGUIUtility.currentViewWidth + 30) - EditorGUIUtility.currentViewWidth + 60, position.y + 60, (EditorGUIUtility.currentViewWidth / 2) - 100, 20);

            var valueRangeRectY = new Rect(EditorGUIUtility.currentViewWidth - (EditorGUIUtility.currentViewWidth * 0.475f) + 65, position.y + 60, (EditorGUIUtility.currentViewWidth / 2) - 100, 20);

            EditorGUI.PrefixLabel(levelRangeMinLabel, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Level Min"));

            position = EditorGUI.PrefixLabel(levelRangeMaxLabel, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Level Max"));

            levelRange.vector2IntValue =
                new Vector2Int(EditorGUI.DelayedIntField(levelRangeRectX, levelRange.vector2IntValue.x),
                EditorGUI.DelayedIntField(levelRangeRectY, levelRange.vector2IntValue.y));

            EditorGUI.PrefixLabel(valueRangeMinLabel, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Value Min"));

            EditorGUI.PrefixLabel(valueRangeMaxLabel, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Value Max"));

            valueRange.vector2Value =
                new Vector2(EditorGUI.DelayedFloatField(valueRangeRectX, valueRange.vector2Value.x),
                EditorGUI.DelayedFloatField(valueRangeRectY, valueRange.vector2Value.y));

            position.x = originalPosition.x + 10;
            position.y = originalPosition.y;

            position.width = EditorGUIUtility.currentViewWidth - 30;
            position.height = 20;
            position.y += 85;

            // EditorGUI.DrawRect(position, Color.white);

            clampWithinCurve.boolValue = EditorGUI.Toggle(position, "Limit to Max?", clampWithinCurve.boolValue);

            position.y += 25;

            if (!clampWithinCurve.boolValue)
            {
                GUI.contentColor = Color.black;
                EditorGUI.DrawRect(new Rect(originalPosition.x + 15, position.y - 5, EditorGUIUtility.currentViewWidth - 60, 55), Color.yellow);

                position.width = EditorGUIUtility.currentViewWidth - 30;
                position.x = (EditorGUIUtility.currentViewWidth + 50) - EditorGUIUtility.currentViewWidth;

                var defaultLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / 2 - 60;
                // EditorGUI.DrawRect(position, Color.black);
                EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Post-Curve Equation Type"));

                position.width = EditorGUIUtility.currentViewWidth / 2 - 60;
                position.y += 25;
                position.height = 20;
                position.x = (EditorGUIUtility.currentViewWidth + 50) - EditorGUIUtility.currentViewWidth;

                GUI.contentColor = Color.white;
                // EditorGUI.DrawRect(position, Color.black);
                equationType.enumValueIndex = (int)(IncrementalEquation.IncrementalType)EditorGUI.EnumPopup(position, (IncrementalEquation.IncrementalType)System.Enum.GetValues(typeof(IncrementalEquation.IncrementalType)).GetValue(equationType.enumValueIndex));

                position.width = EditorGUIUtility.currentViewWidth / 2 - 60;
                position.x = (EditorGUIUtility.currentViewWidth + position.width + 85) - EditorGUIUtility.currentViewWidth;
                position.y -= 25;
                GUI.contentColor = Color.black;
                // EditorGUI.DrawRect(position, Color.white);
                EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("X Value"));
                position.y += 25;
                position.height = 20;
                position.width = EditorGUIUtility.currentViewWidth / 2 - 60;
                position.x = (EditorGUIUtility.currentViewWidth + position.width + 85) - EditorGUIUtility.currentViewWidth;

                //EditorGUI.DrawRect(position, Color.black);
                GUI.contentColor = Color.white;
                xValue.floatValue = EditorGUI.DelayedFloatField(position, xValue.floatValue);

                position.y += 40;
                EditorGUIUtility.labelWidth = defaultLabelWidth;
                position.x -= 185;
                position.x = originalPosition.x;
                // EditorGUI.DrawRect(position, Color.white);

                GUI.contentColor = Color.white;
            }

            position.width = EditorGUIUtility.currentViewWidth - 30;
            position.x = originalPosition.x + 10;

            // EditorGUI.DrawRect(position, Color.white);
            overrideMasterCurve.boolValue = EditorGUI.Toggle(position, "Use new curve?", overrideMasterCurve.boolValue);

            //position.y += 25;
            position.width = 225;
            //EditorGUI.DrawRect(position, Color.blue);

            if (overrideMasterCurve.boolValue)
            {
                position.x += 15;
                position.y += 30;

                EditorGUI.DrawRect(new Rect(originalPosition.x + 15, position.y - 5, EditorGUIUtility.currentViewWidth - 60, 145), Color.yellow);

                GUI.contentColor = Color.black;
                EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - (40 * 2);

                EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Level Range Values Curve (Normalized)"));

                position.width = EditorGUIUtility.currentViewWidth - 80;
                position.height = 110;
                position.y += 25;

                // EditorGUI.DrawRect(position, Color.blue);

                levelRangeValuesCurve.animationCurveValue = EditorGUI.CurveField(position, levelRangeValuesCurve.animationCurveValue);

                position.y += 110;

                GUI.contentColor = Color.white;
            }

            position.x = originalPosition.x + 10;
            position.height = 20;
            position.width = EditorGUIUtility.currentViewWidth - 40;
            position.y += 25;

            // EditorGUI.DrawRect(position, Color.blue);
            showDebugValues.boolValue = EditorGUI.Toggle(position, "Show Debug List", showDebugValues.boolValue);

            if (showDebugValues.boolValue)
            {
                // Calculate rects
                var debugLevelStartRect = new Rect((EditorGUIUtility.currentViewWidth + 30) - EditorGUIUtility.currentViewWidth, position.y + 30, 30, 15);

                var debugLevelEndRect = new Rect(EditorGUIUtility.currentViewWidth - (EditorGUIUtility.currentViewWidth * 0.475f), position.y + 30, 30, 15);

                var debugLevelStartValueRect = new Rect((EditorGUIUtility.currentViewWidth + 30) - EditorGUIUtility.currentViewWidth + 60, position.y + 30, (EditorGUIUtility.currentViewWidth / 2) - 100, 20);

                var debugLevelEndValueRect = new Rect(EditorGUIUtility.currentViewWidth - (EditorGUIUtility.currentViewWidth * 0.475f) + 65, position.y + 30, (EditorGUIUtility.currentViewWidth / 2) - 100, 20);


                EditorGUI.PrefixLabel(debugLevelStartRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Start"));

                EditorGUI.PrefixLabel(debugLevelEndRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("End"));

                debugLevelRange.vector2IntValue = new Vector2Int(EditorGUI.DelayedIntField(debugLevelStartValueRect, debugLevelRange.vector2IntValue.x),
                    EditorGUI.DelayedIntField(debugLevelEndValueRect, debugLevelRange.vector2IntValue.y));

                position.y += 65;
                position.width = EditorGUIUtility.currentViewWidth - 30;

                if (GUI.Button(position, "Update List"))
                {

                }
            }
        } // End of if (property.isExpanded)

        EditorGUI.EndFoldoutHeaderGroup();
        // EditorGUI.EndProperty();
    }
}
