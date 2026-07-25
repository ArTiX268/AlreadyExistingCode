using System;
using UnityEngine;

namespace ArTiX.Utils
{
    public class MasterTimer : MonoBehaviour
    {
        public class Timer
        {
            private MasterTimer timer;

            private Guid id;

            public Timer(MasterTimer timer, Guid id)
            {
                this.timer = timer;
                this.id = id;

                timer.OnFinishedEvent += OnTimerFinished;
                timer.OnActiveEvent += OnTimerActive;
            }

            public void Start() => timer.Play();
            public void Pause() => timer.IsActive = false;
            public void Resume() => timer.IsActive = true;
            public void Stop() => timer.Stop();
            public void Kill() => timer.Kill(id);

            public float Duration
            {
                get => timer.duration;
                set => timer.duration = value;
            }
            public bool Scaled
            {
                get => timer.scaled;
                set => timer.scaled = value;
            }
            public bool Autodestroy
            {
                get => timer.autodestroy;
                set => timer.autodestroy = value;
            }
            public bool Infinite
            {
                get => timer.infinite;
                set => timer.infinite = value;
            }
            public int RepetitionNumber
            {
                get => timer.repetitionNumber;
                set => timer.repetitionNumber = value;
            }

            public float ElapsedTime => timer.ElapsedTime;
            public float ElapsedTimeAsPercentage => timer.ElapsedTime / timer.duration;
            public bool IsActive => timer.IsActive;

            public event Action OnFinishedEvent;
            public event Action OnActiveEvent;

            private void OnTimerFinished() => OnFinishedEvent?.Invoke();
            private void OnTimerActive() => OnActiveEvent?.Invoke();
        }

        private static readonly Pool<MasterTimer> pool = new Pool<MasterTimer>();

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
        public static Timer Create(in MonoBehaviour target, float duration, bool startOnCreate = false, bool autodestroy = false, bool scaled = true, int repetitionNumber = 0, in bool infinite = false, Action onTimerActiveEvent = null, Action onFinishedEvent = null, string name = "Timer")
        {
            if (duration <= 0)
            {
                onFinishedEvent?.Invoke();
                Debug.LogWarning("Duration is negative or null. No timer created. Try passing a duration strictly greater than 0.");
                return null;
            }
            if (target == null) return null;

            pool.Take(out MasterTimer timer);

            if (timer == null) timer = new GameObject().AddComponent(typeof(MasterTimer)) as MasterTimer;

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

            Guid id = Guid.NewGuid();
            timer.id = id;
            return new Timer(timer, id);
        }

        public bool IsActive { get; private set; }
        public float ElapsedTime { get; private set; }
        public float RemainingTime => duration - ElapsedTime;

        public float duration;

        private Guid id;
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

        private MasterTimer() { }

        private void Update()
        {
            if (target == null) 
            {
                Kill(id);
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
                        if (autodestroy) Kill(id);
                        else Stop();
                    }

                    OnFinishedEvent?.Invoke();
                }
            }
        }

        private void Play()
        {
            Stop();
            IsActive = true;
        }

        private void Stop()
        {
            currentRepetitionNumber = repetitionNumber;
            IsActive = false;
            ElapsedTime = 0;
        }

        private void Kill(in Guid id)
        {
            Stop();
            pool.MoveIn(this);
        }

        private void OnDestroy()
        {
            pool.Remove(this);
        }
    }
}