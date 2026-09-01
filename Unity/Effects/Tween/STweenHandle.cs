using System;
using UnityEngine;

namespace ArTiX.Effects.Tween
{
    public readonly struct STweenHandle
    {
        private readonly Tween tween;
        private readonly int version;

        public STweenHandle(in Tween tween)
        {
            this.tween = tween;
            version = tween == null ? -1 : tween.Version;
        }

        public bool IsAlive => tween != null && tween.Version == version;

        public void Kill()
        {
            if (IsAlive) tween.Kill();
        }

        #region Add Property

        public STweenHandle TweenProperty(UnityEngine.Object target, Transform transform, ETransformProperty property, Vector3 startValue, Vector3 targetValue, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (IsAlive) tween.TweenProperty(target, transform, property, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return this;
        }

        public STweenHandle TweenProperty(UnityEngine.Object target, Transform transform, Quaternion startRota, Quaternion targetRota, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (IsAlive) tween.TweenProperty(target, transform, startRota, targetRota, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return this;
        }

        public STweenHandle TweenProperty(UnityEngine.Object target, Material material, in string property, float startValue, float targetValue, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (IsAlive) tween.TweenProperty(target, material, property, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return this;
        }

        public STweenHandle TweenProperty(UnityEngine.Object target, Material material, in string property, Color startColor, Color targetColor, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (IsAlive) tween.TweenProperty(target, material, property, startColor, targetColor, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return this;
        }

        public STweenHandle TweenProperty(UnityEngine.Object target, CanvasGroup group, float startValue, float targetValue, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (IsAlive) tween.TweenProperty(target, group, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return this;
        }

        #endregion

        #region Add Event

        public STweenHandle TweenEvent(UnityEngine.Object target, float startValue, float targetValue, Action<float> tweenEvent, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (IsAlive) tween.TweenEvent(target, startValue, targetValue, tweenEvent, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return this;
        }

        public STweenHandle TweenEvent(UnityEngine.Object target, Vector2 startValue, Vector2 targetValue, Action<Vector2> tweenEvent, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (IsAlive) tween.TweenEvent(target, startValue, targetValue, tweenEvent, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return this;
        }

        public STweenHandle TweenEvent(UnityEngine.Object target, Vector3 startValue, Vector3 targetValue, Action<Vector3> tweenEvent, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (IsAlive) tween.TweenEvent(target, startValue, targetValue, tweenEvent, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return this;
        }

        public STweenHandle TweenEvent(UnityEngine.Object target, Quaternion startValue, Quaternion targetValue, Action<Quaternion> tweenEvent, Tween.AnimParams animParams,
            float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (IsAlive) tween.TweenEvent(target, startValue, targetValue, tweenEvent, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return this;
        }

        #endregion
    }
}
