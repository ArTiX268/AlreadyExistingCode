using System.Collections.Generic;
using Com.IsartDigital.Sokoban.Tools;
using Godot;

// Author : Aidan Bachelez

namespace Com.IsartDigital.Sokoban
{
	public partial class SoundManager : Node2D
    {
        [Export] private Node sfxParent;
        [Export] private Node musicParent;
        [Export] private float musicVolume;
        [Export] private float sfxVolume;
        [Export] private bool startMuted;

        private const string MUSIC_DIR_PATH = "res://Assets/Audio/Musics/";

        //Music
        private static readonly Dictionary<EMusicType, AudioStream> musics = new Dictionary<EMusicType, AudioStream>
        {
            { EMusicType.MainMenu, ResourceLoader.Load<AudioStream>(MUSIC_DIR_PATH + "BackGroundMusic.mp3") },
            { EMusicType.Level, ResourceLoader.Load<AudioStream>(MUSIC_DIR_PATH + "NeoSoulNight.mp3") },
            { EMusicType.Win, ResourceLoader.Load<AudioStream>(MUSIC_DIR_PATH + "NeoSoulNight.mp3") },
        };

        //Sfx
        private static readonly Dictionary<ESoundType, List<AudioStream>> sfxs = new Dictionary<ESoundType, List<AudioStream>>
        {
            { ESoundType.UI_Click, Utils.GetAllFilesOfTypeInDir<AudioStream>("res://Assets/Audio/SFX/UI/Clicks/") },
        };

        private static readonly PackedScene scnSoundManager = ResourceLoader.Load<PackedScene>("res://Scenes/SoundManager.tscn");
		private static SoundManager instance;

        private const int NB_MAX_SFX = 10;

        private const float FADE_IN_DURATION = 0.5f;
        private const float FADE_OUT_DURATION = 0.5f;
        private const int SOUND_ZERO = -80;

        private const string MUSIC_BUS = "Music";
        private const string SFX_BUS = "SFX";

        private readonly List<AudioStreamPlayer> sfxAudioPlayers = new List<AudioStreamPlayer>();
        private readonly AudioStreamPlayer musicAudioPlayer = new AudioStreamPlayer();

        public bool SoundOn { get; private set; } = true;

        private int musicBusIndex;
        private int sfxBusIndex;

        public static SoundManager GetInstance()
		{
			if (instance == null) instance = scnSoundManager.Instantiate<SoundManager>();
			return instance;
		}

		public override void _Ready()
		{
			if (instance != null && IsInstanceValid(instance))
			{
				QueueFree();
				GD.Print(nameof(SoundManager) + " Instance already exist, destroying the last added.");
				return;
			}

			instance = this;

            sfxBusIndex = AudioServer.GetBusIndex(SFX_BUS);
            musicBusIndex = AudioServer.GetBusIndex(MUSIC_BUS);

            AudioStreamPlayer lAudioPlayer;

            AudioServer.SetBusVolumeDb(sfxBusIndex, sfxVolume);
            AudioServer.SetBusVolumeDb(musicBusIndex, musicVolume);

            for (int i = 0; i < NB_MAX_SFX; i++)
            {
                lAudioPlayer = new AudioStreamPlayer
                {
                    Bus = SFX_BUS
                };

                sfxParent.AddChild(lAudioPlayer);
                sfxAudioPlayers.Add(lAudioPlayer);
            }

            musicAudioPlayer.Bus = MUSIC_BUS;

            musicParent.AddChild(musicAudioPlayer);

            AudioServer.SetBusMute(sfxBusIndex, !SoundOn);
            AudioServer.SetBusMute(musicBusIndex, !SoundOn);

            if (startMuted)
                ToggleSound(out bool lSoundOn);
        }

        public void ToggleSound(out bool pSoundOn)
        {
            SoundOn = !SoundOn;
            pSoundOn = SoundOn;
            AudioServer.SetBusMute(sfxBusIndex, !SoundOn);
            AudioServer.SetBusMute(musicBusIndex, !SoundOn);
        }

        // Music
        public void PlayMusic(EMusicType pMusicType)
        {
            // Check if music is already playing the given music
            AudioStream lMusic = musics[pMusicType];
            if (musicAudioPlayer.Playing)
            {
                if (musicAudioPlayer.Stream == lMusic)
                    return;

                if (musicAudioPlayer.Stream != null)
                {
                    FadeOutSound(musicAudioPlayer).Finished += () =>
                    {
                        musicAudioPlayer.Stream = lMusic;
                        musicAudioPlayer.Play();
                        FadeInSound(musicAudioPlayer);
                    };
                }
            }
            else
            {
                musicAudioPlayer.Stream = lMusic;
                musicAudioPlayer.Play();
                FadeInSound(musicAudioPlayer);
            }
        }

        public void PauseMusic()
        {
            if (!musicAudioPlayer.StreamPaused)
                FadeOutSound(musicAudioPlayer).Finished += () => musicAudioPlayer.StreamPaused = true;
        }

        public void ResumeMusic()
        {
            if (musicAudioPlayer.StreamPaused)
                musicAudioPlayer.StreamPaused = false;
        }

        // SFX
        public void PlaySfx(ESoundType pSoundType)
        {
            if (pSoundType == ESoundType.None) return;

            if (sfxs.TryGetValue(pSoundType, out List<AudioStream> lSfx))
            {
                foreach (AudioStreamPlayer lAudioPlayer in sfxAudioPlayers)
                {
                    if (lAudioPlayer.Playing) continue;

                    lAudioPlayer.Stream = Utils.GetRandomElementFromList(lSfx);

                    lAudioPlayer.Play();
                    return;
                }

                AudioStreamPlayer lPlayer = sfxAudioPlayers[0];
                sfxAudioPlayers.RemoveAt(0);
                sfxAudioPlayers.Add(lPlayer);
                lPlayer.Stop();
                lPlayer.Stream = Utils.GetRandomElementFromList(lSfx);
                lPlayer.Play();
            }
            else
                GD.Print("This sounds as not been added to sfx dictionary");
        }

        public void StopSfx(ESoundType pSoundType)
        {
            if (pSoundType == ESoundType.None) return;

            List<AudioStream> lPossibleSfxs = sfxs[pSoundType];

            foreach (AudioStreamPlayer lAudioPlayer in sfxAudioPlayers)
            {
                if (lAudioPlayer.Playing && lPossibleSfxs.Contains(lAudioPlayer.Stream))
                {
                    lAudioPlayer.Stop();
                }
            }
        }

        private Tween FadeInSound(AudioStreamPlayer pPlayer)
        {
            Tween lTween = CreateTween();
            lTween.TweenProperty(pPlayer, Utils.TWEEN_VOLUME, 0, FADE_IN_DURATION)
                .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad)
                .From(SOUND_ZERO);
            return lTween;
        }

        private Tween FadeOutSound(AudioStreamPlayer pPlayer)
        {
            Tween lTween = CreateTween();
            lTween.TweenProperty(pPlayer, Utils.TWEEN_VOLUME, SOUND_ZERO, FADE_OUT_DURATION)
                .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
            return lTween;
        }

        protected override void Dispose(bool pDisposing)
        {
            instance = null;
            base.Dispose(pDisposing);
        }
    }
}
