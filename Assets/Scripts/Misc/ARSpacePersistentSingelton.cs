using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARSpacePersistentSingelton : MonoBehaviour
{
    public static ARSpacePersistentSingelton SpacePersistentSingelton;

    private void Awake()
    {
       DontDestroyOnLoad(this);
       if (SpacePersistentSingelton == null)
       {
           SpacePersistentSingelton = this;
       }
       else
       {
          Destroy(gameObject); 
       }
    }
}
