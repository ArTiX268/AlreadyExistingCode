using System;

namespace ArTiX.Utils
{
    public abstract class GameState
    {
        public event Action OnExitState;

        public abstract void EnterState();

        public abstract void Update();

        public virtual void ExitState()
        {
            OnExitState?.Invoke();
        }
    }
}