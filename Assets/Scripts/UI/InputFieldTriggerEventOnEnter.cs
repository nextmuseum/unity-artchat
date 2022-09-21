using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TMP_InputField))]
public class InputFieldTriggerEventOnEnter : MonoBehaviour
{
    public UnityEvent triggerEvent;
    private TMP_InputField inputText;

    private void Start() {
       inputText = GetComponent<TMP_InputField>();

       inputText.onTouchScreenKeyboardStatusChanged.AddListener(TouchScreenKeyboardDone);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            TriggerEvent();
        }
    }

    private void TouchScreenKeyboardDone(TouchScreenKeyboard.Status status)
    {
        if(status == TouchScreenKeyboard.Status.Done)
        {
            TriggerEvent();
        }
    }

    private void TriggerEvent()
    {
        triggerEvent?.Invoke();
        inputText.text = "";
    }



}
