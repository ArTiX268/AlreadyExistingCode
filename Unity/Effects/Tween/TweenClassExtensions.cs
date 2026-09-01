using System;
using UnityEngine;

namespace ArTiX.Effects.Tween
{
    public static class TweenClassExtension
    {
        #region Transform

        // POSITION
        public static STweenHandle TweenPosition(this Transform transform, in Vector3 startPos, in Vector3 targetPos, Tween.AnimParams animParams,
            ref STweenHandle tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (!tween.IsAlive) tween = Tween.Create();
            tween.TweenProperty(transform, transform, ETransformProperty.Position, startPos, targetPos, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static STweenHandle TweenPosition(this Transform transform, in Vector3 targetPos, Tween.AnimParams animParams,
            ref STweenHandle tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (!tween.IsAlive) tween = Tween.Create();
            tween.TweenProperty(transform, transform, ETransformProperty.Position, transform.position, targetPos, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        // ROTATION
        public static STweenHandle TweenRotation(this Transform transform, in Quaternion startRota, in Quaternion targetRota, Tween.AnimParams animParams,
            ref STweenHandle tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (!tween.IsAlive) tween = Tween.Create();
            tween.TweenProperty(transform, transform, startRota, targetRota, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static STweenHandle TweenRotation(this Transform transform, in Quaternion targetRota, Tween.AnimParams animParams,
            ref STweenHandle tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (!tween.IsAlive) tween = Tween.Create();
            tween.TweenProperty(transform, transform, transform.rotation, targetRota, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        // SCALE
        public static STweenHandle TweenScale(this Transform transform, in Vector3 startScale, in Vector3 targetScale, Tween.AnimParams animParams,
            ref STweenHandle tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (!tween.IsAlive) tween = Tween.Create();
            tween.TweenProperty(transform, transform, ETransformProperty.Scale, startScale, targetScale, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static STweenHandle TweenScale(this Transform transform, in Vector3 targetScale, Tween.AnimParams animParams,
            ref STweenHandle tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (!tween.IsAlive) tween = Tween.Create();
            tween.TweenProperty(transform, transform, ETransformProperty.Scale, transform.localScale, targetScale, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        #endregion

        #region Material

        public static STweenHandle TweenFloatProperty(this Material mat, UnityEngine.Object target, in string property, in float startValue, in float targetValue, Tween.AnimParams animParams,
            ref STweenHandle tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (!tween.IsAlive) tween = Tween.Create();
            tween.TweenProperty(target, mat, property, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static STweenHandle TweenFloatProperty(this Material mat, UnityEngine.Object target, in string property, in float targetValue, Tween.AnimParams animParams,
            ref STweenHandle tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (!tween.IsAlive) tween = Tween.Create();
            tween.TweenProperty(target, mat, property, mat.GetFloat(property), targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static STweenHandle TweenColorProperty(this Material mat, UnityEngine.Object target, in string property, in Color startColor, in Color targetColor, Tween.AnimParams animParams,
            ref STweenHandle tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (!tween.IsAlive) tween = Tween.Create();
            tween.TweenProperty(target, mat, property, startColor, targetColor, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static STweenHandle TweenColorProperty(this Material mat, UnityEngine.Object target, in string property, in Color targetColor, Tween.AnimParams animParams,
            ref STweenHandle tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (!tween.IsAlive) tween = Tween.Create();
            tween.TweenProperty(target, mat, property, mat.GetColor(property), targetColor, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        #endregion

        public static STweenHandle TweenAlpha(this CanvasGroup group, UnityEngine.Object target, in float targetValue, Tween.AnimParams animParams,
            ref STweenHandle tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (!tween.IsAlive) tween = Tween.Create();
            tween.TweenProperty(target, group, group.alpha, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

        public static STweenHandle TweenAlpha(this CanvasGroup group, UnityEngine.Object target, in float startValue, in float targetValue, Tween.AnimParams animParams,
            ref STweenHandle tween, float delay = 0, bool parrallel = false, bool timeScaled = false, Action onFinishedStepEvent = null)
        {
            if (!tween.IsAlive) tween = Tween.Create();
            tween.TweenProperty(target, group, startValue, targetValue, animParams, delay, parrallel, timeScaled, onFinishedStepEvent);
            return tween;
        }

    }
}
