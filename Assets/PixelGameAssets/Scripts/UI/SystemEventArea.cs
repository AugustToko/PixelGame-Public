using System;
using System.Collections.Generic;
using PixelGameAssets.Scripts.EventManager;
using UnityEngine;
using UnityEngine.UI;
using EventType = PixelGameAssets.Scripts.EventManager.EventType;

namespace PixelGameAssets.Scripts.UI
{
    public class SystemEventArea : MonoBehaviour
    {
        [SerializeField] private Text systemEventText;

        private const byte MaxEventCount = 5;
        
        private readonly LinkedList<string> _events = new LinkedList<string>();

        private void Awake()
        {
            EventCenter.AddListener<string>(EventType.UpdateSystemEvent, AddEventText);
        }

        public void ClearContent()
        {
            _events.Clear();
            ClearText();
        }

        private void ClearText()
        {
            systemEventText.text = "";
        }

        public void AddEventText(string text)
        {
            if (_events.Count > MaxEventCount)
            {
                _events.RemoveFirst();
            }
            _events.AddLast(text);
            PushInText();
        }

        private void PushInText()
        {
            ClearText();
            foreach (var @event in _events)
            {
                systemEventText.text += ("\r\n" + @event + ";");
            }
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener<string>(EventType.UpdateSystemEvent, AddEventText);
        }
    }
}