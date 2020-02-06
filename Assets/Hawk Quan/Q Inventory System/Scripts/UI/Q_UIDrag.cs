using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QInventory
{
    [AddComponentMenu("Q Inventory/Q UI Drag")]
    public class Q_UIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        private Vector2 offset;
        public void OnBeginDrag(PointerEventData eventData)
        {
            offset = eventData.position - (Vector2)transform.position;
            transform.position = eventData.position - offset;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position - offset;
        }
    }
}
