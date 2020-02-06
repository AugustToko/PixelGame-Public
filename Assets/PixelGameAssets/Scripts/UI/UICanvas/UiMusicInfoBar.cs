using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.UI.MusicSystem;
using TMPro;
using UnityEngine;
using EventType = PixelGameAssets.Scripts.EventManager.EventType;

namespace PixelGameAssets.Scripts.UI.UICanvas
{
    /// <summary>
    /// 用于显示音乐信息
    /// </summary>
    public class UiMusicInfoBar : MonoBehaviour
    {
        public TextMeshProUGUI text;

        private void Awake()
        {
            EventCenter.AddListener<Song>(EventType.SetUpMusicInfo, UpdateMusicName);
        }

        private void Start()
        {
            // 主动获取当前歌曲名称
            if (AudioManager.Instance != null && AudioManager.Instance.CurrentBgmClip != null)
                UpdateMusicName(AudioManager.Instance.CurrentBgmClip.name);
            else
                UpdateMusicName("None");
        }

        /// <summary>
        /// 更新音乐信息
        /// </summary>
        /// <param name="song">Music EntityName</param>
        private void UpdateMusicName(Song song)
        {
            text.SetText(song.musicName);
        }
        
        private void UpdateMusicName(string musicName)
        {
            text.SetText(musicName);
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener<Song>(EventType.SetUpMusicInfo, UpdateMusicName);
        }
    }
}