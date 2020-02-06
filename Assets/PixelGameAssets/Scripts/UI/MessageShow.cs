using TMPro;
using UnityEngine;

namespace PixelGameAssets.Scripts.UI
{
    public class MessageShow : MonoBehaviour
    {
        public TextMeshProUGUI mainText;
        
        public TextMeshProUGUI subText;
        
        public void Done()
        {
            Destroy(gameObject);
        }
    }
}