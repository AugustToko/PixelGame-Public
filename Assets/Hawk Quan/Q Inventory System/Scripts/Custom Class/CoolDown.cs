using UnityEngine.UI;

namespace QInventory
{
    [System.Serializable]
    public class CoolDown
    {
        public Image cd;
        public float coolTime;
        public bool isCooling = false;
        public CoolDown()
        {

        }

        public CoolDown(Image cd, float coolTime)
        {
            this.cd = cd;
            this.coolTime = coolTime;
        }
    }
}

