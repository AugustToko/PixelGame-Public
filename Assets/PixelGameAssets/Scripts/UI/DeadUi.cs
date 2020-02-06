using PixelGameAssets.Scripts.Core;
using PixelGameAssets.Scripts.SceneUtils;
using UnityEngine;

namespace PixelGameAssets.Scripts.UI
{
    public class DeadUi : MonoBehaviour
    {
        public void BackToTown()
        {
            SceneUtil.BackToTown();
        }

        public void Restart()
        {
            SceneUtil.RestartCurrentScene();
        }
    }
}