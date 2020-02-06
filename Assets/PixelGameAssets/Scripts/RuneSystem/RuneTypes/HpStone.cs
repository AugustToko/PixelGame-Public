using System;

namespace PixelGameAssets.Scripts.RuneSystem.RuneTypes
{
    public class HpStone : BaseRune
    {
        private void Awake()
        {
            description = "一块可以持续回血的石头";
        }

        public override void Active()
        {
            throw new System.NotImplementedException();
        }

        public override void DisActive()
        {
            throw new System.NotImplementedException();
        }
    }
}