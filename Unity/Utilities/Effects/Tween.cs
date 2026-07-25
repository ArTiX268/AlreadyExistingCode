using System;
using UnityEngine;

namespace ArTiX.Effects
{
    public static class Tween
    {
        public enum ETransition
        {
            Linear,
            SmoothStart2,
            SmoothStart3,
            SmoothStart4,
            SmoothStart5,
            SmoothStop2,
            SmoothStop3,
            SmoothStop4,
            SmoothStop5,
            SmoothStep3,
            SmoothStep4,
            SmoothStep5,
            Arch2,
            SmoothStartArch3,
            SmoothStopArch3,
            SmoothStartExpo,
            SmoothStopExpo,
            SmoothStepExpo,
            StartBack,
            StopBack,
            StepBack,
            StartElastic,
            StopElastic,
            StepElastic,
            StartBounce,
            StopBounce,
            StepBounce
        }

        [Serializable]
        public struct AnimParams
        {
            public float duration;
            public ETransition transition;
        }

        public static float InterpolateValue(in float startValue, in float targetValue, in float elapsedTime, in AnimParams animParams)
            => InterpolateValue(startValue, targetValue, animParams.duration, elapsedTime, animParams.transition);

        public static Vector2 InterpolateValue(in Vector2 startValue, in Vector2 targetValue, in float elapsedTime, in AnimParams animParams)
            => InterpolateValue(startValue, targetValue, animParams.duration, elapsedTime, animParams.transition);

        public static float InterpolateValue(in float startValue, in float targetValue, in float duration, in float elapsedTime, ETransition transition = ETransition.Linear)
        {
            float result = GetValue(elapsedTime / duration, transition);
            return Mathf.LerpUnclamped(startValue, targetValue, result);
        }

        public static Vector2 InterpolateValue(in Vector2 startValue, in Vector2 targetValue, in float duration, in float elapsedTime, ETransition transition = ETransition.Linear)
        {
            float result = GetValue(elapsedTime / duration, transition);
            return Vector2.LerpUnclamped(startValue, targetValue, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="easing"></param>
        /// <param name="transition"></param>
        /// <returns>A value between 0 & 1 which corresponds to the given equation</returns>
        private static float GetValue(float t, in ETransition transition)
        {
            if (t == 0) return 0;
            else if (t == 1) return 1;

            const float C1 = 1.70158f;
            const float C2 = C1 * 1.525f;
            const float C3 = C1 + 1;
            const float C4 = 2 * Mathf.PI / 3;
            const float C5 = 2 * Mathf.PI / 4.5f;
            const float N1 = 7.5625f;
            const float D1 = 2.75f;

            switch (transition)
            {
                case ETransition.Linear:
                    return t;
                case ETransition.SmoothStart2:
                    return t * t;
                case ETransition.SmoothStart3:
                    return t * t * t;
                case ETransition.SmoothStart4:
                    return t * t * t * t;
                case ETransition.SmoothStart5:
                    return t * t * t * t * t;
                case ETransition.SmoothStop2:
                    return 1 - GetValue(1 - t, ETransition.SmoothStart2);
                case ETransition.SmoothStop3:
                    return 1 - GetValue(1 - t, ETransition.SmoothStart3);
                case ETransition.SmoothStop4:
                    return 1 - GetValue(1 - t, ETransition.SmoothStart4);
                case ETransition.SmoothStop5:
                    return 1 - GetValue(1 - t, ETransition.SmoothStart5);
                case ETransition.SmoothStep3:
                    return Mathf.Lerp(GetValue(t, ETransition.SmoothStart2), GetValue(t, ETransition.SmoothStop2), t);
                case ETransition.SmoothStep4:
                    return Mathf.Lerp(GetValue(t, ETransition.SmoothStart3), GetValue(t, ETransition.SmoothStop3), t);
                case ETransition.SmoothStep5:
                    return Mathf.Lerp(GetValue(t, ETransition.SmoothStart4), GetValue(t, ETransition.SmoothStop4), t);
                case ETransition.Arch2:
                    return t * (1 - t) * 4;
                case ETransition.SmoothStartArch3:
                    return t * (1 - t) * t / (2f / 3f * (1 - (2f / 3f)) * t);
                case ETransition.SmoothStopArch3:
                    return t * GetValue(1 - t, ETransition.SmoothStart2) / (1f / 3f * GetValue(1 - (1f / 3f), ETransition.SmoothStart2));
                case ETransition.SmoothStartExpo:
                    return Mathf.Pow(2, (10 * t) - 10);
                case ETransition.SmoothStopExpo:
                    return 1 - Mathf.Pow(2, -10 * t);
                    case ETransition.SmoothStepExpo:
                    return 0.5f * (t < 0.5f ? Mathf.Pow(2, (20 * t) - 10)
                            : (2 - Mathf.Pow(2, (-20 * t) + 10)));
                case ETransition.StartBack:
                    return (C3* t *t * t) - (C1 * t * t);
                case ETransition.StopBack:
                    return 1 + (C3 * (t - 1) * (t - 1) * (t - 1)) + (C1 * (t - 1) * (t - 1));
                case ETransition.StepBack:
                    return t < 0.5f
                            ? 4 * t * t * (((C2 + 1) * 2 * t) - C2) * 0.5f
                            : ((Mathf.Pow((2 * t) - 2, 2) * (((C2 + 1) * ((t * 2) - 2)) + C2)) + 2) * 0.5f;
                case ETransition.StartElastic:
                    return -Mathf.Pow(2, (10 * t) - 10) * Mathf.Sin(((t * 10) - 10.75f) * C4);
                case ETransition.StopElastic:
                    return (Mathf.Pow(2, -10 * t) * Mathf.Sin(((t * 10) - 0.75f) * C4)) + 1;
                case ETransition.StepElastic:
                    return t < 0.5f
                            ? -(Mathf.Pow(2, (20 * t) - 10) * Mathf.Sin(((20 * t) - 11.125f) * C5)) * 0.5f
                            : (Mathf.Pow(2, (-20 * t) + 10) * Mathf.Sin(((20 * t) - 11.125f) * C5) * 0.5f) + 1;
                case ETransition.StartBounce:
                    return 1 - GetValue(1 - t, ETransition.StopBounce);
                case ETransition.StopBounce:
                    if (t < 1 / D1) return N1 * t * t;
                    else if (t < 2 / D1) return (N1 * (t -= 1.5f / D1) * t) + 0.75f;
                    else if (t < 2.5 / D1) return (N1 * (t -= 2.25f / D1) * t) + 0.9375f;
                    else return (N1 * (t -= 2.625f / D1) * t) + 0.984375f;
                case ETransition.StepBounce:
                    return 0.5f * (t < 0.5f
                            ? (1 - GetValue(1 - (2 * t), ETransition.StopBounce))
                            : (1 + GetValue((2 * t) - 1, ETransition.StopBounce)));
            }

            return t;
        }
    }
}
