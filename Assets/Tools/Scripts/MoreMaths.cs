using UnityEngine;

namespace Five.MoreMaths
{
    public static class MoreMaths
    {
        public static Vector3 Blerp(Vector3 startPos, Vector3 endPos, float t)
        {
            var controlPoint1 = Vector3.Lerp(startPos, endPos, 0.25f);
            var controlPoint2 = Vector3.Lerp(startPos, endPos, 0.75f);

            return Mathf.Pow(1 - t, 3) * startPos + 3 * Mathf.Pow(1 - t, 2) * t * controlPoint1 +
                   3 * (1 - t) * Mathf.Pow(t, 2) * controlPoint2 + Mathf.Pow(t, 3) * endPos;
        }

        public static Vector3 Blerp(Vector3 startPos, Vector3 endPos, float controlPointY, float t)
        {
            var controlPoint1 = Vector3.Lerp(startPos, endPos, 0.25f);
            var controlPoint2 = Vector3.Lerp(startPos, endPos, 0.75f);

            controlPoint1.y = controlPointY;
            controlPoint2.y = controlPointY;

            return Mathf.Pow(1 - t, 3) * startPos + 3 * Mathf.Pow(1 - t, 2) * t * controlPoint1 +
                   3 * (1 - t) * Mathf.Pow(t, 2) * controlPoint2 + Mathf.Pow(t, 3) * endPos;
        }

        public static Vector3 Blerp(Vector3 startPos, Vector3 endPos, Vector3 controlPoint1, Vector3 controlPoint2, float t)
        {
            return Mathf.Pow(1 - t, 3) * startPos + 3 * Mathf.Pow(1 - t, 2) * t * controlPoint1 +
                   3 * (1 - t) * Mathf.Pow(t, 2) * controlPoint2 + Mathf.Pow(t, 3) * endPos;
        }
    }

    [System.Serializable]
    public class IncrementalEquation
    {
        public Vector2Int levelRange = new Vector2Int(1, 50);
        public Vector2 valueRange = new Vector2(5.0f, 10.0f);

        public AnimationCurve levelRangeValuesCurve = new AnimationCurve(new Keyframe[] {
            new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f) });

        public bool clampWithinCurve = false;
        public bool overrideMasterCurve = true;

        public IncrementalType equationType = IncrementalType.PreviousMultiplyByX;
        public float xValue = 1.07f;

        // Debug variables.
        public Vector2Int debugLevelRange = new Vector2Int(1, 20);
        public bool showDebugValues = false;

        public enum IncrementalType { PreviousPlusX, PreviousPowerOfX, PreviousMultiplyByX }

        private IncrementalEquation masterCurveEq;

        public void SetMasterCurve(IncrementalEquation masterCurveEquation)
        {
            masterCurveEq = masterCurveEquation;
        }

        /// <summary>
        /// Evaluates and returns the integer value for the given level, using FloorToInt(value).
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int GetIntValue(int level)
        {
            var curve = (overrideMasterCurve) ? levelRangeValuesCurve : masterCurveEq.levelRangeValuesCurve;

            if (masterCurveEq is null && !overrideMasterCurve) 
                throw new System.Exception("OverrideMasterCurve is false but no masterCurve is given!");

            if (clampWithinCurve && level > levelRange.y) level = levelRange.y;

            if (level > levelRange.y)
            {
                switch (equationType)
                {
                    case IncrementalType.PreviousPlusX:
                        return Mathf.FloorToInt(valueRange.y + (level - levelRange.y) * xValue);

                    case IncrementalType.PreviousPowerOfX:
                        var extraLevels = level - levelRange.y;
                        var lastValue = valueRange.y;

                        for (var i = 0; i < extraLevels; i++)
                            lastValue = Mathf.Pow(lastValue, xValue);

                        return Mathf.FloorToInt(lastValue);

                    case IncrementalType.PreviousMultiplyByX:
                        return Mathf.FloorToInt(valueRange.y * Mathf.Pow(xValue, level - levelRange.y));

                    default:
                        throw new System.Exception("EquationType not found! " + equationType);
                }
            }
            else
            {
                if (level < levelRange.x)
                {
                    var invertedLevelRange = levelRange;

                    // i.e. levelRange(5, 10) and level(2)
                    // invertedRange(-10, 4) 
                    invertedLevelRange.y = levelRange.x - 1;
                    invertedLevelRange.x = -levelRange.y;

                    var invertedValueRange = valueRange;

                    // i.e. valueRange(10, 50)
                    // invertedValueRange(-50, 9);
                    invertedValueRange.y = valueRange.x - 1;
                    invertedValueRange.x = -valueRange.y;

                    // [1.] Normalize level based on invertedLevelRange.
                    var normalizedLevel = (float)(level - invertedLevelRange.x) / (invertedLevelRange.y - invertedLevelRange.x);

                    // [2.] Get value from curve.
                    var curveValue = curve.Evaluate(normalizedLevel);

                    // [3.] Denormalize curve value and return.
                    return Mathf.FloorToInt(Mathf.Lerp(invertedValueRange.x, invertedValueRange.y, curveValue));
                }
                else
                {
                    // [1.] Normalize level based on levelRange.
                    var normalizedLevel = (float)(level - levelRange.x) / (levelRange.y - levelRange.x);

                    // [2.] Get value from curve.
                    var curveValue = curve.Evaluate(normalizedLevel);

                    // [3.] Denormalize curve value and return.
                    return Mathf.FloorToInt(Mathf.Lerp(valueRange.x, valueRange.y, curveValue));
                }
            }
        }

