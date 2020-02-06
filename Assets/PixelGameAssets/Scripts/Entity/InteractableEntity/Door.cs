using System.Linq;
using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Camera;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.Damage;
using PixelGameAssets.Scripts.SceneUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelGameAssets.Scripts.Entity.InteractableEntity
{
    public class Door : Interactable
    {
        [Header("Animator")] public Animator animator;

        [Header("Door")] public bool opened;

        public string nextSceneName;

        private new void Update()
        {
            base.Update();
            // Check if all enemies have died 2 times per second (every 0.5f secs)
            InvokeRepeating(nameof(CheckForEnemiesToOpen), 1f, 1f);
        }

        /// <summary>
        /// 检查场景内敌人, 敌人全部被消灭后开门
        /// </summary>
        private void CheckForEnemiesToOpen()
        {
            if (opened || AreEnemiesAlive()) return;

            Pickable = true;
            opened = true;

            if (CameraShaker.Instance != null) CameraShaker.InitShake(0.2f, 1f);

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Open")) animator.Play("Open");
            AudioManager.Instance.PlayAmbientSoundEffect("DoorOpen");
        }

        /// <summary>
        /// 判断场景内敌人数量
        /// </summary>
        /// <returns>敌人是否存活</returns>
        private static bool AreEnemiesAlive()
        {
            var l = GameObject.FindGameObjectsWithTag("Enemies");
            var count = l.Count(o => !o.GetComponent<Health>().dead);
            return count > 0;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            OnTriggerEnter2DFunc(col);
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            OnTriggerStay2DFunc(col);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            OnTriggerExit2DFunc(col);
        }

        /// <summary>
        /// 玩家进入门
        /// </summary>
        /// <param name="commonPlayer">玩家</param>
        protected override void OnPlayerTrigger(BasePlayer commonPlayer)
        {
            if (!opened) return;
            SceneUtil.LoadSceneWithLoading(ref nextSceneName);
            base.OnPlayerTrigger(commonPlayer);
        }
    }
}