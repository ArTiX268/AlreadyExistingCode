using ArTiX.Utils.Pool;
using System;
using UnityEngine;

namespace ArTiX.Utils.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceExtension : MonoBehaviour, IPooledObject<AudioSourceExtension>
    {
        public AudioSource Source { get; private set; }
        public AudioDatasSO Datas { get; private set; }

        private bool isPlaying;

        public Pool<AudioSourceExtension> Pool { get; set; }

        public event EventHandler OnFinishPlayingEvent;

        private void Awake()
        {
            Source = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (!isPlaying || Source.isPlaying) return;

            OnFinishPlayingEvent?.Invoke(this, EventArgs.Empty);
            Stop();
        }

        public void Play(in AudioDatasSO datas)
        {
            Datas = datas;
            datas.ApplyTo(Source);

            isPlaying = true;
            Source.Play();
        }

        public void Stop()
        {
            if (!isPlaying) return;

            Release();
        }

        public void Release()
        {
            isPlaying = false;
            Source.Stop();
            Source.clip = null;

            OnFinishPlayingEvent = null;
            Datas = null;

            Pool.ReturnToPool(this);
        }
    }
}
