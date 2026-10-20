using System;
using UnityEngine;

namespace ArTiX.Utils.TickSystem
{
    public class TickSystem : MonoBehaviour
    {
        public enum ETick
        {
            First,
            Main,
            Late
        }

        private static TickSystem instance;
        public static TickSystem Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameObject(nameof(TickSystem), typeof(TickSystem)).GetComponent<TickSystem>();

                return instance;
            }
        }

        [SerializeField, Tooltip("In seconds")] private float tickInterval = 0.2f;
        public float TickInterval => tickInterval;
        private float timer;

        public event EventHandler OnFirstTick;
        public event EventHandler OnTick;
        public event EventHandler OnLateTick;

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("Instance of TickSystem already exists.");
                Destroy(gameObject);
                return;
            }

            instance = this;

            timer = tickInterval;
        }

        // Update is called once per frame
        private void Update()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = tickInterval;

                OnFirstTick?.Invoke(this, EventArgs.Empty);
                OnTick?.Invoke(this, EventArgs.Empty);
                OnLateTick?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}