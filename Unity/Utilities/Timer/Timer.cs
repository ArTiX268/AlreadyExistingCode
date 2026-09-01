using ArTiX.Utils.Pool;
using System;
using UnityEditor;
using UnityEngine;

namespace ArTiX.Utils
{
    public class Timer : MonoBehaviour, IPooledObject<Timer>
    {
        private static Pool<Timer> pool;
        private const int DEFAULT_POOL_SIZE = 10;
        private const int MAX_POOL_SIZE = DEFAULT_POOL_SIZE * 2;
        private const string PATH_TIMER_PREFAB = "Assets/_PROJECT/Shared/Utilities/Timer/prfb_Timer.prefab";

        static Timer()
        {
            Application.quitting += KillPool;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="startOnCreate"></param>
        /// <param name="autodestroy"></param>
        /// <param name="scaled"></param>
        /// <param name="repetitionNumber"></param>
        /// <param name="onTimerActiveEvent">Called every frame while the timer is active.</param>
        /// <param name="onFinishedEvent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static STimerHandle Create(MonoBehaviour target, float duration, bool startOnCreate = false, bool autodestroy = false, bool scaled = true, int repetitionNumber = 0, in bool infinite = false, Action onTimerActiveEvent = null, Action onFinishedEvent = null, string name = "Timer")
        {
            pool ??= new Pool<Timer>(Create, DEFAULT_POOL_SIZE, MAX_POOL_SIZE);
            Timer timer = pool.GetPooledObject();
            if (timer == null) return default;

            timer.gameObject.name = name;
            timer.duration = duration;
            timer.scaled = scaled;
            timer.autodestroy = autodestroy;
            timer.infinite = infinite;
            timer.repetitionNumber = timer.currentRepetitionNumber = repetitionNumber;
            timer.OnActiveEvent = onTimerActiveEvent;
            timer.OnFinishedEvent = onFinishedEvent;
            timer.IsActive = startOnCreate;
            timer.target = target;

            return new STimerHandle(timer);
        }

        private static Timer Create()
        {
            return Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(PATH_TIMER_PREFAB)).GetComponent<Timer>();
        }

        private static void KillPool() => pool = null;

        public bool IsActive { get; private set; }
        public float ElapsedTime { get; private set; }
        public float ElapsedTimeAsPercentage => ElapsedTime / duration;
        public float RemainingTime => duration - ElapsedTime;

        public Pool<Timer> Pool { get; set; }

        /// <summary>
        /// Incremented every time this timer returns to the pool, which invalidates every
        /// <see cref="STimerHandle"/> still pointing at the lease that just ended.
        /// </summary>
        public int Version { get; private set; }

        public float duration;

        private MonoBehaviour target;

        private bool scaled;
        private bool autodestroy;
        private bool infinite;
        private int repetitionNumber = 0;

        private int currentRepetitionNumber = 0;

        /// <summary>
        /// Called every frame while the timer is active.
        /// </summary>
        public event Action OnActiveEvent;
        public event Action OnFinishedEvent;

        private Timer() { }

        private void Update()
        {
            if (target == null)
            {
                Kill();
                return;
            }

            if (IsActive && currentRepetitionNumber >= 0)
            {
                ElapsedTime += scaled ? Time.deltaTime : Time.unscaledDeltaTime;

                OnActiveEvent?.Invoke();

                if (ElapsedTime > duration)
                {
                    if (!infinite) currentRepetitionNumber--;

                    ElapsedTime = 0;

                    if (currentRepetitionNumber == -1)
                    {
                        if (autodestroy) Kill();
                        else Stop();
                    }

                    OnFinishedEvent?.Invoke();
                }
            }
        }

        public void Play()
        {
            Stop();
            IsActive = true;
        }

        public void Stop()
        {
            currentRepetitionNumber = repetitionNumber;
            IsActive = false;
            ElapsedTime = 0;
        }

        /// <summary>
        /// Ends the current lease and returns the timer to the pool. Called either by <see cref="Update"/> when
        /// the target is gone or the timer autodestroys, or by a <see cref="STimerHandle"/> that still owns it.
        /// </summary>
        public void Kill()
        {
            Stop();
            Release();
        }

        public void Release()
        {
            // Every route back into the pool ends the lease, so the version bump belongs here rather than in
            // Kill(): a direct Release() would otherwise recycle the timer while handles still thought it theirs.
            Version++;
            Pool.ReturnToPool(this);
        }
    }
}