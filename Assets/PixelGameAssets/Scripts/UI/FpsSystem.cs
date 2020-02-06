using System;
using UnityEngine;
using UnityEngine.UI;

namespace PixelGameAssets.Scripts.UI
{
    public class FpsSystem : MonoBehaviour
    {
        private Text _text;

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        // Update is called once per frame
        private void Update()
        {
            _text.text = "FPS: " + Math.Round(1 / Time.deltaTime);
        }
    }
}