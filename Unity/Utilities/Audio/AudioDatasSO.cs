using UnityEngine;
using UnityEngine.Audio;

namespace ArTiX.Utils.Audio
{
    [CreateAssetMenu(fileName = "AudioDatasSO", menuName = "Datas/Audio")]
    public class AudioDatasSO : ScriptableObject
    {
        [SerializeField] private AudioClip[] clips; // random pick = variation
        [SerializeField] private AudioMixerGroup mixerGroup;
        [SerializeField] private bool loop;
        [SerializeField, Range(0, 1)] private float minVolume = 1;
        [SerializeField, Range(0, 1)] private float maxVolume = 1;
        [SerializeField, Range(-3, 3)] private float minPitch = 1;
        [SerializeField, Range(-3, 3)] private float maxPitch = 1;
        [SerializeField, Range(0f, 1f), Tooltip("0 means 2D, 1 means 3D")] private float spatialBlend = 1f;
        [SerializeField, Range(0, 5)] private float dopplerLevel = 1;
        [SerializeField, Range(0, 360)] private int spread = 0;
        [SerializeField, Min(0)] private float minDistance = 1f;
        [SerializeField, Min(0)] private float maxDistance = 25f;
        [SerializeField] private AudioRolloffMode volumeRollof;
        [field: SerializeField, Tooltip("Max number of AudioSources playing this sound at the same time.")] public int MaxConcurrent { get; private set; } = 4;


        public void ApplyTo(AudioSource source)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            source.clip = clip;
            source.outputAudioMixerGroup = mixerGroup;
            source.volume = Random.Range(minVolume, maxVolume);
            source.pitch = Random.Range(minPitch, maxPitch);
            source.spatialBlend = spatialBlend;
            source.dopplerLevel = dopplerLevel;
            source.spread = spread;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.loop = loop;
            source.rolloffMode = volumeRollof;
        }
    }
}