using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using System;
using System.Threading;
using ARtChat;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using ZXing.Common;
using Object = System.Object;

public class QRReader : MonoBehaviour
{
    public RawImage debuggingShowTexture;
    
    public RectTransform qrButton;
    
    private BarcodeReader barCodeReader;

    // private FrameFetcher m_pixelCapturer;
    private bool isDetecting = false;

    private string lastDetectedCode;

    public GameObject crosshair;
    //public Color scanningColor;
    //private Color defaultColor;
    //public Color inProgressColor;

    private GrabARCameraFrame cameraFrameGrabber;

    private Action frameGrabbe;
    private RotateImageEffect _rotateImageEffect;
    private Texture2D _debuggingTex;

    
    private void Awake()
    {
        //cameraFrameGrabber = new GrabARCameraFrame(frameGrabbe);
        // barCodeReader = new BarcodeReader();
        // cam = GetComponent<Camera>();
        _rotateImageEffect = new RotateImageEffect();
    }

    // Use this for initialization
    void Start()
    {
        cameraFrameGrabber = new GrabARCameraFrame(frameGrabbe);

        if(debuggingShowTexture != null)
            debuggingShowTexture.texture = _debuggingTex;

        var barCodeFormats = new List<BarcodeFormat>();
        barCodeFormats.Add(BarcodeFormat.QR_CODE);
        barCodeFormats.Add(BarcodeFormat.PDF_417);

        barCodeReader = new BarcodeReader
        {
            AutoRotate = false,
            Options = new DecodingOptions
            {
                PossibleFormats = barCodeFormats,
                TryHarder = false
            }
        };
        
        // Resolution currentResolution = Screen.currentResolution;
        // m_pixelCapturer = GetComponent<FrameFetcher>();
        //defaultColor = crosshair.GetComponent<Image>().color;

        // Rect qrRect = RectTransformUtility.PixelAdjustRect(qrButton, qrButton.parent.GetComponent<Canvas>());
        // Debug.Log($"qrRect pixel width = {qrRect.width} x {qrRect.height}");

        // StartCoroutine(QRReaderUpdate());
    }

    private void OnEnable()
    {
        frameGrabbe += OnFrameGrabbed;
    }

    private void OnDisable()
    {
        frameGrabbe -= OnFrameGrabbed;
    }

    public string getLastDetectedCode()
    {
        return lastDetectedCode;
    }

    public void startDetecting()
    {
        isDetecting = true;
    }

    public bool detectingStatus()
    {
        return isDetecting;
    }

    public void setActiveColor()
    {
        //crosshair.GetComponent<Image>().color = scanningColor;
    }

    public void setDefaultColor()
    {
        //crosshair.GetComponent<Image>().color = defaultColor;
    }

    public void setProgressColor()
    {
        //crosshair.GetComponent<Image>().color = inProgressColor;
    }
    
    

    public async UniTask<string> GetQRCodeAsync(CancellationToken token)
    {
        // if (cameraFrameGrabber == null) return "";
        
        // Debug.Log("Start deteting QR-Code");
        isDetecting = true;
        cameraFrameGrabber.GetImageAsync();
        // Set color blue
        setActiveColor();

        await UniTask.WaitUntil(() => !String.IsNullOrEmpty(lastDetectedCode), cancellationToken: token);
        
        if (lastDetectedCode.Equals("invalid"))
        {
            lastDetectedCode = "";
            isDetecting = false;
            setDefaultColor();
            return null;
        }
        
        // Debug.Log("Finished waiting");
        setProgressColor();
        isDetecting = false;
        string newQRCode = lastDetectedCode;
        lastDetectedCode = "";
        
        // Debug.Log("QRCode Detection finished");
        return newQRCode;
    }

    private void OnFrameGrabbed()
    {
        // Debug.Log("GrabbedFrame!!");
        DetectQRCode(cameraFrameGrabber.CameraImage);
    }

    private void DetectQRCode(Texture2D camImage)
    {
        Color[] colors = ARtChat.Utility.TextureEdit.GetCenteredCropColors(camImage, qrButton.rect);
        Texture2D cropped = new Texture2D((int)qrButton.rect.width, (int)qrButton.rect.height);
        cropped.SetPixels(colors);
        cropped.Apply();
        Texture2D rotated = _rotateImageEffect.RotateTexture(cropped, 270);
        
        // Graphics.CopyTexture(rotated, _debuggingTex);
        // debuggingShowTexture.texture = _debuggingTex;
        
        Color32[] framebuffer = rotated.GetPixels32();
        // Debug.Log("Reading Frame");
        // Color32[] framebuffer = m_pixelCapturer.m_lastCapturedColors;
        if (framebuffer == null)
        {
            // Debugger.Logger.Red("framebuffer is empty");
            return;
        }

        // Debug.Log("Try to Decode Barcode...");
        var data = barCodeReader.Decode(framebuffer, cropped.width, cropped.height);
        if (data != null)
        {
            lastDetectedCode = data.Text;
            // Debug.Log($"QR-Code Recognized {lastDetectedCode}");
            isDetecting = false;
            // m_pixelCapturer.m_lastCapturedColors = null;
            // m_pixelCapturer.StopCapturing();
        }
        else
        {
            // Debug.Log($"QR-Code data = is Null");
            lastDetectedCode = "invalid";
        }
        
        DestroyImmediate(cropped);
        DestroyImmediate(camImage);
        DestroyImmediate(rotated);
    }
}