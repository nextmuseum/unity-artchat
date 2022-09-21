using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEventOnKeyDown : MonoBehaviour
{
    public KeyCode keyCode;
    public UnityEvent triggerEvent;
    void Update()
    {
        if(Input.GetKeyDown(keyCode))
        {
            Debug.Log("hit keyboard key!!");
            triggerEvent?.Invoke();
        }
    }
}
