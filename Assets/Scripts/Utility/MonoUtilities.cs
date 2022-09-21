using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class MonoUtilities : MonoBehaviour
{
    private static MonoUtilities _instance;

    public static MonoUtilities Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject();
                go.name = "[Singelton] - MonoUtils";
                _instance = go.AddComponent<MonoUtilities>();
            }

            return _instance;
        }
    }

    public void  InstantiateGameObject(GameObject go)
    {
        Instantiate(go);
    }
    public T FindObjOfType<T>() where T : Object
    {
        T foundObject = FindObjectOfType<T>();
        return foundObject;
    }
    
}
