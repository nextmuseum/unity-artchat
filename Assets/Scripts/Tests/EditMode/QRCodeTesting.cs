using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using ZXing;

public class QRCodeTesting
{

    [UnityTest]
    public IEnumerator QRCodeLibraryCanRecognizedTwoDifferentFrames()
    {
        Texture2D qrCode01 = TestUtilities.TestData.LoadQRCode("60b79ec451c8f571c43d339b.png");
        Texture2D qrCode02 = TestUtilities.TestData.LoadQRCode("60efe34b8ff8372a39c2b8f0.png");

        BarcodeReader barcodeReader = new BarcodeReader();

        string text01 = barcodeReader.Decode(qrCode01.GetPixels32(), qrCode01.width, qrCode01.height).Text;
        string text02 = barcodeReader.Decode(qrCode02.GetPixels32(), qrCode02.width, qrCode02.height).Text;
        
        Assert.AreNotEqual(text01, text02);
        
        yield return null;
    }

}
