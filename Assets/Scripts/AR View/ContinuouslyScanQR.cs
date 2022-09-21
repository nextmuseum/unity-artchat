using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using ARtChat.Utility;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ARtChat
{
    public class ContinuouslyScanQR : MonoBehaviour
    {
        
        private QRReader _qrReader;
        private ARSceneManager _sceneManager;
        private string _scannedText = null;
        private CancellationTokenSource _disableCancellation = new CancellationTokenSource();
        private void Awake()
        {
            _qrReader = FindObjectOfType<QRReader>();
            _sceneManager = FindObjectOfType<ARSceneManager>();
        }

        private void OnEnable()
        {
            StartCoroutine(DelayEnable());
        }

        private IEnumerator DelayEnable()
        {
            _disableCancellation = new CancellationTokenSource();
            _scannedText = null;
            yield return new WaitForEndOfFrame();
            ContinuouslyTryScanning();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _sceneManager.StartLoadingScene("620f9ca15bd21bb798bebc2e");
        }

        private void OnDisable()
        {
            Cleanup();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _disableCancellation?.Cancel();
            _disableCancellation?.Dispose();
            _disableCancellation = null;
        }

        private async void ContinuouslyTryScanning()
        {
            while (String.IsNullOrEmpty(_scannedText) && _disableCancellation != null)
            {
                CancellationToken token = UniTaskHelper.GetTimeoutTokenFromSeconds(5);
                CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(token, _disableCancellation.Token);

                try
                {
                    await TryScanQRCode(linked.Token);
                }
                catch (OperationCanceledException e)
                {
                    Debug.Log($"QR-Scan Canceled {e}");
                }
            }
            
            if(!String.IsNullOrEmpty(_scannedText))
                _sceneManager.StartLoadingScene(_scannedText);
        }


        private async UniTask TryScanQRCode(CancellationToken token)
        {
            _scannedText = await _qrReader.GetQRCodeAsync(token);
            //_qrReader.setDefaultColor();
        }
    }
}
