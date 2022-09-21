using UnityEditor;
using UnityEngine;

namespace PlayModeTests
{
    public class PrefabBuilder
    {
        #if UNITY_EDITOR
        private const string CommentPrefabPath = "Assets/Prefabs/Comment.prefab";
        private const string MessagePagePrefabPath = "Assets/Prefabs/ARMessagePage.prefab";
        private const string ImmersalSDKPrefabPath = "Assets/ImmersalSDK/Core/Prefabs/ImmersalSDK.prefab";
        #else
        private const string CommentPrefabPath = "Comment_Testing.prefab";
        private const string MessagePagePrefabPath = "ARMessagePage_Testing.prefab";
        private const string ImmersalSDKPrefabPath = "ImmersalSDK_Testing.prefab";
        #endif
        
        private string prefabToBuild = "";
        private bool withInstance = false;
        private Transform parentTransform = null;

        public PrefabBuilder WithInstance()
        {
            withInstance = true;
            return this;
        }
        public PrefabBuilder BuildComment()
        {
            prefabToBuild = CommentPrefabPath;
            return this;
        }

        public PrefabBuilder BuildMessagePage()
        {
            prefabToBuild = MessagePagePrefabPath;
            return this;
        }
        
        public PrefabBuilder BuildImmersalSDK()
        {
            prefabToBuild = ImmersalSDKPrefabPath;
            return this;
        }

        public PrefabBuilder WithParentObject(Transform parentTrans)
        {
            parentTransform = parentTrans;
            return this;
        }
    
        public GameObject Build()
        {
            #if UNITY_EDITOR
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabToBuild);
            #else
            GameObject prefab = Resources.Load(prefabToBuild) as GameObject;
            #endif
            
            if (!withInstance)
                return prefab;
            
            GameObject instance = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, parentTransform) as GameObject;
            return instance;
        }

        public static implicit operator GameObject(PrefabBuilder builder)
        {
            return builder.Build();
        }
    }
}
