using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Management;


public class ButtonFunctionAR : MonoBehaviour
{
    public GameObject UICanvas;
    public GameObject Guide;
    public GameObject ARMessagePage;
    public GameObject ARCanvas;
    public GameObject Background;

    private ARSceneManager _sceneManager;

    private void Awake()
    {
        _sceneManager = FindObjectOfType<ARSceneManager>();
    }

    public void changeUICanvasStatus()
    {
        bool UICanvasIsActive = UICanvas.activeSelf;
        bool ARCanvasIsActive = ARCanvas.activeSelf;
        UICanvas.SetActive(!UICanvasIsActive);
        ARCanvas.SetActive(!ARCanvasIsActive);
        EventManager.uiCanvasChanged?.Invoke(!UICanvasIsActive);
    }
    public void changeGuideStatus()
    {

        changeUICanvasStatus();
        
        Guide.SetActive(true);
        ARMessagePage.SetActive(false);


    }

    public void changeARMessagePageStatus()
    {
        changeUICanvasStatus();

        ARMessagePage.SetActive(true);
        Guide.SetActive(false);
    }

    public void loadAppUI()
    {
        SceneManager.LoadScene("App UI", LoadSceneMode.Single);
    }
    public void loadAppUI_OnlyLogin()
    {
        _sceneManager.UnloadScene();
        Debug.Log($"Active Loader = {LoaderUtility.GetActiveLoader()}");
        SceneManager.LoadScene("App UI_OnlyLogin", LoadSceneMode.Single);
        LoaderUtility.Deinitialize();
    }
}
