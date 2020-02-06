using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Information : MonoBehaviour {
    [SerializeField]
    private float hideTime;

    private void Update()
    {
        if(gameObject.activeSelf)
        {
            StartCoroutine(HidePanel());
        }
    }

    IEnumerator HidePanel()
    {
        yield return new WaitForSeconds(hideTime);
        gameObject.SetActive(false);
    }
}
