using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    [AddComponentMenu("Q Inventory/Q UI Window")]
    public class Q_UIWindow : MonoBehaviour
    {
        [SerializeField]
        private bool fixedSpawnPosition = true;
        [SerializeField]
        private Vector2 UIPosition;

        private GameObject childPanel;
        //private bool needReset = false;
        private RectTransform rectTransform;

        private void Start()
        {
            childPanel = transform.GetChild(0).gameObject;
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (fixedSpawnPosition)
            {
                if (!childPanel.activeSelf && rectTransform.anchoredPosition != UIPosition)
                {
                    rectTransform.anchoredPosition = UIPosition;
                }
            }
        }

        public void SetUIposition()
        {
            UIPosition = GetComponent<RectTransform>().anchoredPosition;
        }
    }
}
