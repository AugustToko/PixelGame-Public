using System.Collections.Generic;
using System.Linq;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.Helper;
using PixelGameAssets.Scripts.UI.MusicSystem;
using UnityEngine;
using EventType = PixelGameAssets.Scripts.EventManager.EventType;
using Random = System.Random;

namespace PixelGameAssets.Scripts.Core.Audio
{
    /// <summary>
    /// Audio manager.音乐音效的简单管理器
    /// ~目前支持 4 个音频同时播放: shoot, skill, hit(pickup), bgm~
    /// TODO: 使用更精确的目录, 而非 BgmDictionary 与 FxDictionary
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        ///  单例
        /// </summary>
        public static AudioManager Instance;

        /// <summary>
        /// 最大同时音频播放 40~256
        /// </summary>
        [Range(0, 256)] public int audioSourceCount = 20;

        #region DirName

        private const string SoundsDirName = "Sounds";

        private const string FxSoundsDir = SoundsDirName + "/FX";

        private const string NpcSoundsDir = SoundsDirName + "/NPC/Debug";

        private const string BgmSoundsDir = SoundsDirName + "/BGM";

        #endregion

        #region AudioSources

        // 用于播放音频特效 (用于射击) (FX)
        private AudioSource _audioSourceEffectShoot;

        // 用于播放音频特效 (捡到物品等) (FX)
        private AudioSource _audioSourceEffectHit;

        // 用于环境音 (FX)
        private AudioSource _ambientSoundEffect;

        // 用于播放音乐 (BGM)
        private AudioSource _bgAudioSource;

        // npc 语音 (VOICE)
        private AudioSource _npcAudioSource;

        // 背景音乐和音效的音频源
        private AudioSource[] _audioSources;

        // 随机源 (FX)
        private List<AudioSource> _randomSources;

        #endregion

        #region Dictionary

        /// <summary>
        /// 将声音放入字典中，方便管理
        /// 背景音乐集
        /// <see cref="MusicBar"/>
        /// </summary>
        private Dictionary<Song, AudioClip> BgmDictionary { get; set; }

        /// <summary>
        /// 将声音放入字典中，方便管理
        /// 音效集
        /// </summary>
        private Dictionary<string, AudioClip> FxDictionary { get; set; }

        /// <summary>
        /// npc语音集
        /// </summary>
        private Dictionary<string, AudioClip> NpcDictionary { get; set; }

        #endregion

        /// <summary>
        /// bgm 顺序列表
        /// </summary>
        [HideInInspector]
        public static List<Song> BgmList { get; set; }

        public AudioClip CurrentBgmClip { get; private set; }

        /// <summary>
        /// 当前序列
        /// </summary>
        public int currentMusicIndex;

        private bool _needListen;