        /// <summary>
        /// Evaluates and returns the integer value for the given level, using FloorToInt(value).
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int GetCeiledIntValue(int level)
        {
            var curve = (overrideMasterCurve) ? levelRangeValuesCurve : masterCurveEq.levelRangeValuesCurve;

            if (masterCurveEq is null && !overrideMasterCurve)
                throw new System.Exception("OverrideMasterCurve is false but no masterCurve is given!");

            if (clampWithinCurve && level > levelRange.y) level = levelRange.y;

            if (level > levelRange.y)
            {
                switch (equationType)
                {
                    case IncrementalType.PreviousPlusX:
                        return Mathf.CeilToInt(valueRange.y + (level - levelRange.y) * xValue);

                    case IncrementalType.PreviousPowerOfX:
                        var extraLevels = level - levelRange.y;
                        var lastValue = valueRange.y;

                        for (var i = 0; i < extraLevels; i++)
                            lastValue = Mathf.Pow(lastValue, xValue);

                        return Mathf.CeilToInt(lastValue);

                    case IncrementalType.PreviousMultiplyByX:
                        return Mathf.CeilToInt(valueRange.y * Mathf.Pow(xValue, level - levelRange.y));

                    default:
                        throw new System.Exception("EquationType not found! " + equationType);
                }
            }
            else
            {
                if (level < levelRange.x)
                {
                    var invertedLevelRange = levelRange;

                    // i.e. levelRange(5, 10) and level(2)
                    // invertedRange(-10, 4) 
                    invertedLevelRange.y = levelRange.x - 1;
                    invertedLevelRange.x = -levelRange.y;

                    var invertedValueRange = valueRange;

                    // i.e. valueRange(10, 50)
                    // invertedValueRange(-50, 9);
                    invertedValueRange.y = valueRange.x - 1;
                    invertedValueRange.x = -valueRange.y;

                    // [1.] Normalize level based on invertedLevelRange.
                    var normalizedLevel = (float)(level - invertedLevelRange.x) / (invertedLevelRange.y - invertedLevelRange.x);

                    // [2.] Get value from curve.
                    var curveValue = curve.Evaluate(normalizedLevel);

                    // [3.] Denormalize curve value and return.
                    return Mathf.CeilToInt(Mathf.Lerp(invertedValueRange.x, invertedValueRange.y, curveValue));
                }
                else
                {
                    // [1.] Normalize level based on levelRange.
                    var normalizedLevel = (float)(level - levelRange.x) / (levelRange.y - levelRange.x);

                    // [2.] Get value from curve.
                    var curveValue = curve.Evaluate(normalizedLevel);

                    // [3.] Denormalize curve value and return.
                    return Mathf.CeilToInt(Mathf.Lerp(valueRange.x, valueRange.y, curveValue));
                }
            }
        }

