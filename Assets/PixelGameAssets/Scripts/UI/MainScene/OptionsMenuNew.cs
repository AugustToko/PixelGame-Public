using System;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelGameAssets.Scripts.UI.MainScene
{
    /// <summary>
    /// TODO: 选项设置
    /// </summary>
    public class OptionsMenuNew : MonoBehaviour
    {
        public enum Platform
        {
            Desktop,
            Mobile
        };

        public Platform platform;

        // toggle buttons
        [Header("MOBILE SETTINGS")] public GameObject mobileSFXtext;
        public GameObject mobileMusictext;
        public GameObject mobileShadowofftextLINE;
        public GameObject mobileShadowlowtextLINE;
        public GameObject mobileShadowhightextLINE;

        [Header("VIDEO SETTINGS")] public GameObject fullscreentext;
        public GameObject ambientocclusiontext;
        public GameObject shadowofftextLINE;
        public GameObject shadowlowtextLINE;
        public GameObject shadowhightextLINE;
        public GameObject aaofftextLINE;
        public GameObject aa2xtextLINE;
        public GameObject aa4xtextLINE;
        public GameObject aa8xtextLINE;
        public GameObject vsynctext;
        public GameObject motionblurtext;
        public GameObject texturelowtextLINE;
        public GameObject texturemedtextLINE;
        public GameObject texturehightextLINE;
        public GameObject cameraeffectstext;

        [Header("GAME SETTINGS")] public GameObject showhudtext;
        public GameObject tooltipstext;
        public GameObject difficultynormaltext;
        public GameObject difficultynormaltextLINE;
        public GameObject difficultyhardcoretext;
        public GameObject difficultyhardcoretextLINE;

        [Header("CONTROLS SETTINGS")] public GameObject invertmousetext;

        // TODO: 使用 Slider 而非 GameObject (性能)

        #region Slider

        // sliders
        public GameObject musicSlider;
        public GameObject fxSlider;
        public GameObject voiceSlider;
        public GameObject sensitivityXSlider;
        public GameObject sensitivityYSlider;
        public GameObject mouseSmoothSlider;
        [SerializeField] private Slider renderSlider;
        [SerializeField] private Slider lightSlider;

        #endregion

        private float _musicSliderValue = 0.0f;
        private float _fxSliderValue = 0.0f;
        private float _voiceSliderValue = 0.0f;

        private float _sliderValueXSensitivity = 0.0f;
        private float _sliderValueYSensitivity = 0.0f;
        private float _sliderValueSmoothing = 0.0f;
        private float _renderSliderVal = 0.0f;
        private float _lightSliderVal = 0.0f;


        public void Start()
        {
            // check difficulty
            if (PlayerPrefs.GetInt("NormalDifficulty") == 1)
            {
                difficultynormaltextLINE.gameObject.SetActive(true);
                difficultyhardcoretextLINE.gameObject.SetActive(false);
            }
            else
            {
                difficultyhardcoretextLINE.gameObject.SetActive(true);
                difficultynormaltextLINE.gameObject.SetActive(false);
            }

            // check slider values
            // TODO: 消除魔法值
            musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat(PrefsHelper.GameSettings.MusicVolume, 0.7f);
            fxSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat(PrefsHelper.GameSettings.FxVolume, 0.7f);
            voiceSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat(PrefsHelper.GameSettings.VoiceVolume, 1f);
            renderSlider.value = PlayerPrefs.GetFloat(PrefsHelper.GameSettings.RenderQuality, 1f);
            lightSlider.value = PlayerPrefs.GetFloat(PrefsHelper.GameSettings.LightQuality, 1f);

            sensitivityXSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("XSensitivity");
            sensitivityYSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("YSensitivity");
            mouseSmoothSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MouseSmoothing");

            // check full screen
            if (Screen.fullScreen)
            {
                fullscreentext.GetComponent<TMP_Text>().text = "on";
            }
            else if (Screen.fullScreen == false)
            {
                fullscreentext.GetComponent<TMP_Text>().text = "off";
            }

            // check hud value
            showhudtext.GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("ShowHUD") == 0 ? "off" : "on";

            // check tool tip value
            tooltipstext.GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("ToolTips") == 0 ? "off" : "on";

            switch (platform)
            {
                // check shadow distance/enabled
                case Platform.Desktop when PlayerPrefs.GetInt("Shadows") == 0:
                    QualitySettings.shadowCascades = 0;
                    QualitySettings.shadowDistance = 0;
                    shadowofftextLINE.gameObject.SetActive(true);
                    shadowlowtextLINE.gameObject.SetActive(false);
                    shadowhightextLINE.gameObject.SetActive(false);
                    break;
                case Platform.Desktop when PlayerPrefs.GetInt("Shadows") == 1:
                    QualitySettings.shadowCascades = 2;
                    QualitySettings.shadowDistance = 75;
                    shadowofftextLINE.gameObject.SetActive(false);
                    shadowlowtextLINE.gameObject.SetActive(true);
                    shadowhightextLINE.gameObject.SetActive(false);
                    break;
                case Platform.Desktop:
                {
                    if (PlayerPrefs.GetInt("Shadows") == 2)
                    {
                        QualitySettings.shadowCascades = 4;
                        QualitySettings.shadowDistance = 500;
                        shadowofftextLINE.gameObject.SetActive(false);
                        shadowlowtextLINE.gameObject.SetActive(false);
                        shadowhightextLINE.gameObject.SetActive(true);
                    }

                    break;
                }
                case Platform.Mobile when PlayerPrefs.GetInt("MobileShadows") == 0:
                    QualitySettings.shadowCascades = 0;
                    QualitySettings.shadowDistance = 0;
                    mobileShadowofftextLINE.gameObject.SetActive(true);
                    mobileShadowlowtextLINE.gameObject.SetActive(false);
                    mobileShadowhightextLINE.gameObject.SetActive(false);
                    break;
                case Platform.Mobile when PlayerPrefs.GetInt("MobileShadows") == 1:
                    QualitySettings.shadowCascades = 2;
                    QualitySettings.shadowDistance = 75;
                    mobileShadowofftextLINE.gameObject.SetActive(false);
                    mobileShadowlowtextLINE.gameObject.SetActive(true);
                    mobileShadowhightextLINE.gameObject.SetActive(false);
                    break;
                case Platform.Mobile:
                {
                    if (PlayerPrefs.GetInt("MobileShadows") == 2)
                    {
                        QualitySettings.shadowCascades = 4;
                        QualitySettings.shadowDistance = 100;
                        mobileShadowofftextLINE.gameObject.SetActive(false);
                        mobileShadowlowtextLINE.gameObject.SetActive(false);
                        mobileShadowhightextLINE.gameObject.SetActive(true);
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (QualitySettings.vSyncCount)
            {
                // check vsync
                case 0:
                    vsynctext.GetComponent<TMP_Text>().text = "off";
                    break;
                case 1:
                    vsynctext.GetComponent<TMP_Text>().text = "on";
                    break;
            }

            // check mouse inverse
            if (PlayerPrefs.GetInt("Inverted") == 0)
            {
                invertmousetext.GetComponent<TMP_Text>().text = "off";
            }
            else if (PlayerPrefs.GetInt("Inverted") == 1)
            {
                invertmousetext.GetComponent<TMP_Text>().text = "on";
            }

            // check motion blur
            if (PlayerPrefs.GetInt("MotionBlur") == 0)
            {
                motionblurtext.GetComponent<TMP_Text>().text = "off";
            }
            else if (PlayerPrefs.GetInt("MotionBlur") == 1)
            {
                motionblurtext.GetComponent<TMP_Text>().text = "on";
            }

            // check ambient occlusion
            if (PlayerPrefs.GetInt("AmbientOcclusion") == 0)
            {
                ambientocclusiontext.GetComponent<TMP_Text>().text = "off";
            }
            else if (PlayerPrefs.GetInt("AmbientOcclusion") == 1)
            {
                ambientocclusiontext.GetComponent<TMP_Text>().text = "on";
            }

            // check texture quality
            if (PlayerPrefs.GetInt("Textures") == 0)
            {
                QualitySettings.masterTextureLimit = 2;
                texturelowtextLINE.gameObject.SetActive(true);
                texturemedtextLINE.gameObject.SetActive(false);
                texturehightextLINE.gameObject.SetActive(false);
            }
            else if (PlayerPrefs.GetInt("Textures") == 1)
            {
                QualitySettings.masterTextureLimit = 1;
                texturelowtextLINE.gameObject.SetActive(false);
                texturemedtextLINE.gameObject.SetActive(true);
                texturehightextLINE.gameObject.SetActive(false);
            }
            else if (PlayerPrefs.GetInt("Textures") == 2)
            {
                QualitySettings.masterTextureLimit = 0;
                texturelowtextLINE.gameObject.SetActive(false);
                texturemedtextLINE.gameObject.SetActive(false);
                texturehightextLINE.gameObject.SetActive(true);
            }

            if (PlayerPrefs.GetInt("CameraEffects") == 0)
            {
                cameraeffectstext.GetComponent<TMP_Text>().text = "on";
                Debug.Log("CameraEffects: On");
            }
            else if (PlayerPrefs.GetInt("CameraEffects") == 1)
            {
                cameraeffectstext.GetComponent<TMP_Text>().text = "off";
                Debug.Log("CameraEffects: Off");
            }
        }

        public void Update()
        {
            _musicSliderValue = musicSlider.GetComponent<Slider>().value;
            _fxSliderValue = fxSlider.GetComponent<Slider>().value;
            _voiceSliderValue = voiceSlider.GetComponent<Slider>().value;
            _renderSliderVal = renderSlider.GetComponent<Slider>().value;
            _lightSliderVal = lightSlider.GetComponent<Slider>().value;

            _sliderValueXSensitivity = sensitivityXSlider.GetComponent<Slider>().value;
            _sliderValueYSensitivity = sensitivityYSlider.GetComponent<Slider>().value;
            _sliderValueSmoothing = mouseSmoothSlider.GetComponent<Slider>().value;
        }

        public void FullScreen()
        {
            Screen.fullScreen = !Screen.fullScreen;

            if (Screen.fullScreen)
            {
                fullscreentext.GetComponent<TMP_Text>().text = "on";
            }
            else if (Screen.fullScreen == false)
            {
                fullscreentext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void MusicSlider()
        {
            PlayerPrefs.SetFloat(PrefsHelper.GameSettings.MusicVolume, _musicSliderValue);
        }

        public void FxSlider()
        {
            PlayerPrefs.SetFloat(PrefsHelper.GameSettings.FxVolume, _fxSliderValue);
        }

        public void VoiceSlider()
        {
            PlayerPrefs.SetFloat(PrefsHelper.GameSettings.VoiceVolume, _voiceSliderValue);
        }

        public void SensitivityXSlider()
        {
            PlayerPrefs.SetFloat("XSensitivity", _sliderValueXSensitivity);
        }

        public void SensitivityYSlider()
        {
            PlayerPrefs.SetFloat("YSensitivity", _sliderValueYSensitivity);
        }

        public void SensitivitySmoothing()
        {
            PlayerPrefs.SetFloat("MouseSmoothing", _sliderValueSmoothing);
            Debug.Log(PlayerPrefs.GetFloat("MouseSmoothing"));
        }

        public void RenderSlider()
        {
            PlayerPrefs.SetFloat(PrefsHelper.GameSettings.RenderQuality, _renderSliderVal);
            Urp.Instance.SetupRenderScale(_renderSliderVal);
        }

        public void LightSlider()
        {
            PlayerPrefs.SetFloat(PrefsHelper.GameSettings.LightQuality, _lightSliderVal);
            Urp.Instance.SetupLightQuality(_lightSliderVal);
        }

        // the playerprefs variable that is checked to enable hud while in game
        public void ShowHUD()
        {
            if (PlayerPrefs.GetInt("ShowHUD") == 0)
            {
                PlayerPrefs.SetInt("ShowHUD", 1);
                showhudtext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("ShowHUD") == 1)
            {
                PlayerPrefs.SetInt("ShowHUD", 0);
                showhudtext.GetComponent<TMP_Text>().text = "off";
            }
        }

        // the playerprefs variable that is checked to enable mobile sfx while in game
        public void MobileSFXMute()
        {
            if (PlayerPrefs.GetInt("Mobile_MuteSfx") == 0)
            {
                PlayerPrefs.SetInt("Mobile_MuteSfx", 1);
                mobileSFXtext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("Mobile_MuteSfx") == 1)
            {
                PlayerPrefs.SetInt("Mobile_MuteSfx", 0);
                mobileSFXtext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void MobileMusicMute()
        {
            if (PlayerPrefs.GetInt("Mobile_MuteMusic") == 0)
            {
                PlayerPrefs.SetInt("Mobile_MuteMusic", 1);
                mobileMusictext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("Mobile_MuteMusic") == 1)
            {
                PlayerPrefs.SetInt("Mobile_MuteMusic", 0);
                mobileMusictext.GetComponent<TMP_Text>().text = "off";
            }
        }

        // show tool tips like: 'How to Play' control pop ups
        public void ToolTips()
        {
            if (PlayerPrefs.GetInt("ToolTips") == 0)
            {
                PlayerPrefs.SetInt("ToolTips", 1);
                tooltipstext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("ToolTips") == 1)
            {
                PlayerPrefs.SetInt("ToolTips", 0);
                tooltipstext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void NormalDifficulty()
        {
            difficultyhardcoretextLINE.gameObject.SetActive(false);
            difficultynormaltextLINE.gameObject.SetActive(true);
            PlayerPrefs.SetInt("NormalDifficulty", 1);
            PlayerPrefs.SetInt("HardCoreDifficulty", 0);
        }

        public void HardcoreDifficulty()
        {
            difficultyhardcoretextLINE.gameObject.SetActive(true);
            difficultynormaltextLINE.gameObject.SetActive(false);
            PlayerPrefs.SetInt("NormalDifficulty", 0);
            PlayerPrefs.SetInt("HardCoreDifficulty", 1);
        }

        public void ShadowsOff()
        {
            PlayerPrefs.SetInt("Shadows", 0);
            QualitySettings.shadowCascades = 0;
            QualitySettings.shadowDistance = 0;
            shadowofftextLINE.gameObject.SetActive(true);
            shadowlowtextLINE.gameObject.SetActive(false);
            shadowhightextLINE.gameObject.SetActive(false);
        }

        public void ShadowsLow()
        {
            PlayerPrefs.SetInt("Shadows", 1);
            QualitySettings.shadowCascades = 2;
            QualitySettings.shadowDistance = 75;
            shadowofftextLINE.gameObject.SetActive(false);
            shadowlowtextLINE.gameObject.SetActive(true);
            shadowhightextLINE.gameObject.SetActive(false);
        }

        public void ShadowsHigh()
        {
            PlayerPrefs.SetInt("Shadows", 2);
            QualitySettings.shadowCascades = 4;
            QualitySettings.shadowDistance = 500;
            shadowofftextLINE.gameObject.SetActive(false);
            shadowlowtextLINE.gameObject.SetActive(false);
            shadowhightextLINE.gameObject.SetActive(true);
        }

        public void MobileShadowsOff()
        {
            PlayerPrefs.SetInt("MobileShadows", 0);
            QualitySettings.shadowCascades = 0;
            QualitySettings.shadowDistance = 0;
            mobileShadowofftextLINE.gameObject.SetActive(true);
            mobileShadowlowtextLINE.gameObject.SetActive(false);
            mobileShadowhightextLINE.gameObject.SetActive(false);
        }

        public void MobileShadowsLow()
        {
            PlayerPrefs.SetInt("MobileShadows", 1);
            QualitySettings.shadowCascades = 2;
            QualitySettings.shadowDistance = 75;
            mobileShadowofftextLINE.gameObject.SetActive(false);
            mobileShadowlowtextLINE.gameObject.SetActive(true);
            mobileShadowhightextLINE.gameObject.SetActive(false);
        }

        public void MobileShadowsHigh()
        {
            PlayerPrefs.SetInt("MobileShadows", 2);
            QualitySettings.shadowCascades = 4;
            QualitySettings.shadowDistance = 500;
            mobileShadowofftextLINE.gameObject.SetActive(false);
            mobileShadowlowtextLINE.gameObject.SetActive(false);
            mobileShadowhightextLINE.gameObject.SetActive(true);
        }

        public void vsync()
        {
            if (QualitySettings.vSyncCount == 0)
            {
                QualitySettings.vSyncCount = 1;
                vsynctext.GetComponent<TMP_Text>().text = "on";
            }
            else if (QualitySettings.vSyncCount == 1)
            {
                QualitySettings.vSyncCount = 0;
                vsynctext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void InvertMouse()
        {
            if (PlayerPrefs.GetInt("Inverted") == 0)
            {
                PlayerPrefs.SetInt("Inverted", 1);
                invertmousetext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("Inverted") == 1)
            {
                PlayerPrefs.SetInt("Inverted", 0);
                invertmousetext.GetComponent<TMP_Text>().text = "off";
            }
        }

        /// <summary>
        /// TODO: MotionBlur
        /// </summary>
        public void MotionBlur()
        {
            if (PlayerPrefs.GetInt("MotionBlur") == 0)
            {
                PlayerPrefs.SetInt("MotionBlur", 1);
                motionblurtext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("MotionBlur") == 1)
            {
                PlayerPrefs.SetInt("MotionBlur", 0);
                motionblurtext.GetComponent<TMP_Text>().text = "off";
            }
        }

        /// <summary>
        /// TODO: AmbientOcclusion
        /// </summary>
        public void AmbientOcclusion()
        {
            if (PlayerPrefs.GetInt("AmbientOcclusion") == 0)
            {
                PlayerPrefs.SetInt("AmbientOcclusion", 1);
                ambientocclusiontext.GetComponent<TMP_Text>().text = "on";
            }
            else if (PlayerPrefs.GetInt("AmbientOcclusion") == 1)
            {
                PlayerPrefs.SetInt("AmbientOcclusion", 0);
                ambientocclusiontext.GetComponent<TMP_Text>().text = "off";
            }
        }

        public void CameraEffects()
        {
            if (PlayerPrefs.GetInt("CameraEffects") == 0)
            {
                PlayerPrefs.SetInt("CameraEffects", 1);
                cameraeffectstext.GetComponent<TMP_Text>().text = "on";
                Debug.Log("CameraEffects on");
            }
            else if (PlayerPrefs.GetInt("CameraEffects") == 1)
            {
                PlayerPrefs.SetInt("CameraEffects", 0);
                cameraeffectstext.GetComponent<TMP_Text>().text = "off";
                Debug.Log("CameraEffects off");
            }
            else
            {
                Debug.Log("CameraEffects val: " + PlayerPrefs.GetInt("CameraEffects"));
            }
        }

        public void TexturesLow()
        {
            PlayerPrefs.SetInt("Textures", 0);
            QualitySettings.masterTextureLimit = 2;
            texturelowtextLINE.gameObject.SetActive(true);
            texturemedtextLINE.gameObject.SetActive(false);
            texturehightextLINE.gameObject.SetActive(false);
        }

        public void TexturesMed()
        {
            PlayerPrefs.SetInt("Textures", 1);
            QualitySettings.masterTextureLimit = 1;
            texturelowtextLINE.gameObject.SetActive(false);
            texturemedtextLINE.gameObject.SetActive(true);
            texturehightextLINE.gameObject.SetActive(false);
        }

        public void TexturesHigh()
        {
            PlayerPrefs.SetInt("Textures", 2);
            QualitySettings.masterTextureLimit = 0;
            texturelowtextLINE.gameObject.SetActive(false);
            texturemedtextLINE.gameObject.SetActive(false);
            texturehightextLINE.gameObject.SetActive(true);
        }
    }
}