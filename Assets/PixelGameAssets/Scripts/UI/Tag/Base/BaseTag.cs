using UnityEngine;

namespace PixelGameAssets.Scripts.UI.Tag.Base
{
    public class BaseTag : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Canvas>().worldCamera = UnityEngine.Camera.main;
        }
        
    }
}