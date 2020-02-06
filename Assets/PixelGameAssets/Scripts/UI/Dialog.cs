using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelGameAssets.Scripts.UI
{
    public class Dialog : MonoBehaviour
    {
        public TextMeshProUGUI dialogTitle;

        public Button buttonLeft;

        public Button buttonRight;

        public Animator animator;

        public void Show()
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("DialogShow"))
            {
                animator.Play("DialogShow");
            }
        }

        public void Dismiss()
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("DialogDismiss"))
            {
                animator.Play("DialogDismiss");
            }
        }

        public void DestroyDialog()
        {
            Destroy(gameObject);
        }
    }
}