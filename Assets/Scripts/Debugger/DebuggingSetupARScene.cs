using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggingSetupARScene : MonoBehaviour
{
    public List<GameObject> deactivateOnStartup;
    public List<GameObject> activateOnStartup;
    public List<GameObject> dontDestroyOnLoad;

    void Start()
    {
        foreach(GameObject obj in deactivateOnStartup)
        {
            obj.SetActive(false);
        }

        foreach(GameObject obj in activateOnStartup)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in dontDestroyOnLoad)
        {
            DontDestroyOnLoad(obj);
        }
    }

}
