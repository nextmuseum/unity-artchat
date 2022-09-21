using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DebuggingLoadComments : MonoBehaviour
{
    public string artworkID;
    ARSceneManager sceneManager;

#if UNITY_EDITOR
    void Start()
    {
        sceneManager = FindObjectOfType<ARSceneManager>();
        StartCoroutine(DelayedInitialize());
    }

    private IEnumerator DelayedInitialize()
    {
        yield return new WaitForSeconds(1);
        Initialize();
    }

    private async void Initialize()
    {
        CancellationToken token = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
        await sceneManager.LoadScene(artworkID, token);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Initialize();
        }
    }
#elif UNITY_IOS
#endif

}