        /// <summary>
        /// Awake this instance.
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this) Destroy(gameObject);

            //获取音频源
            _audioSources = GetComponents<AudioSource>();
            _bgAudioSource = _audioSources[0];
            _ambientSoundEffect = gameObject.AddComponent<AudioSource>();
            _audioSourceEffectShoot = gameObject.AddComponent<AudioSource>();
            _audioSourceEffectHit = gameObject.AddComponent<AudioSource>();
            _npcAudioSource = gameObject.AddComponent<AudioSource>();

            InitRandomSource();

            InitVolume();

            //加载资源存的所有音频资源
            LoadAudio();
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            // 一旦播放完成, 立即随机播放
            if (_needListen && _bgAudioSource.clip != null && !_bgAudioSource.isPlaying)
            {
                RandomPlay();
            }
        }

        private void InitVolume()
        {
            SetUpBgmVolume(PlayerPrefs.GetFloat(PrefsHelper.GameSettings.MusicVolume, 0.7f));
            SetUpVoiceVolume(PlayerPrefs.GetFloat(PrefsHelper.GameSettings.VoiceVolume, 1f));
            SetUpFxVolume(PlayerPrefs.GetFloat(PrefsHelper.GameSettings.FxVolume, 0.7f));
        }

        private void InitRandomSource()
        {
            _randomSources = new List<AudioSource>();

            for (var i = 0; i < audioSourceCount; i++)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                _randomSources.Add(audioSource);
            }
        }

        /// <summary>
        /// 随机播放且监听是否播放完成
        /// </summary>
        public void RandomPlay()
        {
            var song = BgmList[new Random().Next(0, BgmList.Count)];
            PlayBgm(song);
            _needListen = true;
        }

        /// <summary>
        /// Load the audio
        /// </summary>
        private void LoadAudio()
        {
            // 初始化字典
            BgmDictionary = new Dictionary<Song, AudioClip>();
            FxDictionary = new Dictionary<string, AudioClip>();
            NpcDictionary = new Dictionary<string, AudioClip>();

            BgmList = new List<Song>();

            // 本地加载 
            var bgmArray = Resources.LoadAll<AudioClip>(BgmSoundsDir);
            var fxArray = Resources.LoadAll<AudioClip>(FxSoundsDir);
            var npcArray = Resources.LoadAll<AudioClip>(NpcSoundsDir);

            // 存放到字典和名称列表
            foreach (var item in bgmArray)
            {
                // TODO: 元数据获取
                var song = new Song(item.name, "null", "null");
                BgmDictionary.Add(song, item);
                BgmList.Add(song);
            }

            foreach (var item in fxArray) FxDictionary.Add(item.name, item);
            foreach (var item in npcArray) NpcDictionary.Add(item.name, item);
        }

        /// <summary>
        /// Play the background audio
        /// </summary>
        /// <param name="song">Audio name.</param>
        private void PlayBgm(Song song)
        {
            if (!BgmDictionary.ContainsKey(song)) return;

            _bgAudioSource.clip = BgmDictionary[song];
            CurrentBgmClip = BgmDictionary[song];

            _bgAudioSource.Play();
            _needListen = true;

            EventCenter.Broadcast(EventType.SetUpMusicInfo, song);
        }

        /// <summary>
        /// Play the background music.
        /// </summary>
        /// <param name="songName">BGM name.</param>
        public void PlayBgm(string songName)
        {
            BgmList.ForEach(song =>
            {
                if (song.musicName.Equals(songName))
                {
                    PlayBgm(song);
                }
            });
        }

        /// <summary>
        /// Play the audio effect (shoot)
        /// </summary>
        /// <param name="audioEffectName">Audio effect name.</param>
        public void PlayAudioEffectShoot(string audioEffectName)
        {
            if (!FxDictionary.ContainsKey(audioEffectName)) return;

            _audioSourceEffectShoot.clip = FxDictionary[audioEffectName];

            _audioSourceEffectShoot.Play();
        }

        /// <summary>
        /// Play the audio effect (hit, button click)
        /// </summary>
        /// <param name="audioEffectName">Audio effect name.</param>
        public void PlayAudioEffectHit(string audioEffectName)
        {
            if (!FxDictionary.ContainsKey(audioEffectName)) return;

            _audioSourceEffectHit.clip = FxDictionary[audioEffectName];

            _audioSourceEffectHit.Play();
        }

        public void PlayAmbientSoundEffect(string audioEffectName)
        {
            if (!FxDictionary.ContainsKey(audioEffectName)) return;

            _ambientSoundEffect.clip = FxDictionary[audioEffectName];

            _ambientSoundEffect.Play();
        }

        /// <summary>
        /// 播放 Npc 语音 (单 source)
        /// </summary>
        /// <param name="audioEffectName"></param>
        public void PlayNpcSoundEffect(string audioEffectName)
        {
            if (!NpcDictionary.ContainsKey(audioEffectName)) return;

            _npcAudioSource.clip = NpcDictionary[audioEffectName];

            _npcAudioSource.Play();
        }

        /// <summary>
        /// 随机分配 AudioSource
        /// (子弹掉落， 技能)
        /// </summary>
        /// <param name="audioEffectName">audio name</param>
        public void AddToRandomFxSource(string audioEffectName)
        {
            foreach (var audioSource in _randomSources.Where(audioSource => !audioSource.isPlaying))
            {
                audioSource.clip = FxDictionary[audioEffectName];
                audioSource.Play();
                break;
            }
        }

        /// <summary>
        /// 下首歌
        /// </summary>
        public void Next()
        {
            var size = AudioManager.Instance.BgmDictionary.Count;
            currentMusicIndex = currentMusicIndex >= size - 1 ? 0 : currentMusicIndex++;
            var song = BgmList[currentMusicIndex];
            PlayBgm(song);
        }

        /// <summary>
        /// 上一首歌曲
        /// </summary>
        public void Previous()
        {
            var size = Instance.BgmDictionary.Count;
            currentMusicIndex = currentMusicIndex - 1 < 0 ? size - 1 : currentMusicIndex--;
            var song = BgmList[currentMusicIndex];
            PlayBgm(song);
        }

        public void BgmStop()
        {
            _needListen = false;
            _bgAudioSource.Pause();
        }

        public void BgmResume()
        {
            _bgAudioSource.Play();
        }

        // 音量控制

        #region VolumeController

        /// <summary>
        /// 控制 BGM 音量
        /// </summary>
        public void SetUpBgmVolume(float val)
        {
            _bgAudioSource.volume = val;
        }

        /// <summary>
        /// 控制 FX 音量
        /// </summary>
        public void SetUpFxVolume(float val)
        {
            _ambientSoundEffect.volume = val;
            _audioSourceEffectShoot.volume = val;
            _audioSourceEffectHit.volume = val;

            foreach (var source in _randomSources)
            {
                source.volume = val;
            }
        }

        /// <summary>
        /// 控制 Voice 音量
        /// </summary>
        public void SetUpVoiceVolume(float val)
        {
            _npcAudioSource.volume = val;
        }

        private void SetUpAllSoundsVolume(float val)
        {
            SetUpBgmVolume(val);
            SetUpFxVolume(val);
            SetUpVoiceVolume(val);
        }

        #endregion
    }
}