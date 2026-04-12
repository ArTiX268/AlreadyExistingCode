using ArTiX.Tools;
using Godot;
using System.Collections.Generic;

// Author : Aidan Bachelez

namespace ArTiX.Sound
{
	public partial class SoundManager : Node
    {
        public enum EMusicType
        {
            Level,
            Win,
            MainMenu
        }

        public enum ESoundType
        {
            None,
            Moving,
            Win,
            UI_Click,
            UI_Focus,
        }

        [Export] private Node sfxParent;
        [Export] private Node musicParent;
        [Export] private float musicVolume;
        [Export] private float sfxVolume;
        [Export] private bool startMuted;

        private static SoundManager instance;
        public static SoundManager Instance
        {
            get
            {
                instance ??= new SoundManager();
                return instance;
            }
        }

        private const string MUSIC_DIR_PATH = "res://Assets/Audio/Musics/";

        //Music
        // Load music at runtime
        private static readonly Dictionary<EMusicType, string> musics = new Dictionary<EMusicType, string>
        {
            //{ EMusicType.MainMenu, ResourceLoader.Load<AudioStream>(MUSIC_DIR_PATH + "") },
            //{ EMusicType.Level, ResourceLoader.Load<AudioStream>(MUSIC_DIR_PATH + "") },
            //{ EMusicType.Win, ResourceLoader.Load<AudioStream>(MUSIC_DIR_PATH + "") },
        };

        //Sfx
        // Preload them
        private static readonly Dictionary<ESoundType, List<AudioStream>> sfxs = new Dictionary<ESoundType, List<AudioStream>>
        {
            //{ ESoundType.UI_Click, Utils.GetAllFilesOfTypeInDir<AudioStream>("res://Assets/Audio/SFX/UI/Clicks/") },
        };

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

		public override void _Ready()
		{
			if (instance != null && IsInstanceValid(instance))
			{
				QueueFree();
				GD.Print(nameof(SoundManager) + " Instance already exist, destroying the last added.");
				return;
			}

			instance = this;

            return;

            sfxBusIndex = AudioServer.GetBusIndex(SFX_BUS);
            musicBusIndex = AudioServer.GetBusIndex(MUSIC_BUS);

            AudioStreamPlayer audioPlayer;

            AudioServer.SetBusVolumeDb(sfxBusIndex, sfxVolume);
            AudioServer.SetBusVolumeDb(musicBusIndex, musicVolume);

            for (int i = 0; i < NB_MAX_SFX; i++)
            {
                audioPlayer = new AudioStreamPlayer
                {
                    Bus = SFX_BUS
                };

                sfxParent.AddChild(audioPlayer);
                sfxAudioPlayers.Add(audioPlayer);
            }

            musicAudioPlayer.Bus = MUSIC_BUS;

            musicParent.AddChild(musicAudioPlayer);

            AudioServer.SetBusMute(sfxBusIndex, !SoundOn);
            AudioServer.SetBusMute(musicBusIndex, !SoundOn);

            if (startMuted)
                ToggleSound(out bool soundOn);
        }

        public void ToggleSound(out bool soundOn)
        {
            SoundOn = !SoundOn;
            soundOn = SoundOn;
            AudioServer.SetBusMute(sfxBusIndex, !SoundOn);
            AudioServer.SetBusMute(musicBusIndex, !SoundOn);
        }

        // Music
        public void PlayMusic(EMusicType musicType)
        {
            // Check if music is already playing the given music
            if (musics.TryGetValue(musicType, out string musicPath))
            {
                AudioStream music = ResourceLoader.Load<AudioStream>(musicPath);

                if (musicAudioPlayer.Playing)
                {
                    if (musicAudioPlayer.Stream == music)
                        return;

                    if (musicAudioPlayer.Stream != null)
                    {
                        FadeOutSound(musicAudioPlayer).Finished += () =>
                        {
                            musicAudioPlayer.Stream = music;
                            musicAudioPlayer.Play();
                            FadeInSound(musicAudioPlayer);
                        };
                    }
                }
                else
                {
                    musicAudioPlayer.Stream = music;
                    musicAudioPlayer.Play();
                    FadeInSound(musicAudioPlayer);
                }
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
        public void PlaySfx(ESoundType soundType)
        {
            if (soundType == ESoundType.None) return;

            if (sfxs.TryGetValue(soundType, out List<AudioStream> sfx))
            {
                foreach (AudioStreamPlayer audioPlayer in sfxAudioPlayers)
                {
                    if (audioPlayer.Playing) continue;

                    audioPlayer.Stream = Utils.GetRandomElementFromList(sfx);

                    audioPlayer.Play();
                    return;
                }

                AudioStreamPlayer player = sfxAudioPlayers[0];
                sfxAudioPlayers.RemoveAt(0);
                sfxAudioPlayers.Add(player);
                player.Stop();
                player.Stream = Utils.GetRandomElementFromList(sfx);
                player.Play();
            }
            else
                GD.Print("This sounds as not been added to sfx dictionary");
        }

        public void StopSfx(ESoundType soundType)
        {
            if (soundType == ESoundType.None) return;

            List<AudioStream> possibleSfxs = sfxs[soundType];

            foreach (AudioStreamPlayer audioPlayer in sfxAudioPlayers)
            {
                if (audioPlayer.Playing && possibleSfxs.Contains(audioPlayer.Stream))
                {
                    audioPlayer.Stop();
                }
            }
        }

        private Tween FadeInSound(AudioStreamPlayer player)
        {
            Tween tween = CreateTween();
            tween.TweenProperty(player, Utils.TWEEN_VOLUME, 0, FADE_IN_DURATION)
                .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad)
                .From(SOUND_ZERO);
            return tween;
        }

        private Tween FadeOutSound(AudioStreamPlayer player)
        {
            Tween tween = CreateTween();
            tween.TweenProperty(player, Utils.TWEEN_VOLUME, SOUND_ZERO, FADE_OUT_DURATION)
                .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
            return tween;
        }

        protected override void Dispose(bool disposing)
        {
            instance = null;
            base.Dispose(disposing);
        }
    }
}
