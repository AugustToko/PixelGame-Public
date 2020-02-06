using System;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.Entity;
using PixelGameAssets.Scripts.Entity.InteractableEntity;
using PixelGameAssets.Scripts.SceneUtils;
using PixelGameAssets.Scripts.UI;
using PixelGameAssets.Scripts.UI.MainScene;
using PixelGameAssets.Scripts.UI.MainScene.ObsoleteScript;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelGameAssets.Scripts.Misc
{
    /// <summary>
    /// 动画回调
    /// </summary>
    public class AnimatorEvents : MonoBehaviour
    {
        public void DestroyMe()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// 销毁父物体
        /// </summary>
        public void DestroyParent()
        {
            Destroy(transform.parent.gameObject);
        }

        public void TriggerParentInteractableAction()
        {
            var comp = GetComponentInParent<Interactable>();

            if (comp != null) comp.TriggerAction();
        }

        /// <summary>
        /// 主页面主标题动画完成回调
        /// </summary>
        /// <param name="pram">参数</param>
        public void MainTextAnimDone(int pram)
        {
            MainUi.IsMainTitleAnimDone = true;
        }

        /// <summary>
        /// 闪屏Logo动画结束后回调
        /// </summary>
        /// <param name="pram">参数</param>
        public void SplashLogoAnimDone(int pram)
        {
            SceneManager.LoadScene("MainScene");
//            FadeTransition.instance.TransitionScene("MainScene", 2, 2);
        }

        /// <summary>
        /// 主页标题动画开始调用
        /// </summary>
        /// <param name="pram"></param>
        public void MainTextAnimStart(int pram)
        {
            if (AudioManager.Instance == null)
            {
                return;
            }
            AudioManager.Instance.PlayAudioEffectHit("logo_show");
        }

        /// <summary>
        /// 测试警告后调用
        /// </summary>
        /// <param name="pram"></param>
        [Obsolete("Use WarningSceneCallBack")]
        public void DebugWarningDone(int pram)
        {
            SceneManager.LoadScene("SplashScene");
        }
    }
}