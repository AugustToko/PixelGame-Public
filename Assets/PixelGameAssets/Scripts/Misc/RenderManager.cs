using UnityEngine;
using UnityEngine.Rendering;

namespace PixelGameAssets.Scripts.Misc
{
    public class RenderManager : MonoBehaviour
    {
        public static RenderManager Instance;

        [SerializeField] private RenderPipelineAsset renderPipelineAsset;

        private RenderPipelineAsset _defRenderPipelineAsset;

        private void Awake()
        {
            Instance = this;

            _defRenderPipelineAsset = GraphicsSettings.currentRenderPipeline;
            DontDestroyOnLoad(this);
        }

        public void SwitchTo2D()
        {
            GraphicsSettings.defaultRenderPipeline = renderPipelineAsset;
            GraphicsSettings.renderPipelineAsset = renderPipelineAsset;
        }

        public void SwitchToDefault()
        {
            GraphicsSettings.renderPipelineAsset = _defRenderPipelineAsset;
            GraphicsSettings.defaultRenderPipeline = _defRenderPipelineAsset;
        }
    }
}