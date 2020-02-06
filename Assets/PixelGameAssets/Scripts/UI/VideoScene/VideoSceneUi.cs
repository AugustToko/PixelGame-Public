using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.GKUtils;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.SceneUtils;
using UnityEngine;
using UnityEngine.Video;

namespace PixelGameAssets.Scripts.UI.VideoScene
{
    /// <summary>
    /// VideoScene Ui 管理
    /// </summary>
    public class VideoSceneUi : MonoBehaviour
    {
        public FadeText text;

        public VideoPlayer videoPlayer;

        private void Start()
        {
            videoPlayer.loopPointReached += source => { Done(); };
        }

        private void Update()
        {
//            if (Input.anyKeyDown && !text.showed) text.Show();
//            if (Input.anyKeyDown && text.showed && !_loadScene)
//            {
//                Done();
//            }
            if (Input.anyKeyDown)
            {
                Done();
            }
        }

        private void Done()
        {
            videoPlayer.Stop();

            PlayerPrefs.SetString(ConstKeys.HasEnterVideoOp1, "true");

#if UNITY_EDITOR
            PlayerPrefs.SetString(ConstKeys.HasEnterTutorial, "false");
#endif

            FadeTransition.Instance.TransitionScene(
                PlayerPrefs.GetString(ConstKeys.HasEnterTutorial, "false").Equals("false")
                    ? SceneUtil.Scenes.TutorialScene
                    : SceneUtil.Scenes.TownScene, 1, 1);

            AudioManager.Instance.PlayBgm("Prelude to war");
        }
    }
}