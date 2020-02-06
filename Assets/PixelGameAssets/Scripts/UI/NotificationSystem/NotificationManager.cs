using System;
using System.Collections;
using System.Collections.Generic;
using PixelGameAssets.MonsterLove.StateMachine;
using PixelGameAssets.Scripts.GKUtils;
using UnityEngine;
using EventType = PixelGameAssets.Scripts.EventManager.EventType;

namespace PixelGameAssets.Scripts.UI.NotificationSystem
{
    /// <summary>
    /// 通知管理
    /// </summary>
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance;

        /// <summary>
        /// 存储正在显示的通知
        /// </summary>
        private List<Notification> _notifications;

        /// <summary>
        /// 通知播放完毕, 处于 Dismiss 状态时, 进入回收列表
        /// </summary>
        private List<Notification> _bin;

        public float cooldown = 2.0f;

        /// <summary>
        /// 当出现多条通知时，其中的间距
        /// </summary>
        private const float NotificationSpacing = 100f;

        private enum Status
        {
            Enter,
            Exit
        }

        private StateMachine<Status> _fsm;

        private void Awake()
        {
            Instance = this;
            _notifications = new List<Notification>();
            _bin = new List<Notification>();
            _fsm = StateMachine<Status>.Initialize(this);
            
            EventManager.EventCenter.AddListener<Notification>(EventType.ShowNotification, Show);
        }

        private void Update()
        {
            for (var index = 0; index < _bin.Count; index++)
            {
                var notification = _bin[index];

                if (!(notification != null && notification.Animator != null &&
                      notification.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)) continue;

                _bin.Remove(notification);
                Destroy(notification.Body);
            }

            if (_notifications.Count == 0)
            {
                return;
            }

            for (var index = 0; index < _notifications.Count; index++)
            {
                var notification = _notifications[index];
                notification.CanvasGroup.alpha += 1f;
                if (notification.CanvasGroup.alpha < 1)
                {
                    continue;
                }

                if (notification.Timer > 0f)
                {
                    notification.Timer -= Time.deltaTime;
                    continue;
                }

                notification.Dismiss();
                _bin.Add(notification);
                _notifications.Remove(notification);
            }
        }

        /// <summary>
        /// 显示通知
        /// </summary>
        /// <param name="notification"></param>
        private void Show(Notification notification)
        {
            if (_notifications.Count != 0)
            {
                foreach (var n in _notifications)
                {
                    var pos = n.RectTransform.position;
                    n.RectTransform.position = new Vector3(pos.x, pos.y - NotificationSpacing);
                }
            }

            _notifications.Add(notification);
        }

        private IEnumerator Enter_Enter()
        {
            yield return null;
            _fsm.ChangeState(Status.Exit, StateTransition.Overwrite);
        }

        private IEnumerator Enter_Exit()
        {
            yield return null;
        }

        private void OnDestroy()
        {
            foreach (var n in _bin)
            {
                Destroy(n);
            }

            foreach (var notification in _notifications)
            {
                Destroy(notification);
            }

            _bin.Clear();
            _notifications.Clear();
            
            EventManager.EventCenter.RemoveListener<Notification>(EventType.ShowNotification, Show);
        }
    }
}