using System.Collections;
using System.Diagnostics.CodeAnalysis;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PixelGameAssets.Scripts.SceneUtils
{
    /// <summary>
    /// 过度转场
    /// </summary>
    public class FadeTransition : MonoBehaviour
    {
        //fade句柄，在需要切换关卡的时候进行调用
        private static FadeTransition _instance;

        private Canvas _loadingCanvas;

        //需要用代码生成loading的黑屏image和canvas
        private Image _loadingImag;

        //下一关的名字，检查是否加载完毕，进入下一关
        private string _nextScene = "";

        private float _sceneStartTimeStamp;

        public static FadeTransition Instance
        {
            get
            {
                if (_instance) return _instance;
                // check if there is a TransitionKit instance already available in the scene graph before creating one
                _instance = FindObjectOfType(typeof(FadeTransition)) as FadeTransition;

                if (_instance) return _instance;
                //生成 fade object
                var obj = new GameObject("FadeTransition");
                _instance = obj.AddComponent<FadeTransition>();
                DontDestroyOnLoad(obj);

                return _instance;
            }
        }

        /// <summary>
        ///     Transitions the scene 函数（其他脚本调用）
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="pFadeOutDuration"></param>
        /// <param name="pFadeInDuration"></param>
        public void TransitionScene(string sceneName, float pFadeOutDuration, float pFadeInDuration)
        {
            _nextScene = sceneName;
            StartCoroutine(Transition(pFadeOutDuration, pFadeInDuration));
        }

        /// <summary>
        /// 渐出、加载场景、渐入的过程
        /// </summary>
        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private IEnumerator Transition(float pFadeOutDuration, float pFadeInDuration)
        {
            //先行清理
            ClearFadeTransitionContent();

            //set canvas
            var canvasObject = new GameObject("FadeTransitionCanvas");
            _loadingCanvas = canvasObject.AddComponent<Canvas>();
            _loadingCanvas.transform.SetParent(transform);
            _loadingCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _loadingCanvas.sortingOrder = 10;

            var imageObject = new GameObject("FadeTransitionImage");
            _loadingImag = imageObject.AddComponent<Image>();
            _loadingImag.transform.SetParent(_loadingCanvas.transform);
            _loadingImag.color = Color.clear;
            _loadingImag.rectTransform.anchoredPosition = Vector2.zero;
            _loadingImag.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

            //fade in
            yield return StartCoroutine(TickChangeAlpha(pFadeOutDuration));

            //waiting for loaded
            if (_nextScene != "")
            {
                //unity异步加载场景函数
                var asyncOperation = SceneManager.LoadSceneAsync(_nextScene);
                asyncOperation.completed += operation =>
                {
                    // TODO: 使用广播
                    if (UiManager.Instance != null)
                    {
                        // 加载完成后显示名称
                        UiManager.Instance.ShowSceneNameOnUi(SceneManager.GetActiveScene().name);
                    }

                    switch (_nextScene)
                    {
                        case SceneUtil.Scenes.AboutScene:
                        case SceneUtil.Scenes.MainScene:
                            if (RenderManager.Instance != null)
                            {
                                RenderManager.Instance.SwitchToDefault();
                            }
                            break;
                        case SceneUtil.Scenes.RandomScene:
                        case SceneUtil.Scenes.TownScene:
                            RenderManager.Instance.SwitchTo2D();
                            break;
                        default:
                            RenderManager.Instance.SwitchToDefault();
                            break;
                    }
                };
                yield return StartCoroutine(WaitForLevelToLoad(_nextScene)); //输出信息
            }

            // fade out
            yield return StartCoroutine(TickChangeAlpha(pFadeInDuration, true));

            ClearFadeTransitionContent();

            Destroy(this);
            Destroy(gameObject);
        }

        /// <summary>
        ///     清除已有的 fade object
        /// </summary>
        private void ClearFadeTransitionContent()
        {
            if (_loadingCanvas != null)
            {
                Destroy(_loadingCanvas.gameObject);
                Destroy(_loadingCanvas);
            }

            if (_loadingImag != null)
            {
                Destroy(_loadingImag.gameObject);
                Destroy(_loadingImag);
            }
        }

        private void OnDestroy()
        {
            ClearFadeTransitionContent();
        }

        /// <summary>
        ///     检查是否加载完毕，进入了下一关
        /// </summary>
        public static IEnumerator WaitForLevelToLoad(string level)
        {
            while (SceneManager.GetActiveScene().name != level)
            {
                yield return null;
            }
        }

        /// <summary>
        ///     image颜色渐变，用while循环来计算
        /// </summary>
        private IEnumerator TickChangeAlpha(float duration, bool reverseDirection = false)
        {
            var start = reverseDirection ? Color.black : Color.clear;
            var end = reverseDirection ? Color.clear : Color.black;

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var step = Color.Lerp(start, end, Mathf.Pow(elapsed / duration, 2f));
                _loadingImag.color = step;

                yield return null;
            }
        }
    }
}