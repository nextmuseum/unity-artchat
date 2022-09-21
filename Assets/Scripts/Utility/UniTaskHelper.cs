using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;


namespace ARtChat.Utility
{
    public static class UniTaskHelper 
    {
        public static CancellationToken GetTimeoutTokenFromSeconds(float seconds)
        {
            CancellationTokenSource timeoutToken = new CancellationTokenSource();
            timeoutToken.CancelAfterSlim(TimeSpan.FromSeconds(seconds));
            return timeoutToken.Token;
        }
        public static CancellationTokenSource GetTimeoutCancellationSourceFromSeconds(float seconds)
        {
            CancellationTokenSource timeoutToken = new CancellationTokenSource();
            timeoutToken.CancelAfterSlim(TimeSpan.FromSeconds(seconds));
            return timeoutToken;
        }
    }

    public static class TextureEdit
    {
        public static Color[] GetCenteredCropColors(Texture2D source, Rect croppingArea)
        {
            int width = Mathf.FloorToInt(croppingArea.width);
            int height = Mathf.FloorToInt(croppingArea.height);

            int xStart = Mathf.FloorToInt((source.width / 2) - (width / 2));
            int yStart = Mathf.FloorToInt((source.height / 2) - (height / 2));
            
            // Debug.Log($"x,y = {xStart}, {yStart} \n" +
            //           $"widht, height = {width}, {height}" +
            //           $"tex resolution = {source.width}, {source.height}");

            Color[] croppedColors = source.GetPixels(xStart, yStart, width, height);

            return croppedColors;
        }
        
    }
}
