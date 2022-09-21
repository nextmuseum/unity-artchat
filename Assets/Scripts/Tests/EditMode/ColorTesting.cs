using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ColorTesting
{
    [Test]
    public void AreDifferentColorsNotEqual()
    {
        Texture2D qrCode01 = TestUtilities.TestData.LoadQRCode("60b79ec451c8f571c43d339b.png");
        Texture2D qrCode02 = TestUtilities.TestData.LoadQRCode("60efe34b8ff8372a39c2b8f0.png");
        Color32[] qrCode01Color = qrCode01.GetPixels32();
        Color32[] qrCode02Color = qrCode02.GetPixels32();
        
        Assert.True(!qrCode01Color.Equals(qrCode02Color));
    }

    [Test]
    public void IsSameColorEqual()
    {
        Texture2D qrCode01 = TestUtilities.TestData.LoadQRCode("60b79ec451c8f571c43d339b.png");
        Color32[] qrCode01Color = qrCode01.GetPixels32();
        
        Assert.True(qrCode01Color.Equals(qrCode01Color));
    }
}
