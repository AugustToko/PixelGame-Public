using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QInventory
{
    public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [HideInInspector]
        public Q_Inventory inv;

        ItemData itemData;

        private float y;
        //private Vector2 offset;
        private void Start()
        {
            itemData = GetComponent<ItemData>();
            inv = itemData.inv;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemData.item != null)
            {
                if (inv.tag == "Equipment")
                    return;

                if ((inv.tag == "Inventory" || inv.tag == "SkillBar"))
                {
                    if (inv.ReturnCoolingState(itemData.slot))
                        return;
                }

                //offset = eventData.position - (Vector2)transform.position;
                transform.SetParent(Q_GameMaster.Instance.inventoryManager.Canvas.transform);
                //transform.position = eventData.position - offset;
                transform.position = eventData.position;
                GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (itemData.item != null)
            {
                if (inv.tag == "Equipment")
                    return;

                if ((inv.tag == "Inventory" || inv.tag == "SkillBar"))
                {
                    if (inv.ReturnCoolingState(itemData.slot))
                        return;
                }

                //transform.position = eventData.position - offset;
                transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (inv.tag == "Equipment")
                return;
            if ((inv.tag == "Inventory" || inv.tag == "SkillBar"))
            {
                if (inv.ReturnCoolingState(itemData.slot))
                    return;
            }


            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Drop Item");

                DropItem();
            }

            else if (eventData.pointerCurrentRaycast.gameObject.tag == "Vendor")
            {
                Debug.Log("Sell Item");
                itemData.SellItem();
            }

            else if (eventData.pointerCurrentRaycast.gameObject.tag == inv.tag)
            {
                //Debug.Log("the same inventory");
                transform.SetParent(inv.slots[itemData.slot].transform);
                transform.position = inv.slots[itemData.slot].transform.position;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }

            else
            {
                transform.SetParent(inv.slots[itemData.slot].transform);
                transform.position = inv.slots[itemData.slot].transform.position;
                Debug.Log("Return");
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }

        bool JudgeIsOutSide(GameObject target)
        {

            Vector2 targetSize = target.GetComponent<RectTransform>().sizeDelta;
            if (transform.position.x < (target.transform.position.x - targetSize.x / 2) || transform.position.x > (target.transform.position.x + targetSize.x / 2) || transform.position.y < (target.transform.position.y - targetSize.y / 2) || transform.position.y > (target.transform.position.y + targetSize.y / 2))
            {
                return true;
            }
            return false;
        }

        void DropItem()
        {
            if (itemData.item.m_object)
            {
                GameObject dropedItem = null;


                switch (Q_GameMaster.Instance.inventoryManager.dropType)
                {
                    case DropType.DropWithMouse:
                        {
                            //drop at mouse position
                            //Debug.Log("Drop Item");
                            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            dropedItem = Instantiate(itemData.item.m_object, mousePosition, Quaternion.identity);
                            break;
                        }

                    case DropType.DropByThrown:
                        {
                            //drop near player
                            //Debug.Log("Drop Item");
                            Vector3 dropPosition = Q_GameMaster.Instance.inventoryManager.player.transform.position;

                            float xScale = 0;
                            if (Q_GameMaster.Instance.inventoryManager.player.transform.localScale.x >= 0)
                                xScale = 1;
                            else
                                xScale = -1;

                            dropPosition.x += Q_GameMaster.Instance.inventoryManager.playerWidth * xScale;
                            dropedItem = Instantiate(itemData.item.m_object, dropPosition, Quaternion.identity);
                            dropedItem.GetComponent<Rigidbody2D>().AddForce(new Vector2(xScale, 1) * Q_GameMaster.Instance.inventoryManager.dropForce);
                            break;
                        }

                    case DropType.Drop3D:
                        {
                            //3D drop
                            //Debug.Log("Drop Item");
                            //y = Q_GameMaster.Instance.inventoryManager.player.transform.position.y;
                            //Debug.Log(y);
                            Vector3 dropPosition = Vector3.zero;
                            dropPosition = Q_GameMaster.Instance.inventoryManager.player.transform.position;
                            if (Q_GameMaster.Instance.inventoryManager.controller)
                            {
                                dropPosition += Q_GameMaster.Instance.inventoryManager.player.transform.forward * Q_GameMaster.Instance.inventoryManager.controller.radius * 2;
                                dropPosition.y += Q_GameMaster.Instance.inventoryManager.player.GetComponent<CharacterController>().center.y;
                            }
                            else
                            {
                                dropPosition += Q_GameMaster.Instance.inventoryManager.player.transform.forward * Q_GameMaster.Instance.inventoryManager.playerWidth;
                                dropPosition.y += Q_GameMaster.Instance.inventoryManager.playerHeight;
                            }
                            dropedItem = Instantiate(itemData.item.m_object, dropPosition, Quaternion.identity);
                            dropedItem.GetComponent<Rigidbody>().AddForce(Q_GameMaster.Instance.inventoryManager.player.transform.forward * Q_GameMaster.Instance.inventoryManager.dropForce);
                            break;
                        }
                }

                dropedItem.GetComponent<GameObjectData>().amount = itemData.amount;
                itemData.ClearItem();
            }
        }
    }
}
