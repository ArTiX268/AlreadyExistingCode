using System;
using UnityEngine;

namespace Com.ArTiX
{
    public class TickSystem : MonoBehaviour
    {
        public static event EventHandler OnTickEvent;

        private const float TICK_INTERVAL = 0.2f;

        private float tickTimer;

        private void Update()
        {
            tickTimer += Time.deltaTime;
            if (tickTimer >= TICK_INTERVAL)
            {
                tickTimer = 0;
                OnTickEvent?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}