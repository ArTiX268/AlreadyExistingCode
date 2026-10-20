using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Shmup.Datas
{
    public enum ESound
    {
        UI_Clic,
        UI_Slider,
        Shots,
        Start
    }

    public enum EMusic
    {
        Menus,
        Level
    }

    [CreateAssetMenu(fileName = "SoundData", menuName = "Datas/SoundData")]
    public class SoundData : ScriptableObject
    {
        [Serializable]
        private struct SSfx
        {
            public ESound type;
            public AudioClip[] files;
        }

        [Serializable]
        private struct SMusic
        {
            public EMusic type;
            public AudioClip file;
        }

        [SerializeField] private SSfx[] sfxArray;
        [SerializeField] private SMusic[] musicArray;

        private readonly Dictionary<ESound, AudioClip[]> soundArrays = new Dictionary<ESound, AudioClip[]>();
        private readonly Dictionary<EMusic, AudioClip> musics = new Dictionary<EMusic, AudioClip>();

        public void Initialize()
        {
            foreach (SSfx soundList in sfxArray)
            {
                soundArrays.TryAdd(soundList.type, soundList.files);
            }
            foreach (SMusic music in musicArray)
            {
                musics.TryAdd(music.type, music.file);
            }
        }

        public AudioClip[] GetSfxs(in ESound sfxType)
        {
            soundArrays.TryGetValue(sfxType, out AudioClip[] clips);
            return clips;
        }

        public AudioClip GetMusic(in EMusic musicType)
        {
            musics.TryGetValue(musicType, out AudioClip music);
            return music;
        }
    }
}