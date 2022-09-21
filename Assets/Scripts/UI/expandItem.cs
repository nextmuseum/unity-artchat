using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class expandItem : MonoBehaviour
{

    public void showDescription()
    {
        for (int a = 1; a < transform.childCount; a++)
        {
            bool active = transform.GetChild(a).gameObject.activeSelf;
            Debug.Log(transform.GetChild(a).gameObject.name);
            transform.GetChild(a).gameObject.SetActive(!active);
        }
    }
    void Update()
    {
        
    }
}
