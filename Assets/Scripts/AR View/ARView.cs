using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ARView : MonoBehaviour
{
    public void loadAppUI()
    {
        SceneManager.LoadScene("App UI", LoadSceneMode.Single);
    }
}
