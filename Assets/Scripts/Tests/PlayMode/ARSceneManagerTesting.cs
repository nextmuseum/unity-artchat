using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using UnityEditor;

namespace PlayModeTests
{
    public class ARSceneManagerTesting 
    {
        public class LoadingComments
        {
            public const string ArtworkID = "60efe17d8ff8372a39c2b8eb";
            
            private ARSceneManager _arSceneManager;
            private ARSpaceManager _arSpaceManager;
            private QRReader _reader;
            [UnityTest]
            public IEnumerator Should_Not_Duplicate_Comments_When_Load_Unload_Load()
            {
                CreateScene();

                int firstLoadCount = 0;
                int secondLoadCount = 0;
                yield return LoadingScene().ToCoroutine();
                async UniTask LoadingScene()
                {
                    CancellationToken token = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
                    await _arSceneManager.LoadScene(ArtworkID, token);
                    firstLoadCount = _arSpaceManager.worldSpaceCanvas.transform.childCount;
                    await UniTask.Delay(2000);
                    _arSceneManager.UnloadScene();
                    await UniTask.Delay(2000);
                    token = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
                    await _arSceneManager.LoadScene(ArtworkID, token);
                    secondLoadCount = _arSpaceManager.worldSpaceCanvas.transform.childCount;
                }
                
                Debugger.Logger.Blue("Comments Object Active first = " + firstLoadCount.ToString() + 
                                     "second = " + secondLoadCount.ToString());

                secondLoadCount.Should().Be(firstLoadCount);
            }

            private void CreateScene()
            {
                GameObject immersalSDK = A.Prefab.BuildImmersalSDK().WithInstance();
                GameObject pleaseScanImmersalTextDummy = A.GameObject;
                GameObject crosshairTmp = A.GameObject.WithImage();
                GameObject iconsDummy = A.GameObject;
                GameObject commentPrefab = A.Prefab.BuildComment();
                GameObject cameraObj = A.GameObject.WithCamera().WithQRReader();
                GameObject arSceneManagerObject =
                    A.GameObject.WithObjectBasedARMapManager().WithARLocalizer().WithARSpaceManager().WithARSceneManager();
                
                _arSceneManager = arSceneManagerObject.GetComponent<ARSceneManager>();
                _arSpaceManager = arSceneManagerObject.GetComponent<ARSpaceManager>();
                _reader = cameraObj.GetComponent<QRReader>();
                
                cameraObj.tag = "MainCamera";
                _reader.crosshair = crosshairTmp;
                _arSceneManager.crosshair = crosshairTmp;
                _arSceneManager.icons = iconsDummy;
                _arSceneManager.pleaseScanImmersalText = pleaseScanImmersalTextDummy;
                _arSpaceManager.commentPrefab = commentPrefab;
            }
        }
    }
}