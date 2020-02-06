using System.Collections.Generic;
using PixelGameAssets.Scripts.Core;
using Unity.Collections;
using UnityEngine.SceneManagement;

namespace PixelGameAssets.Scripts.SceneUtils
{
    /// <summary>
    /// Scene 加载工具类
    /// Author: chenlongcould
    /// </summary>
    public static class SceneUtil
    {
        /// <summary>
        /// 所有场景集合
        /// </summary>
        public static class Scenes
        {
            public const string TownScene = "TownScene";
            public const string ForestScene = "ForestScene";
            public const string MainScene = "MainScene";
            public const string TutorialScene = "TutorialScene";
            public const string VideoScene = "VideoScene";
            public const string SplashScene = "SplashScene";
            public const string RandomScene = "RandomScene";
            public const string AboutScene = "AboutScene";
        }
        
        public static string NextSceneName;

        public static int GetCurrentSceneId()
        {
            //获取场景所在序号
            return SceneManager.GetActiveScene().buildIndex;
        }

        /// <summary>
        /// 获取场景名称
        /// </summary>
        /// <returns>Scene Name</returns>
        public static string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        public static void LoadSceneWithLoading(ref string sceneName)
        {
            if (sceneName == null || sceneName.Equals("")) return;
            NextSceneName = sceneName;
            SceneManager.LoadScene("LoadingScene");
        }

        /// <summary>
        /// 重新开始当前关卡
        /// </summary>
        public static void RestartCurrentScene()
        {
            var sceneName = GetCurrentSceneName();
            LoadSceneWithLoading(ref sceneName);
        }

        /// <summary>
        /// 回到城镇
        /// </summary>
        public static void BackToTown()
        {
            var sceneName = Scenes.TownScene;
            LoadSceneWithLoading(ref sceneName);
        }

        public static void BackToMainMenu()
        {
            var sceneName = Scenes.MainScene;
            FadeTransition.Instance.TransitionScene(sceneName, 1, 1);
        }

        public static bool IsGameScene(string scenesName)
        {
            bool val = false;
            switch (scenesName)
            {
                case Scenes.TownScene:
                case Scenes.ForestScene:
                case Scenes.RandomScene:
                    val = true;
                    break;
                case Scenes.AboutScene:
                case Scenes.MainScene:
                case Scenes.SplashScene:
                case Scenes.TutorialScene:
                case Scenes.VideoScene:
                    val = false;
                    break;
            }

            return val;
        }
    }
}