using UnityEngine;

// ReSharper disable once CheckNamespace
namespace FiveGames.Tools.MoreMaths
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public static class MoreMaths
    {
        public static Vector3 Vector3BezierLerp(Vector3 startPos, Vector3 endPos, Vector3 controlPoint1,
            Vector3 controlPoint2, float t)
        {
            return Mathf.Pow(1 - t, 3) * startPos + 3 * Mathf.Pow(1 - t, 2) * t * controlPoint1 +
                   3 * (1 - t) * Mathf.Pow(t, 2) * controlPoint2 + Mathf.Pow(t, 3) * endPos;
        }

        [System.Serializable]
        public class IncrementalEquation
        {

            public enum GraphType
            {
                // y = mx+b
                // y is the value
                // x is the level
                // m is the slope =>      y2 - y1
                //                   m = ----------
                //                        x2 - x1
                // b is the y-intercept
                Linear,

                // y = ab^x
                // y is the value
                // x is the level
                Exponential
            }

            public GraphType equationType;
            
            public double valueAtLevelA; // this is y for x = 0
            public double valueAtLevelB; // this is y for x = 199

            public int levelA = 0; // this is y for x = 0
            public int levelB = 199; // this is y for x = 199

            public bool maxAtLevelB = false;
            
            #region Linear Helpers

            // ReSharper disable once InconsistentNaming
            private double m(double x1, double x2, double y1, double y2) => (y2 - y1) / (x2 - x1);

            // ReSharper disable once InconsistentNaming
            private double b(double x, double y, double m) => y - m * x;

            #endregion

            public IncrementalEquation()
            {
                valueAtLevelA = 1.0;
                valueAtLevelB = 199.0;
                levelA = 0;
                levelB = 199;
                equationType = GraphType.Linear;
            }

            public IncrementalEquation(GraphType equationType)
            {
                valueAtLevelA = 1.0;
                valueAtLevelB = 199.0;
                levelA = 0;
                levelB = 199;
                this.equationType = equationType;
            }

            public IncrementalEquation(double valueAtLevel1, double valueAtLevel200, GraphType equationType)
            {
                this.valueAtLevelA = valueAtLevel1 != 0.0 ? valueAtLevel1 : 1.0;
                this.valueAtLevelB = valueAtLevel200 != 0.0 ? valueAtLevel200 : 1.0;
                this.equationType = equationType;
            }

            public int GetIntValueAtLevel(int level)
            {
                var val = 0;

                if (maxAtLevelB && level > levelB) level = levelB;

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (equationType)
                {
                    case GraphType.Linear:
                        var mValue = m(levelA, levelB, valueAtLevelA, valueAtLevelB);
                        var bValue1 = b(levelA, valueAtLevelA, mValue);
                        val = Mathf.CeilToInt((float) (mValue * level + bValue1));
                        break;

                    case GraphType.Exponential:
                        //y1 = a*b^x1
                        //spinsRequiredLevel1 = a*b^0  --> 10 = a*1 --> a = spinsRequiredLevel1
                        // which means, when replacing 'a' in the 2nd equation, and solving for 'b'...
                        //spinsRequiredLevel200 = a*b^199 --> spinsRequiredLevel200 = spinsRequiredLevel1 * b^199 --> ...
                        //b = root(spinsRequiredLevel200/spinsRequiredLevel1, 199)
                        // which means, when replacing 'a' and 'b' in the general equation, using 'level' as x, we have ...
                        //y = spinsRequiredLevel1 * root(spinsRequiredLevel200/spinsRequiredLevel1, 199) ^ level
                        var aValue = valueAtLevelA;
                        var bValue2 = Mathf.Pow((float) valueAtLevelB / (float) valueAtLevelA, 1.0f / levelB);
                        val = Mathf.CeilToInt((float) (aValue * Mathf.Pow(bValue2, level)));
                        break;
                }

                return val;
            }

            public float GetFloatValueAtLevel(int level)
            {
                var val = 0f;

                if (maxAtLevelB && level > levelB) level = levelB;

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (equationType)
                {
                    case GraphType.Linear:
                        var mValue = m(levelA, levelB, valueAtLevelA, valueAtLevelB);
                        var bValue1 = b(levelA, valueAtLevelA, mValue);
                        val = (float) (mValue * level + bValue1);
                        break;

                    case GraphType.Exponential:
                        //y1 = a*b^x1
                        //spinsRequiredLevel1 = a*b^0  --> 10 = a*1 --> a = spinsRequiredLevel1
                        // which means, when replacing 'a' in the 2nd equation, and solving for 'b'...
                        //spinsRequiredLevel200 = a*b^199 --> spinsRequiredLevel200 = spinsRequiredLevel1 * b^199 --> ...
                        //b = root(spinsRequiredLevel200/spinsRequiredLevel1, 199)
                        // which means, when replacing 'a' and 'b' in the general equation, using 'level' as x, we have ...
                        //y = spinsRequiredLevel1 * root(spinsRequiredLevel200/spinsRequiredLevel1, 199) ^ level
                        var aValue = valueAtLevelA;
                        var bValue2 = Mathf.Pow((float) valueAtLevelB / (float) valueAtLevelA, 1.0f / levelB);
                        val = (float) (aValue * Mathf.Pow(bValue2, level));
                        break;
                }

                return val;
            }

            public double GetDoubleValueAtLevel(int level)
            {
                var val = 0.0;

                if (maxAtLevelB && level > levelB) level = levelB;

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (equationType)
                {
                    case GraphType.Linear:
                        var mValue = m(levelA, levelB, valueAtLevelA, valueAtLevelB);
                        var bValue1 = b(levelA, valueAtLevelA, mValue);
                        val = mValue * level + bValue1;
                        break;

                    case GraphType.Exponential:
                        //y1 = a*b^x1
                        //spinsRequiredLevel1 = a*b^0  --> 10 = a*1 --> a = spinsRequiredLevel1
                        // which means, when replacing 'a' in the 2nd equation, and solving for 'b'...
                        //spinsRequiredLevel200 = a*b^199 --> spinsRequiredLevel200 = spinsRequiredLevel1 * b^199 --> ...
                        //b = root(spinsRequiredLevel200/spinsRequiredLevel1, 199)
                        // which means, when replacing 'a' and 'b' in the general equation, using 'level' as x, we have ...
                        //y = spinsRequiredLevel1 * root(spinsRequiredLevel200/spinsRequiredLevel1, 199) ^ level
                        var aValue = valueAtLevelA;
                        var bValue2 = Mathf.Pow((float) valueAtLevelB / (float) valueAtLevelA, 1.0f / levelB);
                        val = aValue * Mathf.Pow(bValue2, level);
                        break;
                }

                return val;
            }
        }
    }
}