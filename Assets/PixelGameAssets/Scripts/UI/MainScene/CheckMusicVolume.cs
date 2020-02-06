using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.Helper;
using UnityEngine;
using UnityEngine.Serialization;

namespace PixelGameAssets.Scripts.UI.MainScene
{
    public class CheckMusicVolume : MonoBehaviour
    {

        [SerializeField]
        private AudioSource bgm;

        [SerializeField]
        private AudioSource sampleFx;

        [SerializeField]
        private AudioSource sampleVoice;
        
        public void Start()
        {
            // remember volume level from last time
            bgm.volume = PlayerPrefs.GetFloat(PrefsHelper.GameSettings.MusicVolume);
        }

        public void UpdateMusicVolume()
        {
            var vol = PlayerPrefs.GetFloat(PrefsHelper.GameSettings.MusicVolume);
            bgm.volume = vol;
            AudioManager.Instance.SetUpBgmVolume(vol);
        }

        public void UpdateFxVolume()
        {
            var vol = PlayerPrefs.GetFloat(PrefsHelper.GameSettings.FxVolume);
            sampleFx.volume = vol;
            sampleFx.Play();
        }
        
        public void UpdateVoiceVolume()
        {
            var vol = PlayerPrefs.GetFloat(PrefsHelper.GameSettings.VoiceVolume);
            sampleVoice.volume = vol;
            sampleVoice.Play();
        }
    }
}