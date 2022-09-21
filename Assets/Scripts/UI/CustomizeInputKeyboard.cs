using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomizeInputKeyboard : MonoBehaviour
{

    public float offset = -20;
    private float openKeyboardHeight;
    private Vector3 initPos;
    private RectTransform rectTrans;
    // Start is called before the first frame update
    void Start()
    {
        rectTrans = GetComponent<RectTransform>();

        initPos = rectTrans.position;
        openKeyboardHeight = TouchScreenKeyboard.area.height + offset;
        
        // Debug.Log($"Keyboard Height = {openKeyboardHeight}");
    }

    // Update is called once per frame
    void Update()
    {
        if(TouchScreenKeyboard.visible)
        {
            openKeyboardHeight = TouchScreenKeyboard.area.height + offset;
            rectTrans.position = new Vector3(initPos.x, 
                                             initPos.y + openKeyboardHeight, 
                                             initPos.z);
            
        }

        else if(rectTrans.position != initPos)
        {
            rectTrans.position = initPos;
        }
    }
}
