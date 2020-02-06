using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PixelGameAssets.Scripts.Core
{
    /// <summary>
    /// Volume Manager 用于控制后处理
    /// </summary>
    public class VManager : MonoBehaviour
    {
        [SerializeField] private Volume volume;

        public static VManager Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }

            Instance = this;
        }

        private void Start()
        {
            if (PlayerPrefs.GetInt("CameraEffects") == 0)
            {
                gameObject.SetActive(true);
                Debug.Log("Set Active: true");
            }
            else if (PlayerPrefs.GetInt("CameraEffects") == 1)
            {
                gameObject.SetActive(false);
                Debug.Log("Set Active: false");
            }
        }

        public void SetBlur(int val)
        {
            volume.profile.TryGet(out DepthOfField d);
            if (d)
            {
                d.focalLength.value = val;
            }
        }
    }
}