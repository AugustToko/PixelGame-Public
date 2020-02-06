using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QInventory
{
    [RequireComponent(typeof(SkillBar))]
    public class SkillBarInputManager : MonoBehaviour
    {
        [SerializeField]
        private List<SkillBarInput> skillBarInputs = new List<SkillBarInput>();

        private IEnumerator Start()
        {
            yield return 0;
            foreach (SkillBarInput skillBarInput in skillBarInputs)
            {
                skillBarInput.keyText = Q_GameMaster.Instance.inventoryManager.skillBar.slots[skillBarInput.SlotID].transform.GetChild(0).GetComponent<Text>();
                if (skillBarInput.keyName.Length == 0)
                    skillBarInput.keyText.text = skillBarInput.key.ToString();
                else
                    skillBarInput.keyText.text = skillBarInput.keyName.ToString();
            }
        }

        private void Update()
        {
            foreach (SkillBarInput skillBarInput in skillBarInputs)
            {
                if (Input.GetKeyDown(skillBarInput.key))
                {
                    int childNum = Q_GameMaster.Instance.inventoryManager.skillBar.slots[skillBarInput.SlotID].transform.childCount;

                    if (childNum == 2)
                    {
                        ItemData data = Q_GameMaster.Instance.inventoryManager.skillBar.slots[skillBarInput.SlotID].transform.GetChild(1).GetComponent<ItemData>();
                        data.UseItem();
                    }
                }
            }
        }

        public int ReturnKeyListCount()
        {
            return skillBarInputs.Count;
        }
    }
}

