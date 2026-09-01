using ArTiX.Utils;
using Noise;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Effects
{
    public class Shaker : MonoBehaviour
    {
        private class Target
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
        [SerializeField, Range(0, 1)] public float intensityCoef = 1;
        [SerializeField] private float jitteriness = 1;
        [SerializeField] private bool timeScaled;

        [Header("Anim")]
        [SerializeField, Tooltip("Anim to reach max intensity.")] private Tween.AnimParams attackParams;
        [SerializeField, Tooltip("How much time the shake will be active at full intensity.")] private float holdDuration;
        [SerializeField, Tooltip("Anim to reach 0 intensity.")] private Tween.AnimParams releaseParams;

        private readonly List<Target> targetsDatas = new List<Target>();

        private float intensity;

        private Vector2 Amplitude => amplitude * (intensity * intensityCoef);
        private float Angle => maxAngle * intensity * intensityCoef;

        private Tween intensityTween;

        #endregion

        private void Start()
        {
            List<Transform> newTargets = new List<Transform>();
            foreach (Transform target in targets)
            {
                if (target != null) newTargets.Add(target);
            }
            targets = newTargets;
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
                Target targetDatas;
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
            if (targets == null || targets.Count == 0) return;

            Stop();

            amplitude = amplitude.Abs();

            targetsDatas.Clear();
            intensity = 0f;

            intensityTween = Tween.Create();
            intensityTween.TweenEvent(0, 1, ModifyIntensity, attackParams);
            intensityTween.TweenEvent(1, 0, ModifyIntensity, releaseParams, delay: holdDuration);

            for (int i = targets.Count - 1; i >= 0; i--)
            {
                if (targets[i] == null)
                {
                    targets.RemoveAt(i);
                    continue;
                }

                targetsDatas.Add(new Target
                {
                    target = targets[i],
                    origin = targets[i].localPosition,
                    originAngle = targets[i].eulerAngles.z
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
                targetsDatas[i].target.localPosition = targetsDatas[i].origin;
            }

            intensityTween?.Kill();
            intensityTween = null;
        }

        public bool IsPlaying() => intensity > 0;

        private void ModifyIntensity(float newIntensity) => intensity = newIntensity;
    }
}
