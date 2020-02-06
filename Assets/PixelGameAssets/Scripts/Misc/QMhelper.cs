using QInventory;
using UnityEngine;
using UnityEngine.UI;

namespace PixelGameAssets.Scripts.Misc
{
    public static class QMhelper
    {
        private const int HpIndex = 0;

        public static void SetupPlayer(GameObject gameObject)
        {
            Q_GameMaster.Instance.inventoryManager.player = gameObject;
        }

        public static void SetupCoinText(Text text)
        {
            Q_GameMaster.Instance.inventoryManager.playerInventoryManager.playerCurrencies[HpIndex].showText = text;
        }

        public static void SetupHpText(Text text)
        {
            Q_GameMaster.Instance.inventoryManager.playerInventoryManager.playerAttributesUIs[HpIndex].showText = text;
        }

        public static float GetMaxHpVal()
        {
            return Q_GameMaster.Instance.inventoryManager.playerInventoryManager.playerAttributes[HpIndex].maxValue;
        }

        public static float GetCurrHpVal()
        {
            return Q_GameMaster.Instance.inventoryManager.playerInventoryManager.playerAttributes[HpIndex].currentValue;
        }

        public static void SetCurrentHp(float val)
        {
            Q_GameMaster.Instance.inventoryManager.playerInventoryManager.playerAttributes[HpIndex].currentValue = val;
        }

        public static void Save()
        {
            InventoryManager.SaveInventoryData();
        }

        public static void Load()
        {
            Q_GameMaster.Instance.GetComponent<LoadInventoryData>().Load();
        }

        public static void SetActive(bool val)
        {
            Q_GameMaster.Instance.gameObject.SetActive(val);
        }
    }
}