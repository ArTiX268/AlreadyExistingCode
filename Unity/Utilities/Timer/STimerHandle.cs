namespace ArTiX.Utils
{
    public readonly struct STimerHandle
    {
        private readonly Timer timer;
        private readonly int version;

        public STimerHandle(Timer timer)
        {
            this.timer = timer;
            version = timer == null ? -1 : timer.Version;
        }

        public bool IsAlive => timer != null && timer.Version == version;

        public bool IsActive => IsAlive && timer.IsActive;
        public float ElapsedTime => IsAlive ? timer.ElapsedTime : 0;
        public float ElapsedTimeAsPercentage => IsAlive ? timer.ElapsedTimeAsPercentage : 0;
        public float RemainingTime => IsAlive ? timer.RemainingTime : 0;

        public void SetDuration(float duration)
        {
            if (IsAlive) timer.duration = duration;
        }

        public void Play()
        {
            if (IsAlive) timer.Play();
        }

        public void Stop()
        {
            if (IsAlive) timer.Stop();
        }

        public void Kill()
        {
            if (IsAlive) timer.Kill();
        }
    }
}
