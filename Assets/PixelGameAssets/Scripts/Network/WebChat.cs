using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebSocket = WebSocketSharp.WebSocket;

namespace PixelGameAssets.Scripts.Network
{
    public class WebChat : MonoBehaviour
    {
        public GameObject scrollViewBody;

        public GameObject textWrapper;

        public TMP_InputField inputText;

        private WebSocket _webSocket;

        private readonly List<string> _content = new List<string>();

        private void Start()
        {
            Connect();

            _webSocket.OnOpen += (sender, args) => { Debug.Log("OnOpen"); };
            _webSocket.OnClose += (sender, args) => { Debug.Log("OnClose"); };
            _webSocket.OnError += (sender, args) => { Debug.Log(args.Message); };
            _webSocket.OnMessage += (sender, args) =>
            {
                {
                    _content.Add(args.Data);
                    Debug.Log(args.Data);
                }
            };

            inputText.onSubmit.AddListener(arg0 =>
            {
                if (!_webSocket.IsConnected || !_webSocket.IsAlive)
                {
                    Connect();
                }

                _webSocket.SendAsync(arg0, b =>
                {
                    if (b)
                    {
                        Debug.Log("send success");
                        _content.Add(arg0);
                    }
                });
            });
        }

        private void Connect()
        {
            //            var webSocket = new WebSocket("wss://geek-cloud.top/websocket/pixel-chat");
            _webSocket = new WebSocket("ws://127.0.0.1:8272");
            _webSocket.Connect();
        }

        private void MakeText(string data)
        {
            Instantiate(textWrapper, scrollViewBody.transform).GetComponentInChildren<TextMeshProUGUI>().text = data;
        }

        private void Update()
        {
            if (_content.Count <= 0) return;
            foreach (var data in _content)
            {
                MakeText(data);
            }
            _content.Clear();
        }
    }
}