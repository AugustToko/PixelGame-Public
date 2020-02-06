using System;
using System.Collections;
using System.Linq;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.Map;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EventType = PixelGameAssets.Scripts.EventManager.EventType;

namespace PixelGameAssets.Scripts.SceneUtils
{
    /// <summary>
    /// 异步加载 Scene
    /// TODO: 修复卡顿, 重置架构
    /// </summary>
    public class LoadAsyncScene : MonoBehaviour
    {
        private AsyncOperation _async;

        //显示进度的文本
        private Text _progress;

        //进度条的数值
        private float _progressValue;

        //进度条
        private Slider _slider;

        private void Start()
        {
            if (SceneUtil.NextSceneName == null || SceneUtil.NextSceneName.Equals(""))
            {
                Destroy(this);
                Destroy(gameObject);
                return;
            }

            _progress = GetComponent<Text>();
            _slider = FindObjectOfType<Slider>();

            // 随机地图单独判断
            if (SceneUtil.NextSceneName.Contains("Random"))
            {
                StartCoroutine(LoadRandomSceneDarkDebug("RandomScene"));

//                SceneUtil.NextSceneName += GameManager.RandomSceneType;

//                if (SceneUtil.NextSceneName.Equals("RandomSceneA"))
//                {
//                }
//                else if (SceneUtil.NextSceneName.Equals("RandomSceneB"))
//                {
//                    StartCoroutine(LoadRandomSceneSkyCity("RandomScene"));
//                }
            }
            else
            {
                StartCoroutine(LoadScene(SceneUtil.NextSceneName));
            }
        }

//        /// <summary>
//        /// RandomSceneA
//        /// 普通地牢
//        /// </summary>
//        /// <param name="sceneName"></param>
//        /// <returns></returns>
//        private IEnumerator LoadRandomSceneDungeon(string sceneName)
//        {
//            GameManager.RandomSceneType = "A";
//
//            EventCenter.Broadcast(EventType.BeginLoadScene);
//
//            _async = SceneManager.LoadSceneAsync(sceneName);
//
//            _async.allowSceneActivation = false;
//
//            var can = false;
//
//            TilemapLevelLite.Step = 5000;
//
//            var helper = ResourceLoader.RandomMapTypeDungeon.GetComponent<RandomMapHelper>();
//
//            MapBuilder.InitBuilder(ref helper, ref TilemapLevelLite.PlayerRoomBuilder);
//            MapBuilder.InitBuilder(ref helper, ref TilemapLevelLite.Builder);
//
//            var task = TilemapLevelLite.Builder.MakeMapData(() =>
//            {
//                var maxV3 = Vector3Int.zero;
//
//                foreach (var v3 in TilemapLevelLite.Builder.WallTop)
//                {
//                    maxV3 = Vector3Int.Max(v3, maxV3);
//                }
//
//                // 偏移
//                maxV3 += new Vector3Int(50, 50, 0);
//
//                var task2 = TilemapLevelLite.PlayerRoomBuilder.MakeMapData(
//                    () =>
//                    {
//                        TilemapLevelLite.TransferPosPlayerRoom =
//                            TilemapLevelLite.PlayerRoomBuilder.GetRandomPosOnRoad();
//
//                        TilemapLevelLite.PlayerPos = TilemapLevelLite.PlayerRoomBuilder.GetRandomPosOnRoad();
//
//                        TilemapLevelLite.TransferPosMainRoom = TilemapLevelLite.Builder.GetRandomPosOnRoad();
//
//                        can = true;
//                    },
//                    maxV3, 200);
//            }, Vector3Int.zero, TilemapLevelLite.Step);
//
//            while (!_async.isDone)
//            {
//                _progressValue = _async.progress < 0.9f ? _async.progress : 1.0f;
//
//                _slider.value = _progressValue;
//                _progress.text = (int) (_slider.value * 100) + " %";
//
//                if (_progressValue >= 0.9 && can)
//                {
//                    _slider.value = 1.0f;
//                    _progress.text = "按任意键继续";
//                    if (Input.anyKeyDown)
//                    {
//                        Resources.UnloadUnusedAssets();
//                        GC.Collect();
//
//                        _async.allowSceneActivation = true;
//                        SceneUtil.NextSceneName = null;
//                        Destroy(this);
//                    }
//                }
//
//                yield return null;
//            }
//        }

//        /// <summary>
//        /// RandomSceneB 天空城
//        /// </summary>
//        /// <param name="sceneName"></param>
//        /// <returns></returns>
//        private IEnumerator LoadRandomSceneSkyCity(string sceneName)
//        {
//            GameManager.RandomSceneType = "B";
//
//            EventCenter.Broadcast(EventType.BeginLoadScene);
//
//            _async = SceneManager.LoadSceneAsync(sceneName);
//
//            _async.allowSceneActivation = false;
//
//            var can = false;
//
//            TilemapLevelLite.Step = 5000;
//
//            TilemapLevelLite.CameraBg = new Color32(100, 168, 255, 0);
//
//            TilemapLevelLite.After = () =>
//            {
//                for (int i = 0; i < UnityEngine.Random.Range(3, 5); i++)
//                {
//                    TilemapLevelLite.bgObj.Add(Instantiate(ResourceLoader.Cloud, Vector3.zero,
//                        Quaternion.Euler(Vector3.zero), UiManager.Instance.gameObject.transform));
//                }
//            };
//
//            var helper = ResourceLoader.RandomMapTypeDungeon.GetComponent<RandomMapHelper>();
//
//            MapBuilder.InitBuilder(ref helper, ref TilemapLevelLite.PlayerRoomBuilder);
//            MapBuilder.InitBuilder(ref helper, ref TilemapLevelLite.Builder);
//
//            var task = TilemapLevelLite.Builder.MakeMapData(() =>
//            {
//                var maxV3 = new Vector3Int(0, 0, 0);
//
//                maxV3 = TilemapLevelLite.Builder.WallTop.Aggregate(maxV3, (current, v3) => Vector3Int.Max(v3, current));
//
//                // 偏移
//                maxV3 += new Vector3Int(50, 50, 0);
//
//                var task2 = TilemapLevelLite.PlayerRoomBuilder.MakeMapData(
//                    () =>
//                    {
//                        TilemapLevelLite.TransferPosPlayerRoom =
//                            TilemapLevelLite.PlayerRoomBuilder.GetRandomPosOnRoad();
//
//                        TilemapLevelLite.PlayerPos = TilemapLevelLite.PlayerRoomBuilder.GetRandomPosOnRoad();
//
//                        TilemapLevelLite.TransferPosMainRoom = TilemapLevelLite.Builder.GetRandomPosOnRoad();
//
//                        can = true;
//                    },
//                    maxV3, 200);
//            }, Vector3Int.zero, TilemapLevelLite.Step);
//
//            while (!_async.isDone)
//            {
//                _progressValue = _async.progress < 0.9f ? _async.progress : 1.0f;
//
//                _slider.value = _progressValue;
//                _progress.text = (int) (_slider.value * 100) + " %";
//
//                if (_progressValue >= 0.9 && can)
//                {
//                    _slider.value = 1.0f;
//                    _progress.text = "按任意键继续";
//                    if (Input.anyKeyDown)
//                    {
//                        Resources.UnloadUnusedAssets();
//                        GC.Collect();
//
//                        _async.allowSceneActivation = true;
//                        SceneUtil.NextSceneName = null;
//                        Destroy(this);
//                    }
//                }
//
//                yield return null;
//            }
//        }

