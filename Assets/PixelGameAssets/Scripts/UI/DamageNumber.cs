using System;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace PixelGameAssets.Scripts.UI
{
    public class DamageNumber : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private CanvasGroup _canvasGroup;

        private TextMeshProUGUI _text;

        private Canvas _canvas;

        private float _maxPos = 100f;

        private float _startPos;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _canvas = GetComponent<Canvas>();
            _canvas.sortingLayerName = "FX";
            _startPos = _rectTransform.position.y;
        }

        private void Update()
        {
            var pos = _rectTransform.position;
            _rectTransform.position = new Vector3(pos.x, pos.y + 75f * Time.deltaTime);
            _canvasGroup.alpha -= 1.5f * Time.deltaTime;
            if (_rectTransform.position.y - _startPos >= _maxPos)
            {
                Destroy(gameObject);
            }
        }

        public void SetUpDamage(int damage)
        {
            _text.text = "-" + damage;
        }
    }
}