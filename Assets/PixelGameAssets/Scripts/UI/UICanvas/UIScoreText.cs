using PixelGameAssets.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PixelGameAssets.Scripts.UI.UICanvas
{
    public class UIScoreText : MonoBehaviour
    {
        public Text text;

        private void OnEnable()
        {
            ScoreManager.ScoreUpdated += UpdateScoreUi;
        }

        private void OnDisable()
        {
            ScoreManager.ScoreUpdated -= UpdateScoreUi;
        }

        private void Start()
        {
            UpdateScoreUi(ScoreManager.Instance.Score);
        }

        private void UpdateScoreUi(int value)
        {
            if (text != null) text.text = value.ToString();
        }
    }
}