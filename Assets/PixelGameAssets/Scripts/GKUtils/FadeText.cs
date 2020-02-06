using TMPro;
using UnityEngine;

namespace PixelGameAssets.Scripts.GKUtils
{
    /// <summary>
    /// 可渐变文字
    /// </summary>
    public class FadeText : MonoBehaviour
    {
        public TextMeshProUGUI text;

        public Animator animator;

        public bool showed = false;

        public float coldTime = 5f;

        private float _coldTimer = 0f;

        public FadeText(Animator animator)
        {
            this.animator = animator;
        }

        private void Update()
        {
            if (_coldTimer > 0f)
            {
                _coldTimer -= Time.deltaTime;
            }
            else if(showed && _coldTimer == 0f)
            {
                Hide();
            }
        }

        public void EventShowed()
        {
            showed = true;
            _coldTimer = coldTime;
        }
        
        public void EventHided()
        {
            showed = false;
        }

        public void Show()
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("FadeTextIn"))
            {
                animator.Play("FadeTextIn");
            }
            Debug.Log("Show");
        }

        public void Hide()
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("FadeTextOut"))
            {
                animator.Play("FadeTextOut");
            }
            Debug.Log("Hide");
        }
    }
}