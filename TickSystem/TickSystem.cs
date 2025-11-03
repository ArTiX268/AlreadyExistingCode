using System;
using UnityEngine;

namespace Com.ArTiX
{
    public class TickSystem : MonoBehaviour
    {
        public enum ETickOrder
        {
            None,
            First,
            Last
        }

        private static event EventHandler OnTickEvent;
        private static event EventHandler OnFirstTickEvent;
        private static event EventHandler OnLastTickEvent;

        private const float TICK_INTERVAL = 1f;

        private float tickTimer;

        private void Update()
        {
            tickTimer += Time.deltaTime;
            if (tickTimer >= TICK_INTERVAL)
            {
                tickTimer = 0;
                OnFirstTickEvent?.Invoke(this, EventArgs.Empty);
                OnTickEvent?.Invoke(this, EventArgs.Empty);
                OnLastTickEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles subscription to event tick.
        /// </summary>
        /// <param name="pEventHandler">The event to call on tick.</param>
        /// <param name="pOrder">Defines if it must be called among the first, last or middle events.</param>
        public static void SubscribeEvent(in EventHandler pEventHandler, in ETickOrder pOrder = ETickOrder.None)
        {
            switch (pOrder)
            {
                case ETickOrder.None:
                    OnTickEvent += pEventHandler;
                    break;
                case ETickOrder.First:
                    OnFirstTickEvent += pEventHandler;
                    break;
                case ETickOrder.Last:
                    OnLastTickEvent += pEventHandler;
                    break;
            }
        }
    }
}