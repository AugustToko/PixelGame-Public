using PixelGameAssets.Scripts.EventManager;
using PixelGameAssets.Scripts.UI.UICanvas;
using UnityEngine;
using EventType = PixelGameAssets.Scripts.EventManager.EventType;

namespace PixelGameAssets.Scripts.Core
{
    /// <summary>
    /// 管理玩家经验值
    /// TODO: 玩家等级设定
    /// </summary>
    public class ExpManager : MonoBehaviour
    {
        private static ExpManager _instance;

        public static ExpManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameManager.Instance.gameObject.AddComponent<ExpManager>();
                }

                return _instance;
            }
            private set => _instance = value;
        }

        public class LevelDataPack
        {
            public int Level;
            public float LevelBarPercentage;

            public LevelDataPack(int level, float levelBarPercentage)
            {
                Level = level;
                LevelBarPercentage = levelBarPercentage;
            }
        }
        
        /// <summary>
        /// see <see cref="PlayerInfo.exp"/>
        /// </summary>
        public double Exp { get; set; }

        public int Level { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        public void AddExp(double exp)
        {
            if (Exp > 3600)
            {
                return;
            }

            Exp += exp;
            CheckLevel(UiManager.Instance != null);
        }

        private void CheckLevel(bool updateUi)
        {
            var percentage = 0.0f;

            if (Exp < 400 && Exp >= 0)
            {
                Level = 1;
                percentage = (float) (Exp / 400);
            }
            else if (Exp < 700 && Exp >= 400)
            {
                Level = 2;
                percentage = (float) ((Exp - 400) / 700);
            }
            else if (Exp < 1100 && Exp >= 700)
            {
                Level = 3;
                percentage = (float) ((Exp - 700) / 1100);
            }
            else if (Exp < 1700 && Exp >= 1100)
            {
                Level = 4;
                percentage = (float) ((Exp - 1100) / 1700);
            }
            else if (Exp < 2500 && Exp >= 1700)
            {
                Level = 5;
                percentage = (float) ((Exp - 1700) / 2500);
            }
            else if (Exp < 3600 && Exp >= 2500)
            {
                Level = 6;
                percentage = (float) ((Exp - 2500) / 3600);
            }


            if (updateUi)
            {
                EventCenter.Broadcast(EventType.UpdateLevel, new LevelDataPack(Level, percentage));
            }
        }
    }
}