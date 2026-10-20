using ArTiX.Utils;
using Noise;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Effects
{
    public class Shaker : MonoBehaviour
    {
        private struct TargetData
        {
            public Transform target;
            public Vector3 origin;
            public float originAngle;
        }

        #region Variables

        [SerializeField] private List<Transform> targets;

        [Header("Parameters")]
        [SerializeField] private Vector2 amplitude = Vector2.one * 5f;
        [SerializeField, Tooltip("To shake the angle")] private float maxAngle = 0;
        [SerializeField] private float jitteriness = 1;
        [SerializeField] private bool timeScaled = true;

        [Header("Anim")]
        [SerializeField, Tooltip("Anim to reach max intensity.")] private Tween.Tween.AnimParams attackParams;
        [SerializeField, Tooltip("How much time the shake will be active at full intensity.")] private float holdDuration;
        [SerializeField, Tooltip("Anim to reach 0 intensity.")] private Tween.Tween.AnimParams releaseParams;

        private readonly List<TargetData> targetsDatas = new List<TargetData>();

        private float intensityCoef = 1;

        private float intensity;

        private Vector2 Amplitude => amplitude * (intensity * intensityCoef);
        private float Angle => maxAngle * intensity * intensityCoef;
        public bool IsPlaying => intensity > 0;

        private Tween.STweenHandle intensityTween;
        private Tween.STweenHandle intensityCoefTween;

        private Vector2 currentOffset = Vector2.zero;

        #endregion

        private void Start()
        {
            List<Transform> newTargets = new List<Transform>();
            foreach (Transform target in targets)
            {
                if (target != null) newTargets.Add(target);
            }
            targets = newTargets;

            amplitude = amplitude.Abs();
        }

        private void Update()
        {
            if (intensity > 0 && targetsDatas.Count > 0)
            {
                int nbTarget = targetsDatas.Count;
                FastNoise xNoise;
                FastNoise yNoise;
                FastNoise angleNoise;

                float offsetX;
                float offsetY;
                float angleOffset;

                float time = timeScaled ? Time.time : Time.unscaledTime;
                TargetData targetDatas;
                for (int i = 0; i < nbTarget; i++)
                {
                    xNoise = new FastNoise(i);
                    yNoise = new FastNoise(i + 1);
                    angleNoise = new FastNoise(i + 2);

                    xNoise.SetFrequency(jitteriness);
                    yNoise.SetFrequency(jitteriness);
                    angleNoise.SetFrequency(jitteriness);

                    offsetX = Amplitude.x * xNoise.GetPerlin(time, 0);
                    offsetY = Amplitude.y * yNoise.GetPerlin(time, 0);
                    angleOffset = Angle * angleNoise.GetPerlin(time, 0);

                    offsetX += currentOffset.x;
                    offsetY += currentOffset.y;

                    targetDatas = targetsDatas[i];
                    targetDatas.target.SetLocalPositionAndRotation(
                        localPosition: targetDatas.origin + new Vector3(offsetX, offsetY, 0),
                        localRotation: Quaternion.Euler(0, 0, targetDatas.originAngle + angleOffset)
                    );
                }
            }
        }

        public void Shake()
        {
            if (targets?.Count == 0) return;

            Stop();

            targetsDatas.Clear();
            intensity = 0f;

            intensityTween.Kill();

            intensityTween = Tween.Tween.Create("Intensity Tween")
                .TweenEvent(this,
                    startValue: 0,
                    targetValue: 1,
                    tweenEvent: TweenIntensity,
                    animParams: attackParams)
                .TweenEvent(this,
                    startValue: 1,
                    targetValue: 0,
                    tweenEvent: TweenIntensity,
                    animParams: releaseParams,
                    delay: holdDuration);

            for (int i = targets.Count - 1; i >= 0; i--)
            {
                if (targets[i] == null)
                {
                    targets.RemoveAt(i);
                    continue;
                }

                targetsDatas.Add(new TargetData
                {
                    target = targets[i],
                    origin = targets[i].localPosition,
                    originAngle = targets[i].localRotation.eulerAngles.z
                });
            }
        }

        public void Stop()
        {
            if (targets == null) return;

            for (int i = targetsDatas.Count - 1; i >= 0; i--)
            {
                if (targetsDatas[i].target == null)
                {
                    targetsDatas.RemoveAt(i);
                    continue;
                }
                targetsDatas[i].target.SetLocalPositionAndRotation(
                    localPosition: targetsDatas[i].origin,
                    localRotation: Quaternion.Euler(0, 0, targetsDatas[i].originAngle));
            }

            intensityTween.Kill();
        }

        public void Push(in Vector2 offset, in float duration)
        {
            if (duration <= 0) return;

            Tween.Tween.Create().TweenEvent(this, Vector2.zero, offset, TweenOffset, animParams: new Tween.Tween.AnimParams
            (duration, Tween.Tween.ETransition.SmoothStopArch3));
        }

        private void TweenOffset(Vector2 offset) => currentOffset = offset;

        private void TweenIntensity(float newIntensity) => intensity = newIntensity;

        public void ChangeIntensityCoef(float targetValue,
            float duration = 0, Tween.Tween.ETransition transition = Tween.Tween.ETransition.Linear)
        {
            if (duration <= 0) return;
            if (duration == 0) intensityCoef = targetValue;

            intensityCoefTween.Kill();
            intensityCoefTween = Tween.Tween.Create("IntensityCoef Tween")
                .TweenEvent(this,
                    startValue: intensityCoef,
                    targetValue: targetValue,
                    tweenEvent: TweenIntensityCoef,
                    new Tween.Tween.AnimParams(duration, transition)
                );
        }

        private void TweenIntensityCoef(float newIntensityCoef) => intensityCoef = newIntensityCoef;
    }
}
