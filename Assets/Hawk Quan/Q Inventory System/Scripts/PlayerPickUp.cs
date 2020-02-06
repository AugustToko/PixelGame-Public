using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    /// <summary>
    /// 玩家捡起物品交互脚本
    /// 通过在 Inventory Manager 中指定 Player 来自动添加此组件
    /// </summary>
    public class PlayerPickUp : MonoBehaviour
    {
        private const float rayDistance = 2.5f;
        private const float radius = 1.0f;

        private void FixedUpdate()
        {
            PickUpItem3D();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            GameObjectData god;
            if (!(god = collision.GetComponent<GameObjectData>()))
                return;

            if (!god.isEquipped)
            {
                if (Q_InputManager.Instance.pickUpItem == KeyCode.None)
                {
                    if (!Q_GameMaster.Instance.inventoryManager.playerInventory.CheckIsFull(god.item.ID))
                    {
                        god.AddItemSelf();
                        Q_GameMaster.Instance.inventoryManager.PlayAddItemClip();
                    }

                    else
                    {
                        Q_GameMaster.Instance.inventoryManager.SetInformation(Q_GameMaster.Instance.inventoryManager
                            .infoManager.inventoryFull);
                        Debug.Log("Player Inventory is Full!");
                    }
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            GameObjectData GOD = null;
            if (!(GOD = collision.GetComponent<GameObjectData>()))
                return;

            if (!GOD.isEquipped && Q_InputManager.Instance.pickUpItem != KeyCode.None)
            {
                if (Input.GetKeyDown(Q_InputManager.Instance.pickUpItem))
                {
                    if (!Q_GameMaster.Instance.inventoryManager.playerInventory.CheckIsFull(GOD.item.ID))
                    {
                        GOD.AddItemSelf();
                        Q_GameMaster.Instance.inventoryManager.PlayAddItemClip();
                    }

                    else
                    {
                        Q_GameMaster.Instance.inventoryManager.SetInformation(Q_GameMaster.Instance.inventoryManager
                            .infoManager.inventoryFull);
                        Debug.Log("Player Inventory is Full!");
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (Q_InputManager.Instance.pickUpItem != KeyCode.None)
                return;

            GameObjectData GOD = null;
            if (!(GOD = collision.GetComponent<GameObjectData>()))
                return;

            if (!GOD.isEquipped)
            {
                if (!Q_GameMaster.Instance.inventoryManager.playerInventory.CheckIsFull(GOD.item.ID))
                {
                    GOD.AddItemSelf();
                    Q_GameMaster.Instance.inventoryManager.PlayAddItemClip();
                }

                else
                {
                    Q_GameMaster.Instance.inventoryManager.SetInformation(Q_GameMaster.Instance.inventoryManager
                        .infoManager.inventoryFull);
                    Debug.Log("Player Inventory is Full!");
                }
            }
        }

        private void PickUpItem3D()
        {
            if (Q_InputManager.Instance.pickUpItem != KeyCode.None &&
                Q_GameMaster.Instance.inventoryManager.pickUp == PickUp.Pick3D)
            {
                RaycastHit hitInfo;
                Vector3 p1 = Vector3.zero;
                if (Q_GameMaster.Instance.inventoryManager.controller)
                {
                    CharacterController charCtrl = Q_GameMaster.Instance.inventoryManager.controller;
                    p1 = Q_GameMaster.Instance.inventoryManager.player.transform.position;
                    p1 -= Q_GameMaster.Instance.inventoryManager.player.transform.forward * charCtrl.radius;
                }
                else
                {
                    p1 = Q_GameMaster.Instance.inventoryManager.player.transform.position;
                    p1 -= Q_GameMaster.Instance.inventoryManager.player.transform.forward *
                          Q_GameMaster.Instance.inventoryManager.playerWidth;
                }

                if (Physics.SphereCast(p1, radius, Q_GameMaster.Instance.inventoryManager.player.transform.forward,
                    out hitInfo, rayDistance))
                {
                    GameObject go = hitInfo.collider.gameObject;
                    GameObjectData data;
                    if ((data = go.GetComponent<GameObjectData>()) != null)
                    {
                        if (data.isEquipped)
                            return;

                        GameObject pickTip = Q_GameMaster.Instance.inventoryManager.toolTip.pickTip;
                        pickTip.transform.position = Camera.main.WorldToScreenPoint(go.transform.position);
                        pickTip.transform.GetChild(0).gameObject.SetActive(true);
                        Q_GameMaster.Instance.inventoryManager.toolTip.pickTipText.text =
                            Q_InputManager.Instance.pickUpItem.ToString() + ": pick up " + data.item.itemName;
                        if (Input.GetKeyDown(Q_InputManager.Instance.pickUpItem))
                        {
                            data.AddItemSelf();
                            pickTip.transform.GetChild(0).gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        Q_GameMaster.Instance.inventoryManager.toolTip.pickTip.transform.GetChild(0).gameObject
                            .SetActive(false);
                    }
                }
                else
                {
                    Q_GameMaster.Instance.inventoryManager.toolTip.pickTip.transform.GetChild(0).gameObject
                        .SetActive(false);
                }
            }
        }
    }
}