        /// <summary>
        /// Evaluates and returns the float value for the given level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public float GetFloatValue(int level)
        {
            var curve = (overrideMasterCurve) ? levelRangeValuesCurve : masterCurveEq.levelRangeValuesCurve;

            if (masterCurveEq is null && !overrideMasterCurve)
                throw new System.Exception("OverrideMasterCurve is false but no masterCurve is given!");

            if (clampWithinCurve && level > levelRange.y) level = levelRange.y;

            if (level > levelRange.y)
            {
                switch (equationType)
                {
                    case IncrementalType.PreviousPlusX:
                        return (valueRange.y + (level - levelRange.y) * xValue);

                    case IncrementalType.PreviousPowerOfX:
                        var extraLevels = level - levelRange.y;
                        var lastValue = valueRange.y;

                        for (var i = 0; i < extraLevels; i++)
                            lastValue = Mathf.Pow(lastValue, xValue);

                        return lastValue;

                    case IncrementalType.PreviousMultiplyByX:
                        return (valueRange.y * Mathf.Pow(xValue, level - levelRange.y));

                    default:
                        throw new System.Exception("EquationType not found! " + equationType);
                }
            }
            else
            {
                if (level < levelRange.x)
                {
                    var invertedLevelRange = levelRange;

                    // i.e. levelRange(5, 10) and level(2)
                    // invertedRange(-10, 4) 
                    invertedLevelRange.y = levelRange.x - 1;
                    invertedLevelRange.x = -levelRange.y;

                    var invertedValueRange = valueRange;

                    // i.e. valueRange(10, 50)
                    // invertedValueRange(-50, 9);
                    invertedValueRange.y = valueRange.x - 1;
                    invertedValueRange.x = -valueRange.y;

                    // [1.] Normalize level based on invertedLevelRange.
                    var normalizedLevel = (float)(level - invertedLevelRange.x) / (invertedLevelRange.y - invertedLevelRange.x);

                    // [2.] Get value from curve.
                    var curveValue = curve.Evaluate(normalizedLevel);

                    // [3.] Denormalize curve value and return.
                    return (Mathf.Lerp(invertedValueRange.x, invertedValueRange.y, curveValue));
                }
                else
                {
                    // [1.] Normalize level based on levelRange.
                    var normalizedLevel = (float)(level - levelRange.x) / (levelRange.y - levelRange.x);

                    // [2.] Get value from curve.
                    var curveValue = curve.Evaluate(normalizedLevel);

                    // [3.] Denormalize curve value and return.
                    return (Mathf.Lerp(valueRange.x, valueRange.y, curveValue));
                }
            }
        }

        /// <summary>
        /// Evaluates and returns the double value for the given level, using explicit cast.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public double GetDoubleValue(int level)
        {
            var curve = (overrideMasterCurve) ? levelRangeValuesCurve : masterCurveEq.levelRangeValuesCurve;

            if (masterCurveEq is null && !overrideMasterCurve)
                throw new System.Exception("OverrideMasterCurve is false but no masterCurve is given!");

            if (clampWithinCurve && level > levelRange.y) level = levelRange.y;

            if (level > levelRange.y)
            {
                switch (equationType)
                {
                    case IncrementalType.PreviousPlusX:
                        return (double)(valueRange.y + (level - levelRange.y) * xValue);

                    case IncrementalType.PreviousPowerOfX:
                        var extraLevels = level - levelRange.y;
                        var lastValue = valueRange.y;

                        for (var i = 0; i < extraLevels; i++)
                            lastValue = Mathf.Pow(lastValue, xValue);

                        return (double)lastValue;

                    case IncrementalType.PreviousMultiplyByX:
                        return (double)(valueRange.y * Mathf.Pow(xValue, level - levelRange.y));

                    default:
                        throw new System.Exception("EquationType not found! " + equationType);
                }
            }
            else
            {
                if (level < levelRange.x)
                {
                    var invertedLevelRange = levelRange;

                    // i.e. levelRange(5, 10) and level(2)
                    // invertedRange(-10, 4) 
                    invertedLevelRange.y = levelRange.x - 1;
                    invertedLevelRange.x = -levelRange.y;

                    var invertedValueRange = valueRange;

                    // i.e. valueRange(10, 50)
                    // invertedValueRange(-50, 9);
                    invertedValueRange.y = valueRange.x - 1;
                    invertedValueRange.x = -valueRange.y;

                    // [1.] Normalize level based on invertedLevelRange.
                    var normalizedLevel = (float)(level - invertedLevelRange.x) / (invertedLevelRange.y - invertedLevelRange.x);

                    // [2.] Get value from curve.
                    var curveValue = curve.Evaluate(normalizedLevel);

                    // [3.] Denormalize curve value and return.
                    return (double)(Mathf.Lerp(invertedValueRange.x, invertedValueRange.y, curveValue));
                }
                else
                {
                    // [1.] Normalize level based on levelRange.
                    var normalizedLevel = (float)(level - levelRange.x) / (levelRange.y - levelRange.x);

                    // [2.] Get value from curve.
                    var curveValue = curve.Evaluate(normalizedLevel);

                    // [3.] Denormalize curve value and return.
                    return (double)(Mathf.Lerp(valueRange.x, valueRange.y, curveValue));
                }
            }
        }
    }
}
