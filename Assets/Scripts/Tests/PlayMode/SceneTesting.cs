using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace PlayModeTests
{
    public class SceneTesting
    {
        
        public const string ArScene = "AR Scene_OnlyLogin";
        public const string UIScene = "App UI_OnlyLogin";

        public class SceneSwitching
        {
            [UnityTest]
            public IEnumerator Loading_AR_UI_AR()
            {
                SceneManager.LoadScene(ArScene, LoadSceneMode.Single);
                yield return new WaitForSeconds(3);
                SceneManager.LoadScene(UIScene, LoadSceneMode.Single);
                yield return new WaitForSeconds(3);
                SceneManager.LoadScene(ArScene, LoadSceneMode.Single);
            }
        }
    }
}
