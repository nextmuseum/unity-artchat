using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameFetcher : MonoBehaviour
{
    public RawImage debuggingTexture;
    // Script Inputs
    public bool m_shouldCaptureOnNextFrame = false;
    public Color32[] m_lastCapturedColors;

    // Privates
    Texture2D m_centerPixTex;

    void Start()
    {
        Resolution currentResolution = Screen.currentResolution;
        m_centerPixTex = new Texture2D(currentResolution.width, currentResolution.height, TextureFormat.RGBA32, false);
    }

    void OnPostRender()
    {
        if (m_shouldCaptureOnNextFrame)
        {
            Resolution res = Screen.currentResolution;
            Color32[] currentCapture = GetRenderedColors();
            // m_lastCapturedColors = GetRenderedColors();
            // m_shouldCaptureOnNextFrame = currentCapture.Equals(m_lastCapturedColors);
            m_lastCapturedColors = currentCapture;
            StopCapturing();
        }
    }
    
    // public IEnumerator FetchingNextFrame()
    // {
    //     yield return new WaitForEndOfFrame();
    //     Resolution res = Screen.currentResolution;
    //     Color32[] currentCapture = GetRenderedColors();
    //     m_lastCapturedColors = currentCapture;
    //     yield return null;
    // }

    // Helpers
    Color32[] GetRenderedColors()
    {
        Resolution currentResolution = Screen.currentResolution;
        m_centerPixTex.ReadPixels(new Rect(0, 0, currentResolution.width, currentResolution.height), 0, 0);
        m_centerPixTex.Apply();

        if(debuggingTexture != null)
            debuggingTexture.texture = m_centerPixTex;

        return m_centerPixTex.GetPixels32();
    }


    public void StartCapturing()
    {
        m_shouldCaptureOnNextFrame = true;
    }

    public void StopCapturing()
    {
        m_shouldCaptureOnNextFrame = false;
    }
}