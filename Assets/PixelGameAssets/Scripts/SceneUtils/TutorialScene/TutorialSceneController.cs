using System;
using System.Collections;
using PixelGameAssets.Scripts.Actor.Enemies;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Damage;
using PixelGameAssets.Scripts.Entity.InteractableEntity;
using PixelGameAssets.Scripts.Entity.InteractableEntity.Pickup;
using PixelGameAssets.Scripts.Misc;
using PixelGameAssets.Scripts.UI.UICanvas;
using PixelGameAssets.Scripts.WeaponSystem.WeaponInfo;
using UnityEngine;

namespace PixelGameAssets.Scripts.SceneUtils.TutorialScene
{
    public class TutorialSceneController : MonoBehaviour
    {
        public GameObject skeleton;

        /// <summary>
        /// Pickup
        /// </summary>
        public GameObject gun;

        public GameObject transferScene;

        private string currentStep = "none";
        
        private IEnumerator Start()
        {
            GameManager.Instance.Player.StopControl();

            yield return new WaitForSeconds(2.5f);
            UiManager.Instance.ShowMessageOnUi("欢迎来到教程关卡");

            yield return new WaitForSeconds(2.5f);
            UiManager.Instance.ShowMessageOnUi("在这里你能学到最基础的生存方式");

            yield return new WaitForSeconds(2.5f);
            UiManager.Instance.ShowMessageOnUi("你试着走走看, WASD 是你的移动方式");

            GameManager.Instance.Player.ResumeControl();

            yield return new WaitForSeconds(5f);
            UiManager.Instance.ShowMessageOnUi("在地牢里, 你不可避免地要学会攻击");

            GameManager.Instance.Player.StopControl();

            yield return new WaitForSeconds(2.5f);
            UiManager.Instance.ShowMessageOnUi("那里有把枪, 去吧按下 F 键, 鼠标左键攻击");

            var currGun = Instantiate(gun, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
            if (currGun.GetComponent<WeaponPickup>().wepRes.weaponData is GunInfo gunInfo)
            {
                //TODO: 修改合适的值
            }

            GameManager.Instance.Player.ResumeControl();
            
            yield return new WaitForSeconds(5f);
            UiManager.Instance.ShowMessageOnUi("是的就是这么残酷, 无论是你的处境还是你的武器都很残酷");

            yield return new WaitForSeconds(2.5f);
            UiManager.Instance.ShowMessageOnUi("接下来是实战了, 能否生存下来就看你了");

            var sk = Instantiate(skeleton, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
            var skHealth = sk.GetComponent<Health>();
            skHealth.health = 40;

            currentStep = "Attack";

            while (skHealth.health != 0)
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(2f);
            UiManager.Instance.ShowMessageOnUi("恭喜你迈出了第一步, 不过今后还有更强大的怪物等着你, 祝你好运");
            
            yield return new WaitForSeconds(2f);
            UiManager.Instance.ShowMessageOnUi("哦不, 等等, Q, E, Space 这三个键你可以试试哦");
            yield return new WaitForSeconds(3f);
            transferScene.SetActive(true);
            
            PlayerPrefs.SetString(ConstKeys.HasEnterTutorial, "true");
        }


        private void Update()
        {
            if (!currentStep.Equals("Attack") || GameManager.Instance.Player.Health.health >= 2) return;
            GameManager.Instance.Player.Health.TakeHeal(5);
            UiManager.Instance.ShowMessageOnUi("你在搞什么?");
        }
    }
}