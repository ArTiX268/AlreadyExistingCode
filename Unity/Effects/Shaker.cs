using ArTiX.Utils;
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
            public Vector3 current;
            public Vector3 next;
        }

        #region Variables

        [SerializeField] private Transform[] targets;

        [Header("Parameters")]
        [SerializeField] private Vector2 amplitude = Vector2.one * 5f;
        [SerializeField, Range(0.0001f, 0.1f)] private float step = 0.048f;
        [SerializeField, Range(0, 360), Tooltip("It's an angle")] private float minNoise = 15f;
        [SerializeField, Range(0, 360), Tooltip("It's an angle")] private float maxNoise = 15f;
        [SerializeField] private bool influencedByTimeScale;

        [Header("Attack")]
        [SerializeField, Tooltip("The time it takes to reach maximum intensity. Is before duration.")] private float attackDuration = 0.25f;
        [SerializeField] private Tween.ETransition attackTrans = Tween.ETransition.Sine;
        [SerializeField] private Tween.EEasing attackEase = Tween.EEasing.InOut;

        [Header("Shaking")]
        [SerializeField] private float shakeDuration = 2f;
        [SerializeField] private Tween.ETransition shakeTransition = Tween.ETransition.Quad;
        [SerializeField] private Tween.EEasing shakeEasing = Tween.EEasing.InOut;

        [Header("Release")]
        [SerializeField, Tooltip("The time it takes to stop shaking. Is after duration.")] private float releaseDuration = 0.25f;
        [SerializeField] private Tween.ETransition releaseTransition = Tween.ETransition.Sine;
        [SerializeField] private Tween.EEasing releaseEase = Tween.EEasing.InOut;

        private List<Target> targetsDatas = new List<Target>();

        private float intensity;

        private float elapsedTime = 0;
        private float stepElapsedTime = 0;

        private delegate void DoShaker(float deltaTime);
        private DoShaker doShake;
        /// <summary>
        /// Setting this resets the elapsed time.
        /// </summary>
        private DoShaker DoShake
        {
            get => doShake;
            set
            {
                doShake = value;
                elapsedTime = 0;
            }
        }

        #endregion

        private void Start()
        {
            List<Transform> newTargets = new List<Transform>();
            foreach (Transform target in targets)
            {
                if (target != null) newTargets.Add(target);
            }
            targets = newTargets.ToArray();
        }

        private void Update()
        {
            if (doShake != null)
            {
                float delta = influencedByTimeScale ? Time.deltaTime : Time.unscaledDeltaTime;
                doShake.Invoke(delta);

                stepElapsedTime = Mathf.Clamp(stepElapsedTime + delta, 0, step);
                int nbTarget = targetsDatas.Count;
                Vector2 newPos;

                for (int i = 0; i < nbTarget; i++)
                {
                    newPos = Tween.InterpolateValue(targetsDatas[i].current, targetsDatas[i].next, step, stepElapsedTime, shakeEasing, shakeTransition);
                    targetsDatas[i].target.localPosition = new Vector3(newPos.x, newPos.y, targetsDatas[i].target.localPosition.z);

                    if (stepElapsedTime >= step)
                    {
                        stepElapsedTime = 0;
                        targetsDatas[i].current = targetsDatas[i].next;
                        targetsDatas[i].next = FindNewPos(targetsDatas[i].origin, targetsDatas[i].current);
                    }
                }
            }
        }

        public void StartShaker()
        {
            if (targets == null || targets.Length == 0) return;

            Stop();

            amplitude = amplitude.Abs();

            targetsDatas.Clear();
            intensity = 0f;

            foreach (Transform target in targets)
            {
                targetsDatas.Add(new Target
                {
                    target = target,
                    origin = target.localPosition,
                    current = target.localPosition,
                    next = FindNewPos(target.localPosition, target.localPosition + Vector3.right)
                });
            }

            DoShake = Attack;
        }

        #region State Machine

        private void Attack(float delta)
        {
            IncrementTimer(attackDuration, delta);
            intensity = Tween.InterpolateValue(0, 1, attackDuration, elapsedTime, attackEase, attackTrans);

            if (elapsedTime >= attackDuration)
                DoShake = Shaking;
        }

        private void Shaking(float delta)
        {
            IncrementTimer(shakeDuration, delta);
            if (elapsedTime >= attackDuration)
                DoShake = Release;
        }

        private void Release(float delta)
        {
            IncrementTimer(releaseDuration, delta);
            intensity = Tween.InterpolateValue(1, 0, releaseDuration, elapsedTime, releaseEase, releaseTransition);

            if (elapsedTime >= releaseDuration)
                Stop();
        }

        #endregion

        public void Stop()
        {
            if (targets == null) return;

            foreach (Target target in targetsDatas)
                target.target.localPosition = target.origin;

            DoShake = null;
            stepElapsedTime = 0f;
        }

        public bool IsPlaying() => doShake != null;

        private void IncrementTimer(in float duration, in float delta) => elapsedTime = Mathf.Clamp(elapsedTime + delta, 0, duration);

        private Vector2 FindNewPos(in Vector3 origin, in Vector3 current)
        {
            float noise = Random.Range(-maxNoise, maxNoise);
            if (noise > -minNoise && noise < minNoise) // Noise too small
                noise = noise > 0 ? minNoise : -minNoise;

            float angle = Vector2.Angle(Vector2.right, new Vector2(current.x - origin.x, current.y - origin.y).normalized) + noise;
            Vector2 next = Utilities.VectorFromAngle(angle);

            next.x *= amplitude.x;
            next.y *= amplitude.y;

            return (Vector2)origin + next;
        }
    }
}
