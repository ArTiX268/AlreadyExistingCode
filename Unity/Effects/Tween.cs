using UnityEngine;

namespace ArTiX.Effects
{
    public static class Tween
    {
        public enum EEasing : byte
        {
            InOut,
            In,
            Out
        }
        public enum ETransition
        {
            Linear,
            Sine,
            /// <summary>
            /// x²
            /// </summary>
            Quad,
            /// <summary>
            /// x at the power of 3
            /// </summary>
            Cubic,
            /// <summary>
            /// x at the power of 4
            /// </summary>
            Quart,
            /// <summary>
            /// x at the power of 5
            /// </summary>
            Quint,
            Circ,
            Elastic,
            Expo,
            Back,
            Bounce
        }

        public static float InterpolateValue(in float startValue, in float targetValue, in float duration, in float elapsedTime, EEasing easing = EEasing.In, ETransition transition = ETransition.Linear)
        {
            float result = GetValue(elapsedTime / duration, easing, transition);
            return Mathf.LerpUnclamped(startValue, targetValue, result);
        }
        public static Vector2 InterpolateValue(in Vector2 startValue, in Vector2 targetValue, in float duration, in float elapsedTime, EEasing easing = EEasing.In, ETransition transition = ETransition.Linear)
        {
            float result = GetValue(elapsedTime / duration, easing, transition);
            return Vector2.LerpUnclamped(startValue, targetValue, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="easing"></param>
        /// <param name="transition"></param>
        /// <returns>A value between 0 & 1 which corresponds to the given equation</returns>
        private static float GetValue(float t, in EEasing easing, in ETransition transition)
        {
            if (transition == ETransition.Sine)
            {
                switch (easing)
                {
                    case EEasing.In:
                        return 1 - Mathf.Cos((t * Mathf.PI) * 0.5f);
                    case EEasing.Out:
                        return Mathf.Sin((t * Mathf.PI) * 0.5f);
                    case EEasing.InOut:
                        return -(Mathf.Cos(Mathf.PI * t) - 1) * 0.5f;
                }
            }
            if (transition == ETransition.Quad)
            {
                switch (easing)
                {
                    case EEasing.In:
                        return t * t;
                    case EEasing.Out:
                        return 1 - ((1 - t) * (1 - t));
                    case EEasing.InOut:
                        return t < 0.5f ?
                            2 * t * t :
                            1 - Mathf.Pow(-2 * t + 2, 2) * 0.5f;
                }
            }
            if (transition == ETransition.Cubic)
            {
                switch (easing)
                {
                    case EEasing.In:
                        return t * t * t;
                    case EEasing.Out:
                        return 1 - ((1 - t) * (1 - t) * (1 - t));
                    case EEasing.InOut:
                        return t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) * 0.5f;
                }
            }
            if (transition == ETransition.Quart)
            {
                switch (easing)
                {
                    case EEasing.In:
                        return t * t * t * t;
                    case EEasing.Out:
                        return 1 - ((1 - t) * (1 - t) * (1 - t) * (1 - t));
                    case EEasing.InOut:
                        return t < 0.5f ?
                            8 * t * t * t * t :
                            1 - Mathf.Pow(-2 * t + 2, 4) / 2;
                }
            }
            if (transition == ETransition.Quint)
            {
                switch (easing)
                {
                    case EEasing.In:
                        return t * t * t * t * t * t * t * t;
                    case EEasing.Out:
                        return 1 - ((1 - t) * (1 - t) * (1 - t) * (1 - t) * (1 - t));
                    case EEasing.InOut:
                        return t < 0.5f ?
                            16 * t * t * t * t * t
                            : 1 - Mathf.Pow(-2 * t + 2, 5) * 0.5f;
                }
            }
            if (transition == ETransition.Circ)
            {
                switch (easing)
                {
                    case EEasing.In:
                        return 1 - Mathf.Sqrt(1 - t * t);
                    case EEasing.Out:
                        return 1 - Mathf.Sqrt(1 - ((t - 1) * (t - 1)));
                    case EEasing.InOut:
                        return t < 0.5f
                                    ? (1 - Mathf.Sqrt(1 - 4 * t * t)) * 0.5f
                                    : (Mathf.Sqrt(1 - Mathf.Pow(-2 * t + 2, 2)) + 1) * 0.5f;
                }
            }
            if (transition == ETransition.Elastic)
            {
                if (t == 0) return 0;
                else if (t == 1) return 1;

                const float C4 = (2 * Mathf.PI) / 3;
                const float C5 = (2 * Mathf.PI) / 4.5f;

                switch (easing)
                {
                    case EEasing.In:
                        return -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * C4);

                    case EEasing.Out:
                        return Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * C4) + 1;
                    case EEasing.InOut:
                        return t < 0.5f
                            ? -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * C5)) * 0.5f
                            : (Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * C5)) * 0.5f + 1;
                }
            }
            if (transition == ETransition.Expo)
            {
                if (t == 0) return 0;
                else if (t == 1) return 1;

                switch (easing)
                {
                    case EEasing.In:
                        return Mathf.Pow(2, 10 * t - 10);
                    case EEasing.Out:
                        return 1 - Mathf.Pow(2, -10 * t);
                    case EEasing.InOut:
                        return t < 0.5f ? Mathf.Pow(2, 20 * t - 10) * 0.5f
                            : (2 - Mathf.Pow(2, -20 * t + 10)) * 0.5f;
                }
            }
            if (transition == ETransition.Back)
            {
                if (t == 0) return 0;
                else if (t == 1) return 1;

                const float C1 = 1.70158f;
                const float C2 = C1 * 1.525f;
                const float C3 = C1 + 1;

                switch (easing)
                {
                    case EEasing.In:
                        return C3 * t * t * t - C1 * t * t;
                    case EEasing.Out:
                        return 1 + C3 * ((t - 1) * (t - 1) * (t - 1) + C1 * (t - 1) * (t - 1));
                    case EEasing.InOut:
                        return t < 0.5f
                            ? (4 * t * t * ((C2 + 1) * 2 * t - C2)) * 0.5f
                            : (Mathf.Pow(2 * t - 2, 2) * ((C2 + 1) * (t * 2 - 2) + C2) + 2) * 0.5f;
                }
            }
            if (transition == ETransition.Bounce)
            {
                const float N1 = 7.5625f;
                const float D1 = 2.75f;

                switch (easing)
                {
                    case EEasing.In:
                        return 1 - GetValue(1 - t, EEasing.Out, transition);
                    case EEasing.Out:
                        if (t < 1 / D1) return N1 * t * t;
                        else if (t < 2 / D1) return N1 * (t -= 1.5f / D1) * t + 0.75f;
                        else if (t < 2.5 / D1) return N1 * (t -= 2.25f / D1) * t + 0.9375f;
                        else return N1 * (t -= 2.625f / D1) * t + 0.984375f;
                    case EEasing.InOut:
                        return t < 0.5f
                            ? (1 - GetValue(1 - 2 * t, EEasing.Out, transition)) * 0.5f
                            : (1 + GetValue(2 * t - 1, EEasing.Out, transition)) * 0.5f;
                }
            }

            return t;
        }
    }
}
