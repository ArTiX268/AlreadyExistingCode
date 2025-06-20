using UnityEngine;

public class Timer : MonoBehaviour
{
    [HideInInspector] public bool timerFinished;

    private float currentTimer = 0;
    private bool canUpdateTimer = false;

    [Header("Timer parameters")]
    private float _duration = 0;
    private float _timeBeforeStart = 0;

    private int _numberOfLoops = 0;

    private bool _scaled = true;
    private bool _loopInfinitly = false;
    private bool _fixedTimer = false;
    private bool _destroyedOnFinished = false;
    private bool _destroy = false;

    private void Update()
    {
        if (timerFinished)
            timerFinished = false;


        if (_destroy)
            TimerManager.DestroyTimer(gameObject);


        if (!_fixedTimer)
        {
            if (_scaled)
                TimerLogic(Time.deltaTime);
            else
                TimerLogic(Time.unscaledDeltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (timerFinished)
            timerFinished = false;

        if (_fixedTimer)
        {
            if (_scaled)
                TimerLogic(Time.fixedDeltaTime);
            else
                TimerLogic(Time.fixedUnscaledDeltaTime);
        }
    }

    private void LateUpdate()
    {
    }

    public void InitializeValues(float duration, float timeBeforeStart = 0, int numberOfLoops = 0, bool destroyedOnFinished = true, bool scaled = true, bool loopInfinitly = false, bool fixedTimer = false)
    {
        if (duration < 0)
        {
            Debug.LogError("Duration is negative. It has been set to 0 by default.");
            _duration = 0;
        }
        else
        {
            _duration = duration;
        }

        if (_timeBeforeStart < 0)
        {
            Debug.LogError("Time before start is negative. It has been set to 0 by default.");
            _timeBeforeStart = 0;
        }
        else
        {
            _timeBeforeStart = duration;
        }

        if (numberOfLoops < 0)
        {
            Debug.LogError("Number of loops is negative. It has been set to 0 by default.");
            _numberOfLoops = 0;
        }
        else
        {
            _numberOfLoops = numberOfLoops;
        }

        _scaled = scaled;
        _loopInfinitly = loopInfinitly;
        _fixedTimer = fixedTimer;
        _destroyedOnFinished = destroyedOnFinished;
    }

    public void StartTimer() { canUpdateTimer = true; }

    public void PauseTimer() { canUpdateTimer = false; }

    public void ResumeTimer() { canUpdateTimer = true; }

    public int GetNumberOfLoops() { return _numberOfLoops; }

    public void StopTimer()
    {
        currentTimer = 0;
        canUpdateTimer = false;
        timerFinished = false;
    }

    public void RestartTimer()
    {
        currentTimer = 0;
        canUpdateTimer = true;
    }

    public void ClearTimer()
    {
        ResetTimerValues();
        canUpdateTimer = false;
        currentTimer = 0;
        timerFinished = false;
    }

    private void ResetTimerValues()
    {
        _duration = 0;
        _timeBeforeStart = 0;
        _numberOfLoops = 0;
        _scaled = true;
        _loopInfinitly = false;
        _fixedTimer = false;
    }

    private bool IncrementTimer(float incrementationValue)
    {
        if (canUpdateTimer)
        {
            currentTimer += incrementationValue;

            if (currentTimer >= _duration)
            {
                canUpdateTimer = false;
                currentTimer = 0;
                return true;
            }

            return false;
        }

        return false;
    }

    private void TimerLogic(float incrementationValue)
    {
        if (IncrementTimer(incrementationValue))
        {
            if (_loopInfinitly)
            {
                RestartTimer();
            }
            else
            {
                timerFinished = true;

                if (_numberOfLoops == 0)
                {
                    if (_destroyedOnFinished)
                    {
                        _destroy = true;
                    }
                }

                if (_numberOfLoops > 0)
                {
                    _numberOfLoops -= 1;
                    RestartTimer();
                }
            }
        }
    }
}
