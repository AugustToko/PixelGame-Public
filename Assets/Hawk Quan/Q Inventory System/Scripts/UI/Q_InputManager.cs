using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Q_InputManager : MonoBehaviour {
    public static Q_InputManager Instance; 

    public KeyCode useItem;
    public KeyCode interact;
    public KeyCode pickUpItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
