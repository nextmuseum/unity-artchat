using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GrabARCameraFrame 
{
    private ARCameraManager _cameraManager;
    private Action notifier;

    public Texture2D CameraImage
    {
        get;
        private set;
    } 
    public GrabARCameraFrame(Action notify) 
    {
        _cameraManager = MonoUtilities.Instance.FindObjOfType<ARCameraManager>();
        notifier = notify;
    }

    public void GetImageAsync()
    {
        // if (_cameraManager.TryGetLatestImage(out XRCpuImage image))
        if (_cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            // Debug.Log("Got Image");
            MonoUtilities.Instance.StartCoroutine(ProcessImage(image));
            image.Dispose();
        }
        else{
            // Debug.Log("Could not Grab Image");
        }
    }

    private IEnumerator ProcessImage(XRCpuImage cameraImage)
    {
        // Debug.Log("Start Converting Image");
        // Debug.Log("Camera width / height = " + (cameraImage.width + " / " + cameraImage.height));
        var request = cameraImage.ConvertAsync(new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, cameraImage.width, cameraImage.height),
            outputDimensions = new Vector2Int(cameraImage.width, cameraImage.height),
            outputFormat = TextureFormat.RGB24,
            transformation = XRCpuImage.Transformation.MirrorY
        });

        // Debug.Log("Wait unitl ready... " + request.status.IsDone() + ",  " + request.status.IsError());
        yield return new WaitUntil(() => request.status.IsDone());
        // Debug.Log("Ready");

        if (request.status != XRCpuImage.AsyncConversionStatus.Ready)
        {
            Debug.LogErrorFormat("Request failed with status{0}", request.status);
            request.Dispose();
            yield break;
        }

        
        CameraImage = ConvertXRCamImageToTexture2D(request);
        // Debug.Log("Camera Image returened to Texture2D = " + CameraImage);
        // ValueChanged();
        notifier?.Invoke();
    }

    private Texture2D ConvertXRCamImageToTexture2D(XRCpuImage.AsyncConversion conversionRequest)
    {
        // Debug.Log("start converting to texture2d");

        var rawImage = conversionRequest.GetData<byte>();
        Texture2D output = new Texture2D(conversionRequest.conversionParams.outputDimensions.x,
            conversionRequest.conversionParams.outputDimensions.y,
            conversionRequest.conversionParams.outputFormat,
            false);

        
        output.LoadRawTextureData(rawImage);
        output.Apply();
        conversionRequest.Dispose();
        return output;
    }

}
