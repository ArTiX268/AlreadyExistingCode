using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Effects
{
    public class Tween : MonoBehaviour
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

        #region Static

        [Serializable]
        public struct AnimParams
        {
            public float duration;
            public ETransition transition;
        }

        public static Tween Create()
        {
            Tween tween;
            if (disabledTween.Count > 0)
            {
                tween = disabledTween[0];
                tween.enabled = true;
                disabledTween.RemoveAt(0);
            }
            else
                tween = new GameObject().AddComponent(typeof(Tween)) as Tween;

            return tween;
        }

        private readonly static List<Tween> disabledTween = new List<Tween>();

        public static float InterpolateValue(in float startValue, in float targetValue, float percentage, in ETransition transition)
        {
            percentage = Mathf.Clamp01(percentage);
            float result = GetValue(percentage, transition);
            return Mathf.LerpUnclamped(startValue, targetValue, result);
        }

        public static Vector2 InterpolateValue(in Vector2 startValue, in Vector2 targetValue, float percentage, in ETransition transition)
        {
            percentage = Mathf.Clamp01(percentage);
            float result = GetValue(percentage, transition);
            return Vector2.LerpUnclamped(startValue, targetValue, result);
        }

        public static float InterpolateValue(in float startValue, in float targetValue, in float elapsedTime, in AnimParams animParams)
            => InterpolateValue(startValue, targetValue, animParams.duration, elapsedTime, animParams.transition);

        public static Vector2 InterpolateValue(in Vector2 startValue, in Vector2 targetValue, in float elapsedTime, in AnimParams animParams)
            => InterpolateValue(startValue, targetValue, animParams.duration, elapsedTime, animParams.transition);

        public static float InterpolateValue(in float startValue, in float targetValue, in float duration, float elapsedTime, ETransition transition = ETransition.Linear)
            => InterpolateValue(startValue, targetValue, elapsedTime / duration, transition);

        public static Vector2 InterpolateValue(in Vector2 startValue, in Vector2 targetValue, in float duration, float elapsedTime, ETransition transition = ETransition.Linear)
            => InterpolateValue(startValue, targetValue, elapsedTime / duration, transition);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="easing"></param>
        /// <param name="transition"></param>
        /// <returns>A value between 0 & 1 which corresponds to the given equation</returns>
        private static float GetValue(float t, in ETransition transition)
        {
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
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return (C3 * t * t * t) - (C1 * t * t);

                case ETransition.StopBack:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return 1 + (C3 * (t - 1) * (t - 1) * (t - 1)) + (C1 * (t - 1) * (t - 1));

                case ETransition.StepBack:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return t < 0.5f
                            ? 4 * t * t * (((C2 + 1) * 2 * t) - C2) * 0.5f
                            : ((Mathf.Pow((2 * t) - 2, 2) * (((C2 + 1) * ((t * 2) - 2)) + C2)) + 2) * 0.5f;

                case ETransition.StartElastic:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return -Mathf.Pow(2, (10 * t) - 10) * Mathf.Sin(((t * 10) - 10.75f) * C4);

                case ETransition.StopElastic:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return (Mathf.Pow(2, -10 * t) * Mathf.Sin(((t * 10) - 0.75f) * C4)) + 1;

                case ETransition.StepElastic:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return t < 0.5f
                            ? -(Mathf.Pow(2, (20 * t) - 10) * Mathf.Sin(((20 * t) - 11.125f) * C5)) * 0.5f
                            : (Mathf.Pow(2, (-20 * t) + 10) * Mathf.Sin(((20 * t) - 11.125f) * C5) * 0.5f) + 1;

                case ETransition.StartBounce:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return 1 - GetValue(1 - t, ETransition.StopBounce);

                case ETransition.StopBounce:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    if (t < 1 / D1) return N1 * t * t;
                    else if (t < 2 / D1) return (N1 * (t -= 1.5f / D1) * t) + 0.75f;
                    else if (t < 2.5 / D1) return (N1 * (t -= 2.25f / D1) * t) + 0.9375f;
                    else return (N1 * (t -= 2.625f / D1) * t) + 0.984375f;

                case ETransition.StepBounce:
                    if (t == 0) return 0;
                    else if (t == 1) return 1;
                    return 0.5f * (t < 0.5f
                            ? (1 - GetValue(1 - (2 * t), ETransition.StopBounce))
                            : (1 + GetValue((2 * t) - 1, ETransition.StopBounce)));
            }

            return t;
        }

        #endregion

        #region Instance

        private Tween() { }

        private class Tweener
        {
            private readonly Action OnFinishedEvent;

            private readonly float startValue;
            private readonly float targetValue;
            private readonly AnimParams animParams;
            private readonly Action<float> tweenEvent;
            private float delay;
            private bool scaled;

            private float timer;

            public Tweener(float startValue, float targetValue, Action<float> tweenEvent, float delay, in AnimParams animParams, bool scaled, in Action onFinishedEvent)
            {
                this.startValue = startValue;
                this.targetValue = targetValue;
                this.animParams = animParams;
                this.tweenEvent = tweenEvent;
                this.delay = delay;
                this.scaled = scaled;
                OnFinishedEvent = onFinishedEvent;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns>True when finished, false otherwise.</returns>
            public bool UpdateTween()
            {
                if (delay > 0)
                {
                    delay -= scaled ? Time.deltaTime : Time.unscaledDeltaTime;
                    return false;
                }

                timer = Mathf.Clamp(timer + (scaled ? Time.deltaTime : Time.unscaledDeltaTime), 0, animParams.duration);

                tweenEvent.Invoke(InterpolateValue(startValue, targetValue, timer, animParams));

                if (timer == animParams.duration)
                {
                    OnFinishedEvent?.Invoke();
                    return true;
                }

                return false;
            }
        }

        public event Action OnFinishedEvent;

        private List<List<Tweener>> tweeners = new List<List<Tweener>>();

        private void Update()
        {
            if (tweeners.Count > 0)
            {
                List<Tweener> currentTweeners = tweeners[0];
                for (int i = currentTweeners.Count - 1; i >= 0; i--)
                {
                    if (currentTweeners[i].UpdateTween())
                        currentTweeners.RemoveAt(i);
                }

                if (currentTweeners.Count == 0) tweeners.RemoveAt(0);
            }
            else
            {
                OnFinishedEvent?.Invoke();
                Kill();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="targetValue"></param>
        /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
        /// <param name="animParams"></param>
        /// <param name="delay"></param>
        /// <param name="parrellel"></param>
        /// <param name="timeScaled"></param>
        /// <param name="onFinishedEvent"></param>
        public void TweenEvent(in float startValue, in float targetValue, in Action<float> tweenEvent, in AnimParams animParams,
            in float delay = 0, in bool parrellel = false, in bool timeScaled = false, in Action onFinishedEvent = null)
        {
            List<Tweener> tweenerList;

            if (!parrellel || tweeners.Count == 0)
            {
                tweenerList = new List<Tweener>();
                tweeners.Add(tweenerList);
            }
            else
                tweenerList = tweeners[tweeners.Count - 1];

            tweenerList.Add(new Tweener(startValue, targetValue, tweenEvent, delay, animParams, timeScaled, onFinishedEvent));
        }

        public void Kill()
        {
            if (enabled)
            {
                tweeners.Clear();
                disabledTween.Add(this);
                enabled = false;
            }
        }

        private void OnDestroy()
        {
            disabledTween.Clear();
        }

        #endregion
    }
}
