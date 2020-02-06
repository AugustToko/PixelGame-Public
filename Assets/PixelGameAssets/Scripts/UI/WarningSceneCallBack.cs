using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelGameAssets.Scripts.UI
{
    public class WarningSceneCallBack : MonoBehaviour
    {
        [SerializeField] private string nextSceneName;
        
        /// <summary>
        /// 测试警告后调用
        /// </summary>
        /// <param name="pram"></param>
        public void DebugWarningDone(int pram)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}