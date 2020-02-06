using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.Misc;
using TMPro;
using UnityEngine;
using EventType = PixelGameAssets.Scripts.EventManager.EventType;

namespace PixelGameAssets.Scripts.UI.NotificationSystem
{
    /// <summary>
    /// 通知
    /// TODO: 待完善 
    /// </summary>
    public class Notification : Object
    {
        private readonly Transform _parent;

        public RectTransform RectTransform;

        public CanvasGroup CanvasGroup;

        public readonly Animator Animator;

        public readonly GameObject Body;

        public float Timer = 2.0f;

        private Notification(ref Builder builder)
        {
            Title = builder.Title;
            Content = builder.Content;
            _parent = builder.Parent;

            Body = Instantiate(ResourceLoader.NotificationRes, Vector3.zero, Quaternion.Euler(Vector3.zero), _parent);
            Body.GetComponentsInChildren<TextMeshProUGUI>()[0].text = Title;
            Body.GetComponentsInChildren<TextMeshProUGUI>()[1].text = Content;
            Animator = Body.GetComponent<Animator>();

            RectTransform = Body.GetComponent<RectTransform>();
            RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 150);
            RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 60, 60);

            CanvasGroup = Body.GetComponent<CanvasGroup>();
        }

        public string Title { get; set; }

        public string Content { get; set; }

        /// <summary>
        ///     展示通知
        /// </summary>
        public void Show()
        {
            if (NotificationManager.Instance != null)
            {
                var n = this;
                EventCenter.Broadcast(EventType.ShowNotification, n);
            }
        }

        public void Dismiss()
        {
            Animator.Play("Dismiss", 0, 0f);
        }

        public class Builder
        {
            public readonly string Content;
            public readonly Transform Parent = GameObject.Find("UI-Canvas").transform;
            public readonly string Title;

            /// <summary>
            ///     带 parent 构造
            /// </summary>
            /// <param name="parent">parent</param>
            /// <param name="title">标题</param>
            /// <param name="content">内容</param>
            public Builder(ref Transform parent, string title, string content)
            {
                Title = title;
                Content = content;
                Parent = parent;
            }

            /// <summary>
            ///     不带 parent 构造
            /// </summary>
            /// <param name="title">标题</param>
            /// <param name="content">内容</param>
            public Builder(string title, string content)
            {
                Title = title;
                Content = content;
            }

            public Notification Build()
            {
                var build = this;
                return new Notification(ref build);
            }

            public void Show()
            {
                Build().Show();
            }
        }
    }
}