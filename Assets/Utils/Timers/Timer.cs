using System;
using Player;
using UnityEngine;

namespace Utils.Timers
{

    public class CountdownTimer : Timer
    {
        public CountdownTimer(float time) : base(time) { }

        public override void Tick()
        {
            if (IsRunning && CurrentTime > 0)
            {
                CurrentTime -= Time.deltaTime;
            }
            
            if (IsRunning && CurrentTime <= 0)
            {
                Stop();
            }
        }

        public override bool IsFinished() => CurrentTime <= 0;
    }

    public class PlayerCooldownTimer : Timer
    {
        private PlayerStats _stats;
        private string _stat;
        public PlayerCooldownTimer(PlayerStats stats, string stat)
        {
            _stat = stat;
            _stats = stats;
            CurrentTime = 0f;
        }

        public override void Tick()
        {
            if (IsRunning && CurrentTime > 0)
            {
                CurrentTime -= Time.deltaTime;
            }
            
            if (IsRunning && CurrentTime <= 0)
            {
                Stop();
            }
        }
        
        public override void Start()
        {
            CurrentTime = _stats.GetStat(_stat);
            if (!IsRunning)
            {
                IsRunning = true;
                TimerManager.RegisterTimer(this);
                OnTimerStart.Invoke();
            }
        }

        public override bool IsFinished() => CurrentTime <= 0;
    }
    
    public class StopwatchTimer : Timer
    {
        public StopwatchTimer() : base() { }

        public override void Tick()
        {
            if (IsRunning)
            {
                CurrentTime += Time.deltaTime;
                return;
            }
            
            Stop();
        }

        public override bool IsFinished() => false;
        
        public override float Progress => CurrentTime;
    }
    public abstract class Timer : IDisposable
    {
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; protected set; }

        protected float InitialTime;

        public virtual float Progress => Mathf.Clamp(CurrentTime / InitialTime, 0, 1);
        
        public Action OnTimerStart = delegate {  };
        public Action OnTimerStop = delegate {  };
        
        protected Timer(float time = 0f)
        {
            InitialTime = time;
            CurrentTime = 0f;
        }

        public virtual void Start()
        {
            CurrentTime = InitialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                TimerManager.RegisterTimer(this);
                OnTimerStart.Invoke();
            }
        }
        
        public virtual void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                TimerManager.DeregisterTimer(this);
                OnTimerStop.Invoke();
            }
        }
        public abstract void Tick();

        public abstract bool IsFinished();
        
        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public virtual void Reset() => CurrentTime = InitialTime;
        public virtual void Reset(float time)
        {
            InitialTime = time;
            Reset();
        }
        
        // Dispose pattern
        private bool _disposed;
        ~Timer()
        {
            Dispose(false);
        }
        
        // Call Dispose to ensure deregistration of the timer from the TimerManager
        // and to avoid memory leaks
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            
            if (disposing)
            {
                TimerManager.DeregisterTimer(this);
            }
            
            _disposed = true;
        }
    }
}