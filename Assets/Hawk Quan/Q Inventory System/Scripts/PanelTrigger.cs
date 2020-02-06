using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Q Inventory/Panel Trigger")]
public class PanelTrigger : MonoBehaviour {
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private float openDistance;
    [SerializeField]
    private GameObject[] panelsOpenWith;
    [SerializeField]
    private bool panelAppearAbove = false;
    [SerializeField]
    private Vector3 offset;
    private Scrollbar m_Scrollbar;

    private void Start()
    {
        m_Scrollbar = panel.transform.Find("Scroll Rect").Find("Scrollbar").GetComponent<Scrollbar>();
    }
    private void Update()
    {
        if (!Q_GameMaster.Instance.inventoryManager.player)
            return;

        if (!panel)
            return;

        if(Vector3.Distance(transform.position, Q_GameMaster.Instance.inventoryManager.player.transform.position) <= openDistance)
        {
            if (Input.GetKeyDown(Q_InputManager.Instance.interact))
            {
                if(panel)
                {
                    Q_GameMaster.Instance.inventoryManager.PlayOpenPanelClip();
                    OpenPanel();
                    if(panel.activeSelf)
                    {
                        foreach (GameObject m_Panel in panelsOpenWith)
                        {
                            if (!m_Panel.activeSelf)
                            {
                                OpenTargetPanel(m_Panel);
                            }
                        }
                    }
                }
            }
        }


        else if(panel.activeSelf)
        {
            if (m_Scrollbar)
                m_Scrollbar.value = 1;
            panel.SetActive(false);
            Q_GameMaster.Instance.inventoryManager.toolTip.Deactivate();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, openDistance);
    }

    void OpenTargetPanel(GameObject target)
    {
        target.SetActive(true);
    }

    void OpenPanel()
    {
        if(!panel.activeSelf && panelAppearAbove)
        {
           panel.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + offset);
        }

        if (panel.activeSelf)
            Q_GameMaster.Instance.inventoryManager.toolTip.Deactivate();

        panel.SetActive(!panel.activeSelf);
    }
}