        /// <summary>
        /// RandomSceneB 天空城
        /// </summary>
        /// <param name="sceneName">SceneName</param>
        /// <returns>IEnumerator</returns>
        private IEnumerator LoadRandomSceneDarkDebug(string sceneName)
        {
            GameManager.RandomSceneType = "A";

            EventCenter.Broadcast(EventType.BeginLoadScene);

            _async = SceneManager.LoadSceneAsync(sceneName);
            _async.allowSceneActivation = false;
            var loadMapDataDone = false;

            // 规模
            const int bigMapStep = 5000;
            const int playerPointMapStep = 200;

            var helper = ResourceLoader.RandomMapTypeDungeon.GetComponent<RandomMapHelper>();

            MapBuilder.InitBuilder(ref helper, ref TilemapLevelLite.PlayerRoomBuilder);
            MapBuilder.InitBuilder(ref helper, ref TilemapLevelLite.Builder);

            yield return TilemapLevelLite.Builder.MakeMapData(async () =>
            {
                var maxV3 = new Vector3Int(0, 0, 0);

                maxV3 = TilemapLevelLite.Builder.WallTop.Aggregate(maxV3, (current, v3) => Vector3Int.Max(v3, current));

                // 偏移
                maxV3 += new Vector3Int(50, 50, 0);

                await TilemapLevelLite.PlayerRoomBuilder.MakeMapData(
                    () =>
                    {
                        TilemapLevelLite.TransferPosPlayerRoom =
                            TilemapLevelLite.PlayerRoomBuilder.GetRandomPosOnRoad();

                        TilemapLevelLite.PlayerPos = TilemapLevelLite.PlayerRoomBuilder.GetRandomPosOnRoad();

                        TilemapLevelLite.TransferPosMainRoom = TilemapLevelLite.Builder.GetRandomPosOnRoad();

                        loadMapDataDone = true;
                    },
                    // 小房间规模 smallMapStep
                    maxV3, playerPointMapStep);

                // 大房间规模 bigMapStep
            }, Vector3Int.zero, bigMapStep);

            TilemapLevelLite.After = () =>
            {
                for (var i = 0; i < UnityEngine.Random.Range(3, 5); i++)
                {
                    TilemapLevelLite.bgObj.Add(Instantiate(ResourceLoader.Cloud, Vector3.zero,
                        Quaternion.Euler(Vector3.zero), UiManager.Instance.gameObject.transform));
                }

                GameObject.Find("GlobalLight").GetComponent<Light2D>().intensity = 0.2f;
            };

            while (!_async.isDone)
            {
                _progressValue = _async.progress < 0.9f ? _async.progress : 1.0f;

                _slider.value = _progressValue;
                _progress.text = (int) (_slider.value * 100) + " %";

                if (_progressValue >= 0.9 && loadMapDataDone)
                {
                    _slider.value = 1.0f;
                    _progress.text = "按任意键继续";

                    if (Input.anyKeyDown)
                    {
                        SceneUtil.NextSceneName = "";
                        _async.allowSceneActivation = true;
                        Resources.UnloadUnusedAssets();
                        GC.Collect();
                        Destroy(this);
                        Destroy(gameObject);
                    }
                }

                yield return null;
            }
        }

        /// <summary>
        /// 常规加载
        /// </summary>
        /// <param name="sceneName">sceneName</param>
        /// <returns>IEnumerator</returns>
        private IEnumerator LoadScene(string sceneName)
        {
            GameManager.RandomSceneType = "";

            EventCenter.Broadcast(EventType.BeginLoadScene);

            _async = SceneManager.LoadSceneAsync(sceneName);

            _async.allowSceneActivation = false;

            while (!_async.isDone)
            {
                _progressValue = _async.progress < 0.9f ? _async.progress : 1.0f;

                _slider.value = _progressValue;

                _progress.text = (int) (_slider.value * 100) + " %";

                if (_progressValue >= 0.9)
                {
                    _slider.value = _slider.maxValue;
                    _progress.text = "按任意键继续";
                    if (Input.anyKeyDown)
                    {
                        SceneUtil.NextSceneName = "";
                        _async.allowSceneActivation = true;
                        Destroy(this);
                        Destroy(gameObject);
                    }
                }

                yield return null;
            }
        }
    }
}