using ArTiX.Effects.Tween;
using ArTiX.Shmup.Datas;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ArTiX.Shmup.Sound
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private SoundData datas;

        [SerializeField] private AudioMixer masterMixer;
        [SerializeField] private AudioMixerGroup musicMixer;
        [SerializeField] private AudioMixerGroup sfxMixer;

        private static SoundManager instance;
        public static SoundManager Instance
        {
            get
            {
                instance ??= new GameObject("SoundManager", typeof(SoundManager)).GetComponent<SoundManager>();
                return instance;
            }
        }

        private const int NB_MAX_SFX = 15;

        private readonly List<AudioSource> activeSfxSources = new List<AudioSource>();
        private readonly List<AudioSource> inactiveSfxSources = new List<AudioSource>();
        private readonly Dictionary<AudioSource, Action> OnFinishedPlayingEventsDic = new Dictionary<AudioSource, Action>(); 

        private AudioSource musicSource;

        private void Awake()
        {
            datas.Initialize();

            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            musicSource = new GameObject($"MusicPlayer", typeof(AudioSource)).GetComponent<AudioSource>();
            musicSource.transform.SetParent(transform);
            musicSource.outputAudioMixerGroup = musicMixer;
            musicSource.playOnAwake = false;

            for (int i = 0; i < NB_MAX_SFX; i++) CreateSfxSource(i);
        }

        private void Update()
        {
            for (int i = activeSfxSources.Count - 1; i >= 0; i--)
            {
                if (!activeSfxSources[i].isPlaying)
                {
                    inactiveSfxSources.Add(activeSfxSources[i]);
                    OnFinishedPlayingEventsDic[activeSfxSources[i]]?.Invoke();
                    OnFinishedPlayingEventsDic[activeSfxSources[i]] = null;
                    activeSfxSources.RemoveAt(i);
                }
            }
        }

        public void PlayMusic(EMusic music)
        {
            musicSource.Stop();
            musicSource.clip = datas.GetMusic(music);
            musicSource.Play();
            FadeInMusic();
        }

        public void FadeInMusic()
        {
            musicSource.TweenVolume(
                startValue: 0,
                targetValue: 1,
                animParams: new MasterTween.AnimParams()
                {
                    duration = 0.5f,
                    transition = MasterTween.ETransition.SmoothStart4
                }
            );
        }

        public void FadeOutMusic()
        {
            musicSource.TweenVolume(
                targetValue: 0, 
                animParams: new MasterTween.AnimParams()
                {
                    duration = 0.5f,
                    transition = MasterTween.ETransition.SmoothStart4
                },
                onFinishedStepEvent: musicSource.Stop
            );
        }

        public void PlaySfx(in ESound sfx, in Action onFinishedEvent = null)
        {
            AudioClip[] sfxs = datas.GetSfxs(sfx);
            if (sfxs == null) 
            {
                Debug.LogWarning("Found no sounds in the sound database, or a field is null.");
                return; 
            }

            AudioSource source = null;
            if (inactiveSfxSources.Count > 0)
            {
                source = inactiveSfxSources[0];
                activeSfxSources.Add(source);
                inactiveSfxSources.Remove(source);
            }
            else
            {
                CreateSfxSource(activeSfxSources.Count);
                activeSfxSources.Add(source);
            }

            source.clip = sfxs[UnityEngine.Random.Range(0, sfxs.Length)];
            source.Play();
            OnFinishedPlayingEventsDic[source] = onFinishedEvent;
        }

        private void CreateSfxSource(int nb)
        {
            AudioSource source = new GameObject($"SFXPlayer_{nb}", typeof(AudioSource)).GetComponent<AudioSource>();
            source.transform.SetParent(transform);
            source.outputAudioMixerGroup = sfxMixer;
            source.playOnAwake = false;
            inactiveSfxSources.Add(source);
            OnFinishedPlayingEventsDic.Add(source, null);
        }

        #region VolumeManagment

        public void SetMasterVolume(float level) => SetVolume(level, "masterVolume");
        public void SetSFXVolume(float level) => SetVolume(level, "sfxsVolume");
        public void SetMusicVolume(float level) => SetVolume(level, "musicsVolume");

        private void SetVolume(in float level, string parameter)
            => masterMixer.SetFloat(parameter, Mathf.Log10(level) * 20f);

        #endregion
    }
}