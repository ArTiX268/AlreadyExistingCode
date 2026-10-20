using ArTiX.Utils.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Utils.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        private const int INITIAL_POOL_SIZE = 16;
        private const int MAX_POOL_SIZE = 48;

        [SerializeField] private AudioSourceExtension sourcePrefab;

        private Pool<AudioSourceExtension> pool;

        private readonly Dictionary<AudioDatasSO, int> playbackNb = new Dictionary<AudioDatasSO, int>();

        protected override void Awake()
        {
            SetInstance(this);

            if (sourcePrefab == null)
            {
                Debug.LogError($"{nameof(AudioManager)} has no {nameof(sourcePrefab)} assigned, the audio pool stays empty.", this);
                return;
            }

            pool = new Pool<AudioSourceExtension>(CreateSource, INITIAL_POOL_SIZE, MAX_POOL_SIZE);
        }

        private AudioSourceExtension CreateSource()
        {
            return Instantiate(sourcePrefab, transform);
        }

        public AudioSourceExtension Play(in AudioDatasSO datas) => Play(datas, transform.position, null);
        public AudioSourceExtension Play(in AudioDatasSO datas, in Vector3 position) => Play(datas, position, null);
        public AudioSourceExtension Play(in AudioDatasSO datas, in Transform follow) => Play(datas, follow.position, follow);

        private AudioSourceExtension Play(in AudioDatasSO datas, in Vector3 position, in Transform follow)
        {
            if (playbackNb.ContainsKey(datas) && playbackNb[datas] == datas.MaxConcurrent) return null;

            AudioSourceExtension source = pool.GetPooledObject();
            if (source == null) return null;

            source.transform.SetParent(follow == null ? transform : follow);
            source.transform.position = position;

            if (!playbackNb.ContainsKey(datas)) playbackNb.Add(datas, 1);
            else playbackNb[datas]++;

            source.OnFinishPlayingEvent += OnSourceFinishedPlayingEvent;

            source.Play(datas);

            return source;
        }

        private void OnSourceFinishedPlayingEvent(object sender, System.EventArgs args)
        {
            AudioSourceExtension source = (sender as AudioSourceExtension);
            playbackNb[source.Datas]--;
            source.OnFinishPlayingEvent -= OnSourceFinishedPlayingEvent;
        }
    }
}
