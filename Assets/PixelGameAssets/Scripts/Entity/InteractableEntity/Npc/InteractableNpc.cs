using System;
using System.Collections.Generic;
using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.Core.Audio;
using PixelGameAssets.Scripts.GKUtils;
using PixelGameAssets.Scripts.Misc;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace PixelGameAssets.Scripts.Entity.InteractableEntity.Npc
{
    public class InteractableNpc : Interactable
    {
        // 对话内容
        protected readonly List<Statement> Content = new List<Statement>();

        // 对话音频内容
        protected readonly List<string> NpcVoiceName = new List<string>();

        protected GameObject Dialog;

        [Header("Custom")]
        // npc名称
        public string npcName = "None";

        /// <summary>
        /// 语气, 情感
        /// </summary>
        public enum Modal
        {
            Normal
        }

        /// <summary>
        /// 语句
        /// </summary>
        public class Statement
        {
            // 语气
            public readonly Modal Modal;

            // 对话内容
            public readonly string Content;

            public Statement(Modal modal, string content)
            {
                Modal = modal;
                Content = content;
            }
        }

        /// <summary>
        /// 必须回调该方法 base.Awake();
        /// </summary>
        protected void Awake()
        {
            spawnsPopup = false;
            oneTimes = false;
            entityName = npcName;
        }

        protected new void Update()
        {
            base.Update();
        }

        protected void ShowDialog()
        {
            if (Dialog != null) HideDialog();

            var pos = transform.position;
            pos.y += 20;
            Dialog = Instantiate(ResourceLoader.NormalDialogRes, pos, Quaternion.Euler(Vector3.zero));
            Dialog.transform.localScale = new Vector3(0.7f,0.7f);
            var dialogSpriteRenderer = Dialog.GetComponent<Canvas>();
            dialogSpriteRenderer.sortingLayerName = "FX";
//            var npcSpriteRender = GameObject.Find("SpriteHolder").GetComponent<SpriteRenderer>();
//            dialogSpriteRenderer.sortingOrder = npcSpriteRender.sortingOrder;
//            GkLog.Debug("npc", npcSpriteRender.sortingOrder);
//            GkLog.Debug("dialog", dialogSpriteRenderer.sortingOrder);
            var text = Dialog.GetComponentInChildren<TextMeshProUGUI>();
            text.text = Content[new Random().Next(0, Content.Count)].Content;
            PlayRandomVoice();
        }

        protected void PlayRandomVoice()
        {
            if (NpcVoiceName.Count == 0) return;
            AudioManager.Instance.PlayNpcSoundEffect(NpcVoiceName[new Random().Next(0, NpcVoiceName.Count)]);
        }

        protected void PlayVoice(string voiceName)
        {
            if (NpcVoiceName.Count == 0) return;
            AudioManager.Instance.PlayNpcSoundEffect(voiceName);
        }

        protected void HideDialog()
        {
            Destroy(Dialog);
        }

        /// <summary>
        /// 检测进入碰撞
        /// </summary>
        /// <param name="col">碰撞的物体</param>
        private void OnTriggerEnter2D(Collider2D col)
        {
            OnTriggerEnter2DFunc(col);
        }

        /// <summary>
        /// 检测退出碰撞
        /// </summary>
        /// <param name="col">碰撞的物体</param>
        private void OnTriggerExit2D(Collider2D col)
        {
            OnTriggerExit2DFunc(col);
        }

        protected override void OnPlayerEnter(BasePlayer commonPlayer)
        {
            base.OnPlayerEnter(commonPlayer);
            ShowDialog();
        }

        protected override void OnPlayerExit(BasePlayer commonPlayer)
        {
            base.OnPlayerExit(commonPlayer);
            HideDialog();
        }
    }
}