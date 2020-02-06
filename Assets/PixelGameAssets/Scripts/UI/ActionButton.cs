using UnityEngine;

namespace PixelGameAssets.Scripts.UI
{
    public class ActionButton : MonoBehaviour
    {
        public Animator animator;

        public bool fading;

        public void FadeMe()
        {
            fading = true;

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("FadeOut")) animator.Play("FadeOut");
        }

        public void DestroySelf()
        {
            if (!fading)
                Destroy(gameObject);
        }
    }
}