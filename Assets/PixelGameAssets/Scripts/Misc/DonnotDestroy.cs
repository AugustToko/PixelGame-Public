using System;
using UnityEngine;

namespace PixelGameAssets.Scripts.Misc
{
    public class DonnotDestroy : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}