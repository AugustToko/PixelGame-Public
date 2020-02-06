using System;
using System.Reflection;
using PixelGameAssets.Scripts.Helper;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

namespace PixelGameAssets.Scripts.Core
{
    public class Urp : MonoBehaviour
    {
        [SerializeField] private UniversalRenderPipelineAsset renderPipelineAsset;

        [SerializeField] private Renderer2DData renderer2DData;

        public static Urp Instance;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
            SetupRenderScale(PlayerPrefs.GetFloat(PrefsHelper.GameSettings.RenderQuality, 1f));
            SetupLightQuality(PlayerPrefs.GetFloat(PrefsHelper.GameSettings.LightQuality, 1f));
        }

        public void SetupRenderScale(float val)
        {
            if (val < 0 || val > 1)
            {
                return;
            }

            renderPipelineAsset.renderScale = val;
        }

        public void SetupLightQuality(float val)
        {
            if (val < 0 || val > 1)
            {
                return;
            }

            var blendStyle = renderer2DData.lightBlendStyles[0];
            var t = blendStyle.GetType();
            foreach (var fieldInfo in t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (!"renderTextureScale".Equals(fieldInfo.Name)) continue;
                fieldInfo.SetValue(blendStyle, val);
                break;
            }
        }
    }
}