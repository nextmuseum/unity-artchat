using System.Collections;
using System.Collections.Generic;
using ARtChat;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using Immersal;
using Immersal.AR;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests
{
    public class ImmersalCloudMapManagerTesting
    {
        public class Loading_Map_Tests
        {
            public string devToken = "ea7a053d683076bb5a57c14eed25b4f20d134f4d52b07b138c8bd30ea928195a";
            private int mapID = 37321;

            private ImmersalCloudMapManager _immersalCloudMapManager;

            [UnityTest]
            public IEnumerator Call_Activate_Should_Return_GameObject_With_AR_Map_In_Parent() =>
                UniTask.ToCoroutine(async () =>
                {
                    await SetUpScene();

                    GameObject arMapObj = await _immersalCloudMapManager.Activate(mapID);

                    arMapObj.Should().NotBeNull();
                    arMapObj.GetComponentInParent<ARMap>().Should().NotBeNull();
                });

            [UnityTest]
            public IEnumerator Call_Unload_Should_Destroy_Object() =>
                UniTask.ToCoroutine(async () =>
                {
                    await SetUpScene();
                    await _immersalCloudMapManager.Activate(mapID);

                    GameObject.FindObjectOfType<ARMap>().Should().NotBeNull();
                    _immersalCloudMapManager.Unload();

                    await UniTask.NextFrame();
                    
                    GameObject.FindObjectOfType<ARMap>().Should().BeNull();
                });

            private IEnumerator SetUpScene()
            {
                GameObject immersalSDKObj = A.Prefab.BuildImmersalSDK().WithInstance();
                GameObject arSpaceObj = A.GameObject.WithARSpace().WithImmersalCloudMapManager();

                ImmersalSDK immersalSDK = immersalSDKObj.GetComponent<ImmersalSDK>();
                immersalSDK.developerToken = devToken;

                _immersalCloudMapManager = arSpaceObj.GetComponent<ImmersalCloudMapManager>();
                
                yield return null;
            }
            
        }
    }
}
