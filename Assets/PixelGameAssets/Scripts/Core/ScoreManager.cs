using System;
using PixelGameAssets.Scripts.Misc;
using UnityEngine;

namespace PixelGameAssets.Scripts.Core
{
    /// <summary>
    /// 分数管理
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        public int Score { get; private set; }

        public int HighScore { get; private set; }

        public bool HasNewHighScore { get; private set; }

        public static event Action<int> ScoreUpdated = delegate { };
        public static event Action<int> HighScoreUpdated = delegate { };

        private void Awake()
        {
            if (Instance)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            Reset();
        }

        public void Reset()
        {
            // Initialize score
            Score = 0;

            // Initialize highscore
            HighScore = PlayerPrefs.GetInt(ConstKeys.HIGHSCORE, 0);
            HasNewHighScore = false;
        }

        public void AddScore(int amount)
        {
            Score += amount;

            // Fire event
            ScoreUpdated(Score);

            if (Score > HighScore)
            {
                UpdateHighScore(Score);
                HasNewHighScore = true;
            }
            else
            {
                HasNewHighScore = false;
            }
        }

        public void UpdateHighScore(int newHighScore)
        {
            // Update highscore if commonPlayer has made a new one
            if (newHighScore > HighScore)
            {
                HighScore = newHighScore;
                PlayerPrefs.SetInt(ConstKeys.HIGHSCORE, HighScore);
                HighScoreUpdated(HighScore);
            }
        }
    }
}