using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.SceneUtils;
using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;
using UnityEngine.UI;

namespace PixelGameAssets.Scripts.UI.Menu
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance;

        [SerializeField] private Button goBackToTown;

        [SerializeField] private Button resumeButton;

        [SerializeField] private Button restartButton;

        [SerializeField] private Button exitButton;

        [SerializeField] private RectTransform rectTransform;

        [SerializeField] [Header("Animation")] private Animator animator;

        private void Awake()
        {
            Instance = this;

            var currentSceneName = SceneUtil.GetCurrentSceneName();

            goBackToTown.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            resumeButton.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            restartButton.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            exitButton.animator.updateMode = AnimatorUpdateMode.UnscaledTime;

            if (!currentSceneName.Contains(SceneUtil.Scenes.TownScene)
                && !currentSceneName.Contains(SceneUtil.Scenes.TutorialScene))
            {
//                goBackToTown.enabled = true;
//                var colorBlock = goBackToTown.colors;
//                colorBlock.normalColor = Color.white;
//                goBackToTown.colors = colorBlock;

                goBackToTown.interactable = true;
            }
            else
            {
                goBackToTown.interactable = false;

//                goBackToTown.enabled = false;
//                var colorBlock = goBackToTown.colors;
                // TODO: 使用更和谐的颜色
//                colorBlock.normalColor = Color.black;
//                goBackToTown.colors = colorBlock;
            }
        }

        /// <summary>
        /// for animation
        /// </summary>
        public void DestroyMenu()
        {
            Destroy(this);
            Destroy(gameObject);
        }

        /// <summary>
        /// 隐藏菜单
        /// </summary>
        public void HideMenu()
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MainMenuHide"))
            {
                animator.Play("MainMenuHide");
            }

            UiManager.Instance.shownMenu = false;
            GameManager.IsStop = false;
            Time.timeScale = 1;
            Instance = null;

            UiManager.Instance.SetPauseButtonVisible(true);

            VManager.Instance.SetBlur(0);
        }

        /// <summary>
        /// 显示菜单
        /// </summary>
        public void Show()
        {
            rectTransform.anchoredPosition = Vector2.zero;

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MainMenuShow"))
            {
                animator.Play("MainMenuShow");
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }

            VManager.Instance.SetBlur(100);
            
            GameManager.IsStop = true;
            Time.timeScale = 0;
            
//            var minimapRT = transform.parent.Find("MinimapCanvas").GetComponent<RectTransform>();
        }

        /// <summary>
        /// <inheritdoc cref="SceneUtil.BackToTown"/>
        /// </summary>
        public void BackToTown()
        {
            GameManager.IsStop = false;
            Time.timeScale = 1;
            HideMenu();
            SceneUtil.BackToTown();
        }

        /// <summary>
        /// <inheritdoc cref="SceneUtil.RestartCurrentScene"/>
        /// </summary>
        public void Restart()
        {
            HideMenu();
            Time.timeScale = 1;
            SceneUtil.RestartCurrentScene();
        }

        public void ExitGame()
        {
            HideMenu();
            GameManager.ExitGame();
        }

        public void Save()
        {
            HideMenu();
            GameManager.Instance.SavePlayerInfo();
        }

        public void Back2MainMenu()
        {
            HideMenu();
            SceneUtil.BackToMainMenu();
            // TODO: 减缓音量
            Destroy(AudioManager.Instance.gameObject);
        }

        public void Load()
        {
            HideMenu();
            GameManager.Instance.LoadPlayerInfo();
        }

        public void PlayHover()
        {
            AudioManager.Instance.AddToRandomFxSource("snd_momo_wood_fix_indark");
        }
    }
}