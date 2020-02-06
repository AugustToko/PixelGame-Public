using System;
using System.Collections;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.SceneUtils;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace PixelGameAssets.Scripts.UI.MainScene.ObsoleteScript
{
    [Obsolete]
    public class MainUi : MonoBehaviour
    {
        public static bool IsMainTitleAnimDone;

        public string targetScene = GameManager.TownSceneName;

        public TextMeshProUGUI startNotice;

        public Animator noticeScaleAlphaOut;

        public TextMeshProUGUI versionInfo;

        /// <summary>
        /// 检查更新是否完成
        /// </summary>
        private bool _checkVersionDone = false;

        private void Start()
        {

            startNotice.text = "Checking version...";
            versionInfo.text = "Version: " + Application.version;

            StartCoroutine(GetCloudVersion());
        }

        /// <summary>
        /// 获取云端版本
        /// </summary>
        /// <returns>IEnumerator</returns>
        private IEnumerator GetCloudVersion()
        {
            var webRequest = UnityWebRequest.Get("https://www.crypto-studio.com/?project=pixelproject&version=last");

            yield return webRequest.SendWebRequest();

            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                startNotice.text = "Checking version error.";
                _checkVersionDone = true;
            }
            else
            {
                var remoteVersion = webRequest.downloadHandler.text;
                if (remoteVersion.Equals(Application.version))
                {
                    startNotice.text = "Press any key to start";
                    _checkVersionDone = true;
                }
                else
                {
                    startNotice.fontSize = 20;
                    startNotice.text = "The current version does not match the remote version";

                    var dialogObj = Instantiate(ResourceLoader.Dialog, transform);
                    var dialog = dialogObj.GetComponent<Dialog>();
                    dialog.dialogTitle.text = "发现新版本";
                    dialog.buttonLeft.name = "DownLoad";
                    dialog.buttonRight.name = "Skip";

                    dialog.buttonLeft.onClick.AddListener(() =>
                    {
                        Application.OpenURL("https://gikode.itch.io/legendarist");
                        dialog.Dismiss();
                    });

                    dialog.buttonRight.onClick.AddListener(() =>
                    {
                        _checkVersionDone = true;
                        dialog.Dismiss();
                    });

                    dialog.Show();
                }
            }
        }

        private void Update()
        {
            if (IsMainTitleAnimDone && _checkVersionDone && Input.anyKeyDown)
            {
                StartCoroutine(nameof(Click));
            }
        }

        private IEnumerator Click()
        {
            IsMainTitleAnimDone = false;
            AudioManager.Instance.PlayAudioEffectHit("start_click");
            noticeScaleAlphaOut.Play("Scale_Alpha_Out");

            AudioManager.Instance.PlayBgm("Prelude to war");

            FadeTransition.Instance.TransitionScene(
                PlayerPrefs.GetString(ConstKeys.HasEnterTutorial, "false").Equals("false")
                    ? SceneUtil.Scenes.TutorialScene
                    : SceneUtil.Scenes.TownScene, 1, 1);
                
            var uprGo = GameObject.Find("URP");
            if (uprGo != null)
            {
                uprGo.GetComponent<Urp>().SetupRenderScale(0.5f);
            }
            yield break;
        }
    }
}