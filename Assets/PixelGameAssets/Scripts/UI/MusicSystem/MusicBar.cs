using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace PixelGameAssets.Scripts.UI.MusicSystem
{
    /// <summary>
    /// 音乐控制
    /// <see cref="AudioManager"/>
    /// </summary>
    public class MusicBar : MonoBehaviour
    {
        // 当前歌曲
        public Song currentSong;

        // 进度条
        public Slider slider;
        
        public TextMeshProUGUI musicName;

        public TextMeshProUGUI musicArtist;

        // 封面
        public Image musicCover;
        
        public bool isPlaying = false;

        // 播放模式
        public PlayMode currentMode = PlayMode.RepeatList;
        
        public enum PlayMode
        {
            /// <summary>
            /// 列表循环
            /// </summary>
            RepeatList,

            /// <summary>
            /// 单曲循环
            /// </summary>
            RepeatOne,

            /// <summary>
            /// 随机播放
            /// </summary>
            Random
        }

        public void SetUpMusic(ref Song song)
        {
            currentSong = song;

            musicName.text = song.musicName;
            musicArtist.text = song.musicArtist;
            // TODO: 设置音乐 Cover 背景
        }

        public void TogglePlayPause()
        {
            isPlaying = !isPlaying;
            if (isPlaying)
            {
                // TODO: 播放歌曲
                AudioManager.Instance.BgmResume();
            }
            else
            {
                // TODO: 暂停歌曲
                AudioManager.Instance.BgmStop();
            }
        }

        public void Next()
        {
            AudioManager.Instance.Next();
        }

        public void Previous()
        {
            AudioManager.Instance.Previous();
        }
    }
}