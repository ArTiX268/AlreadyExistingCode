using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Effects.Tween
{
    public enum ETransformProperty
    {
        Position,
        Rotation,
        Scale
    }

    public class MasterTween : MonoBehaviour
    {
        #region Structs, Classes & Enums

        #region Tweener

        // ABSTRACTS
        private abstract class Tweener
        {
            protected readonly AnimParams animParams;

            private readonly bool scaled;

            public float Delay { get; private set; }

            protected float timer;

            public readonly Action OnFinishedStepEvent;

            protected Tweener(AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent)
            {
                this.animParams = animParams;
                this.scaled = scaled;
                Delay = delay;
                OnFinishedStepEvent = onFinishedStepEvent;

                timer = 0;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns>True when finished, false otherwise.</returns>
            public bool UpdateTweener()
            {
                if (Delay > 0)
                {
                    Delay -= scaled ? Time.deltaTime : Time.deltaTime / Time.timeScale;
                    return false;
                }

                timer += scaled ? Time.deltaTime : Time.deltaTime / Time.timeScale;

                InvokeEvent();

                if (timer >= animParams.Duration)
                {
                    OnFinishedStepEvent?.Invoke();
                    return true;
                }

                return false;
            }

            protected abstract void InvokeEvent();
        }

        private abstract class EventTweener<T> : Tweener
        {
            protected readonly T startValue;
            protected readonly T targetValue;

            private readonly Action<T> tweenEvent;

            protected EventTweener(T startValue, T targetValue, Action<T> tweenEvent, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
                base(animParams, delay, scaled, onFinishedStepEvent)
            {
                this.startValue = startValue;
                this.targetValue = targetValue;
                this.tweenEvent = tweenEvent;
            }

            protected EventTweener(TweeningParams<T> tweenParams, Action<T> tweenEvent) :
                base(tweenParams.animParams, tweenParams.delay, tweenParams.timeScaled, tweenParams.onFinishedStepEvent)
            {
                startValue = tweenParams.startValue;
                targetValue = tweenParams.targetValue;
                this.tweenEvent = tweenEvent;
            }

            protected override void InvokeEvent() => tweenEvent.Invoke(InterpolateValue());

            protected abstract T InterpolateValue();
        }

        // INSTANTIABLE
        private class FloatTweener : EventTweener<float>
        {
            private Material mat;
            private CanvasGroup group;
            private string property;

            public FloatTweener(float startValue, float targetValue, Action<float> tweenEvent, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
                base(startValue, targetValue, tweenEvent, animParams, delay, scaled, onFinishedStepEvent)
            {
            }

            public FloatTweener(Material material, in string property, float startValue, float targetValue, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
                base(startValue, targetValue, null, animParams, delay, scaled, onFinishedStepEvent)
            {
                mat = material;
                this.property = property;
            }

            public FloatTweener(CanvasGroup group, float startValue, float targetValue, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
                base(startValue, targetValue, tweenEvent: null, animParams, delay, scaled, onFinishedStepEvent)
            {
                this.group = group;
            }

            protected override void InvokeEvent()
            {
                if (mat != null)
                    mat.SetFloat(property, InterpolateValue());
                else if (group != null)
                    group.alpha = InterpolateValue();
                else
                    base.InvokeEvent();
            }

            protected override float InterpolateValue()
            {
                return MasterTween.InterpolateValue(startValue, targetValue, timer, animParams);
            }
        }

        private class Vector2Tweener : EventTweener<Vector2>
        {
            public Vector2Tweener(Vector2 startValue, Vector2 targetValue, Action<Vector2> tweenEvent, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
                base(startValue, targetValue, tweenEvent, animParams, delay, scaled, onFinishedStepEvent)
            {
            }

            protected override Vector2 InterpolateValue()
            {
                return MasterTween.InterpolateValue(startValue, targetValue, timer, animParams);
            }
        }

        private class Vector3Tweener : EventTweener<Vector3>
        {
            private Transform tweenedTransform;
            private ETransformProperty property;

            public Vector3Tweener(Vector3 startValue, Vector3 targetValue, Action<Vector3> tweenEvent, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
                base(startValue, targetValue, tweenEvent, animParams, delay, scaled, onFinishedStepEvent)
            {
            }

            public Vector3Tweener(Transform transform, ETransformProperty property, Vector3 startValue, Vector3 targetValue, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
                base(startValue, targetValue, null, animParams, delay, scaled, onFinishedStepEvent)
            {
                tweenedTransform = transform;
                this.property = property;
            }

            protected override void InvokeEvent()
            {
                if (tweenedTransform != null)
                {
                    switch (property)
                    {
                        case ETransformProperty.Position:
                            TweenPosition(tweenedTransform, InterpolateValue());
                            break;
                        case ETransformProperty.Scale:
                            TweenScale(tweenedTransform, InterpolateValue());
                            break;
                    }
                }
                else
                    base.InvokeEvent();
            }

            protected override Vector3 InterpolateValue()
            {
                return MasterTween.InterpolateValue(startValue, targetValue, timer, animParams);
            }
        }

        private class QuaternionTweener : EventTweener<Quaternion>
        {
            private Transform tweenedTransform;

            public QuaternionTweener(Quaternion startValue, Quaternion targetValue, Action<Quaternion> tweenEvent, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
                base(startValue, targetValue, tweenEvent, animParams, delay, scaled, onFinishedStepEvent)
            {
            }

            public QuaternionTweener(Transform transform, Quaternion startValue, Quaternion targetValue, AnimParams animParams, float delay, bool scaled, Action onFinishedStepEvent) :
                base(startValue, targetValue, null, animParams, delay, scaled, onFinishedStepEvent)
            {
                tweenedTransform = transform;
            }

            protected override void InvokeEvent()
            {
                if (tweenedTransform != null) TweenRotation(tweenedTransform, InterpolateValue());
                else base.InvokeEvent();
            }

            protected override Quaternion InterpolateValue()
            {
                return MasterTween.InterpolateValue(startValue, targetValue, timer, animParams);
            }
        }

        #endregion

        [Serializable]
        public struct TweeningParams<T>
        {
            public T startValue;
            public T targetValue;
            public AnimParams animParams;
            public float delay;
            public bool parrallel;
            public bool timeScaled;
            public Action onFinishedStepEvent;

            public TweeningParams(T startValue, T targetValue, AnimParams animParams, 
                float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
            {
                this.startValue = startValue;
                this.targetValue = targetValue;
                this.animParams = animParams;
                this.delay = delay;
                this.parrallel = parrallel;
                this.timeScaled = timeScaled;
                this.onFinishedStepEvent = onFinishedStepEvent;
            }
        }

        [Serializable]
        public struct AnimParams
        {
            [SerializeField] private float duration;
            [SerializeField] private ETransition transition;

            public readonly float Duration => duration;
            public readonly ETransition Transition => transition;

            public AnimParams(float duration, ETransition transition)
            {
                this.duration = duration;
                this.transition = transition;
            }
        }

        [Serializable]
        public struct InOutAnimParams
        {
            public AnimParams inAnim;
            public float holdDuration;
            public AnimParams outAnim;
        }

        public class Tween
        {
            private readonly MasterTween tween;

            private Guid id;

            public bool IsActive => tween.IsActive;

            public Tween(MasterTween tween, Guid id)
            {
                this.tween = tween;
                this.id = id;
            }

            #region Add Property

            public void TweenProperty(Transform transform, ETransformProperty property, Vector3 startValue, Vector3 targetValue, AnimParams animParams,
                float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
                => tween.TweenProperty(transform, property, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);

            public void TweenProperty(Transform transform, Quaternion startRota, Quaternion targetRota, AnimParams animParams,
                float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
                => tween.TweenProperty(transform, startRota, targetRota, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);

            public void TweenProperty(Material material, in string property, float startValue, float targetValue, AnimParams animParams,
                float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
                 => tween.TweenProperty(material, property, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);

            public void TweenProperty(CanvasGroup group, float startValue, float targetValue, AnimParams animParams,
                float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
                 => tween.TweenProperty(group, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);

            #endregion

            #region Add Event

            /// <summary>
            /// 
            /// </summary>
            /// <param name="startValue"></param>
            /// <param name="targetValue"></param>
            /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
            /// <param name="animParams"></param>
            /// <param name="delay"></param>
            /// <param name="parrallel"></param>
            /// <param name="timeScaled"></param>
            /// <param name="onFinishedStepEvent"></param>
            public void TweenEvent(float startValue, float targetValue, Action<float> tweenEvent, AnimParams animParams,
                float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
                => tween.TweenEvent(startValue, targetValue, tweenEvent, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="startValue"></param>
            /// <param name="targetValue"></param>
            /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
            /// <param name="animParams"></param>
            /// <param name="delay"></param>
            /// <param name="parrallel"></param>
            /// <param name="timeScaled"></param>
            /// <param name="onFinishedStepEvent"></param>
            public void TweenEvent(TweeningParams<float> tweenParams, Action<float> tweenEvent)
            {
                tween.TweenEvent(
                    tweenParams.startValue,
                    tweenParams.targetValue,
                    tweenEvent,
                    tweenParams.animParams,
                    tweenParams.delay,
                    tweenParams.parrallel,
                    tweenParams.timeScaled,
                    tweenParams.onFinishedStepEvent
                );
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="startValue"></param>
            /// <param name="targetValue"></param>
            /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
            /// <param name="animParams"></param>
            /// <param name="delay"></param>
            /// <param name="parrallel"></param>
            /// <param name="timeScaled"></param>
            /// <param name="onFinishedStepEvent"></param>
            public void TweenEvent(Vector2 startValue, Vector2 targetValue, Action<Vector2> tweenEvent, AnimParams animParams,
                float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
                => tween.TweenEvent(startValue, targetValue, tweenEvent, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="startValue"></param>
            /// <param name="targetValue"></param>
            /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
            /// <param name="animParams"></param>
            /// <param name="delay"></param>
            /// <param name="parrallel"></param>
            /// <param name="timeScaled"></param>
            /// <param name="onFinishedStepEvent"></param>
            public void TweenEvent(TweeningParams<Vector2> tweenParams, Action<Vector2> tweenEvent)
            {
                tween.TweenEvent(
                    tweenParams.startValue,
                    tweenParams.targetValue,
                    tweenEvent,
                    tweenParams.animParams,
                    tweenParams.delay,
                    tweenParams.parrallel,
                    tweenParams.timeScaled,
                    tweenParams.onFinishedStepEvent
                );
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="startValue"></param>
            /// <param name="targetValue"></param>
            /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
            /// <param name="animParams"></param>
            /// <param name="delay"></param>
            /// <param name="parrallel"></param>
            /// <param name="timeScaled"></param>
            /// <param name="onFinishedStepEvent"></param>
            public void TweenEvent(Vector3 startValue, Vector3 targetValue, Action<Vector3> tweenEvent, AnimParams animParams,
                float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
                => tween.TweenEvent(startValue, targetValue, tweenEvent, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="startValue"></param>
            /// <param name="targetValue"></param>
            /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
            /// <param name="animParams"></param>
            /// <param name="delay"></param>
            /// <param name="parrallel"></param>
            /// <param name="timeScaled"></param>
            /// <param name="onFinishedStepEvent"></param>
            public void TweenEvent(TweeningParams<Vector3> tweenParams, Action<Vector3> tweenEvent)
            {
                tween.TweenEvent(
                    tweenParams.startValue,
                    tweenParams.targetValue,
                    tweenEvent,
                    tweenParams.animParams,
                    tweenParams.delay,
                    tweenParams.parrallel,
                    tweenParams.timeScaled,
                    tweenParams.onFinishedStepEvent
                );
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="startValue"></param>
            /// <param name="targetValue"></param>
            /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
            /// <param name="animParams"></param>
            /// <param name="delay"></param>
            /// <param name="parrallel"></param>
            /// <param name="timeScaled"></param>
            /// <param name="onFinishedStepEvent"></param>
            public void TweenEvent(Quaternion startValue, Quaternion targetValue, Action<Quaternion> tweenEvent, AnimParams animParams,
                float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
                => tween.TweenEvent(startValue, targetValue, tweenEvent, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="startValue"></param>
            /// <param name="targetValue"></param>
            /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
            /// <param name="animParams"></param>
            /// <param name="delay"></param>
            /// <param name="parrallel"></param>
            /// <param name="timeScaled"></param>
            /// <param name="onFinishedStepEvent"></param>
            public void TweenEvent(TweeningParams<Quaternion> tweenParams, Action<Quaternion> tweenEvent)
            {
                tween.TweenEvent(
                    tweenParams.startValue,
                    tweenParams.targetValue,
                    tweenEvent,
                    tweenParams.animParams,
                    tweenParams.delay,
                    tweenParams.parrallel,
                    tweenParams.timeScaled,
                    tweenParams.onFinishedStepEvent
                );
            }

            #endregion

            public void SetFinishedEvent(Action onFinishedEvent) => tween.OnFinishedEvent += onFinishedEvent;

            public void Kill() => tween.Kill(id);
        }

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

        #endregion

        #region Static

        static MasterTween()
        {
            pool = new Pool<MasterTween>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Tween Create(in string name = "Tween")
        {
            pool.Take(out MasterTween tween);
            if (tween == null) tween = new GameObject().AddComponent(typeof(MasterTween)) as MasterTween;
            tween.name = name;

            Guid id = Guid.NewGuid();
            tween.id = id;

            return new Tween(tween, id);
        }

        private readonly static Pool<MasterTween> pool;

        #region Interpolation Methods

        // FLOATS
        public static float InterpolateValue(float startValue, float targetValue, float percentage, ETransition transition)
        {
            percentage = Mathf.Clamp01(percentage);
            float result = GetValue(percentage, transition);
            return Mathf.LerpUnclamped(startValue, targetValue, result);
        }

        public static float InterpolateValue(float startValue, float targetValue, float elapsedTime, AnimParams animParams)
            => InterpolateValue(startValue, targetValue, animParams.Duration, elapsedTime, animParams.Transition);

        public static float InterpolateValue(float startValue, float targetValue, float duration, float elapsedTime, ETransition transition = ETransition.Linear)
            => InterpolateValue(startValue, targetValue, elapsedTime / duration, transition);

        // VECTOR 2
        public static Vector2 InterpolateValue(Vector2 startValue, Vector2 targetValue, float percentage, ETransition transition)
        {
            percentage = Mathf.Clamp01(percentage);
            float result = GetValue(percentage, transition);
            return Vector2.LerpUnclamped(startValue, targetValue, result);
        }

        public static Vector2 InterpolateValue(Vector2 startValue, Vector2 targetValue, float elapsedTime, AnimParams animParams)
            => InterpolateValue(startValue, targetValue, animParams.Duration, elapsedTime, animParams.Transition);

        public static Vector2 InterpolateValue(Vector2 startValue, Vector2 targetValue, float duration, float elapsedTime, ETransition transition = ETransition.Linear)
            => InterpolateValue(startValue, targetValue, elapsedTime / duration, transition);

        // VECTOR 3
        public static Vector3 InterpolateValue(Vector3 startValue, Vector3 targetValue, float percentage, ETransition transition)
        {
            percentage = Mathf.Clamp01(percentage);
            float result = GetValue(percentage, transition);
            return Vector3.LerpUnclamped(startValue, targetValue, result);
        }

        public static Vector3 InterpolateValue(Vector3 startValue, Vector3 targetValue, float elapsedTime, AnimParams animParams)
            => InterpolateValue(startValue, targetValue, animParams.Duration, elapsedTime, animParams.Transition);

        public static Vector3 InterpolateValue(Vector3 startValue, Vector3 targetValue, float duration, float elapsedTime, ETransition transition = ETransition.Linear)
            => InterpolateValue(startValue, targetValue, elapsedTime / duration, transition);

        // QUATERNIONS
        public static Quaternion InterpolateValue(Quaternion startValue, Quaternion targetValue, float percentage, ETransition transition)
        {
            percentage = Mathf.Clamp01(percentage);
            float result = GetValue(percentage, transition);
            return Quaternion.LerpUnclamped(startValue, targetValue, result);
        }

        public static Quaternion InterpolateValue(Quaternion startValue, Quaternion targetValue, float elapsedTime, AnimParams animParams)
            => InterpolateValue(startValue, targetValue, animParams.Duration, elapsedTime, animParams.Transition);

        public static Quaternion InterpolateValue(Quaternion startValue, Quaternion targetValue, float duration, float elapsedTime, ETransition transition = ETransition.Linear)
            => InterpolateValue(startValue, targetValue, elapsedTime / duration, transition);

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="easing"></param>
        /// <param name="transition"></param>
        /// <returns>A value between 0 & 1 which corresponds to the given equation</returns>
        private static float GetValue(float t, ETransition transition)
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

        // Tween Function
        private static void TweenPosition(Transform transform, Vector3 newPos) => transform.position = newPos;
        private static void TweenRotation(Transform transform, Quaternion newRota) => transform.rotation = newRota;
        private static void TweenScale(Transform transform, Vector3 newScale) => transform.localScale = newScale;

        #endregion

        #region Instance

        private MasterTween() { }

        #region Variables

        private readonly List<List<Tweener>> tweeners = new List<List<Tweener>>();

        public event Action OnFinishedEvent;

        public bool IsActive { get; private set; }

        private Guid id;

        #endregion

        private void Update()
        {
            if (IsActive)
            {
                if (tweeners.Count > 0)
                {
                    List<Tweener> currentTweeners = tweeners[0];

                    for (int i = currentTweeners.Count - 1; i >= 0; i--)
                    {
                        if (currentTweeners[i].UpdateTweener())
                            currentTweeners.RemoveAt(i);
                    }

                    if (currentTweeners.Count == 0) tweeners.RemoveAt(0);
                }
                else
                {
                    if (OnFinishedEvent?.Target != null)
                        OnFinishedEvent.Invoke();

                    Kill(id);
                }
            }
        }

        #region Add Property

        private void TweenProperty(Transform transform, ETransformProperty property, Vector3 startValue, Vector3 targetValue, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new Vector3Tweener(transform, property, startValue, targetValue, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        private void TweenProperty(Transform transform, Quaternion startRota, Quaternion targetRota, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new QuaternionTweener(transform, startRota, targetRota, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        private void TweenProperty(Material material, in string property, float startValue, float targetValue, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new FloatTweener(material, property, startValue, targetValue, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        private void TweenProperty(CanvasGroup group, float startValue, float targetValue, AnimParams animParams, float delay, bool parrallel, bool timeScaled, Action onFinishedStepEvent)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new FloatTweener(group, startValue, targetValue, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        #endregion

        #region Add Event

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="targetValue"></param>
        /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
        /// <param name="animParams"></param>
        /// <param name="delay"></param>
        /// <param name="parrallel"></param>
        /// <param name="timeScaled"></param>
        /// <param name="onFinishedStepEvent"></param>
        private void TweenEvent(float startValue, float targetValue, Action<float> tweenEvent, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new FloatTweener(startValue, targetValue, tweenEvent, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="targetValue"></param>
        /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
        /// <param name="animParams"></param>
        /// <param name="delay"></param>
        /// <param name="parrallel"></param>
        /// <param name="timeScaled"></param>
        /// <param name="onFinishedStepEvent"></param>
        private void TweenEvent(Vector2 startValue, Vector2 targetValue, Action<Vector2> tweenEvent, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new Vector2Tweener(startValue, targetValue, tweenEvent, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="targetValue"></param>
        /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
        /// <param name="animParams"></param>
        /// <param name="delay"></param>
        /// <param name="parrallel"></param>
        /// <param name="timeScaled"></param>
        /// <param name="onFinishedStepEvent"></param>
        private void TweenEvent(Vector3 startValue, Vector3 targetValue, Action<Vector3> tweenEvent, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new Vector3Tweener(startValue, targetValue, tweenEvent, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="targetValue"></param>
        /// <param name="tweenEvent">Will call a function. In general, what you have to do in this function is just to apply the given result to whatever variable you want.</param>
        /// <param name="animParams"></param>
        /// <param name="delay"></param>
        /// <param name="parrallel"></param>
        /// <param name="timeScaled"></param>
        /// <param name="onFinishedStepEvent"></param>
        private void TweenEvent(Quaternion startValue, Quaternion targetValue, Action<Quaternion> tweenEvent, AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            SetupTweener(out List<Tweener> tweenerList, ref delay, parrallel);
            tweenerList.Add(new QuaternionTweener(startValue, targetValue, tweenEvent, animParams, delay, timeScaled, onFinishedStepEvent));
        }

        private void SetupTweener(out List<Tweener> tweenerList, ref float delay, bool parrallel = false)
        {
            IsActive = true;

            if (!parrallel || tweeners.Count == 0)
            {
                tweenerList = new List<Tweener>();
                tweeners.Add(tweenerList);
            }
            else
                tweenerList = tweeners[tweeners.Count - 1];

            if (parrallel && tweenerList.Count >= 1)
                delay += tweenerList[0].Delay;
        }

        #endregion

        private void Kill(in Guid id)
        {
            if (!IsActive || id != this.id) return;

            OnFinishedEvent = null;
            tweeners.Clear();
            pool.MoveIn(this);
        }

        private void OnDisable()
        {
            IsActive = false;
        }

        private void OnDestroy()
        {
            pool.Remove(this);
        }

        #endregion
    }

    public static class TweenClassExtension
    {
        #region Transform

        // POSITION
        public static MasterTween.Tween TweenPosition(this Transform transform, in Vector3 startPos, in Vector3 targetPos, MasterTween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, MasterTween.Tween tween = null)
        {
            if (tween == null) tween = MasterTween.Create();
            tween.TweenProperty(transform, ETransformProperty.Position, startPos, targetPos, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static MasterTween.Tween TweenPosition(this Transform transform, in Vector3 targetPos, MasterTween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, MasterTween.Tween tween = null)
        {
            if (tween == null) tween = MasterTween.Create();
            tween.TweenProperty(transform, ETransformProperty.Position, transform.position, targetPos, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        // ROTATION
        public static MasterTween.Tween TweenRotation(this Transform transform, in Quaternion startRota, in Quaternion targetRota, MasterTween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, MasterTween.Tween tween = null)
        {
            if (tween == null) tween = MasterTween.Create();
            tween.TweenProperty(transform, startRota, targetRota, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static MasterTween.Tween TweenRotation(this Transform transform, in Quaternion targetRota, MasterTween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, MasterTween.Tween tween = null)
        {
            if (tween == null) tween = MasterTween.Create();
            tween.TweenProperty(transform, transform.rotation, targetRota, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        // SCALE
        public static MasterTween.Tween TweenScale(this Transform transform, in Vector3 startScale, in Vector3 targetScale, MasterTween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, MasterTween.Tween tween = null)
        {
            if (tween == null) tween = MasterTween.Create();
            tween.TweenProperty(transform, ETransformProperty.Scale, startScale, targetScale, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static MasterTween.Tween TweenScale(this Transform transform, in Vector3 targetScale, MasterTween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, MasterTween.Tween tween = null)
        {
            if (tween == null) tween = MasterTween.Create();
            tween.TweenProperty(transform, ETransformProperty.Scale, transform.localScale, targetScale, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        #endregion

        #region Material

        public static MasterTween.Tween TweenFloatProperty(this Material mat, in string property, in float startValue, in float targetValue, MasterTween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, MasterTween.Tween tween = null)
        {
            if (tween == null) tween = MasterTween.Create();
            tween.TweenProperty(mat, property, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static MasterTween.Tween TweenFloatProperty(this Material mat, in string property, in float targetValue, MasterTween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, MasterTween.Tween tween = null)
        {
            if (tween == null) tween = MasterTween.Create();
            tween.TweenProperty(mat, property, mat.GetFloat(property), targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        #endregion

        public static MasterTween.Tween TweenAlpha(this CanvasGroup group, in float targetValue, MasterTween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, MasterTween.Tween tween = null)
        {
            tween ??= MasterTween.Create();
            tween.TweenProperty(group, group.alpha, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static MasterTween.Tween TweenAlpha(this CanvasGroup group, in float startValue, in float targetValue, MasterTween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null, MasterTween.Tween tween = null)
        {
            tween ??= MasterTween.Create();
            tween.TweenProperty(group, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

    }
}
