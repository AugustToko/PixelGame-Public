using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace QInventory
{
    public class SceneSwitch : MonoBehaviour
    {
        [SerializeField]
        private string sceneName;
        [SerializeField]
        private KeyCode key;

        [SerializeField]
        private GameObject instruction;

        [SerializeField]
        private KeyCode load;
        [SerializeField]
        private KeyCode save;

        private void Update()
        {
            if(Input.GetKeyDown(key))
            {
                instruction.SetActive(false);
                SwitchScene(); 
            }

            if (Input.GetKeyDown(save))
            {
                InventoryManager.SaveInventoryData();
            }


            if (Input.GetKeyDown(load))
            {
                Q_GameMaster.Instance.GetComponent<LoadInventoryData>().Load();
            }
        }

        

        private void SwitchScene()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
