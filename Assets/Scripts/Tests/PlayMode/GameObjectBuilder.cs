using System.Collections;
using System.Collections.Generic;
using ARtChat;
using Immersal.AR;
using UnityEngine;
using UnityEngine.UI;

namespace PlayModeTests
{
    public class GameObjectBuilder 
    {
        GameObject _gameObject;
        private GameObject BuildObject
        {
            get
            {
                if (_gameObject == null)
                    _gameObject = new GameObject();
                return _gameObject;
            }
        }

        public GameObjectBuilder WithARSpaceManager()
        {
            BuildObject.AddComponent<ARSpaceManager>();
            return this;
        }
        
        public GameObjectBuilder WithARSceneManager()
        {

            BuildObject.AddComponent<ARSceneManager>();
            return this;
        }
        
        public GameObjectBuilder WithARSpace()
        {

            BuildObject.AddComponent<ARSpace>();
            return this;
        }

        public GameObjectBuilder WithObjectBasedARMapManager()
        {
            BuildObject.AddComponent<ObjectBasedMapManager>();
            return this;
        }
        
        public GameObjectBuilder WithImmersalCloudMapManager()
        {
            BuildObject.AddComponent<ImmersalCloudMapManager>();
            return this;
        }
        
        public GameObjectBuilder WithARLocalizer()
        {
            BuildObject.AddComponent<ARLocalizer>();
            return this;
        }

        public GameObjectBuilder WithCamera()
        {
            BuildObject.AddComponent<Camera>();
            return this;
        }

        public GameObjectBuilder WithQRReader()
        {
            BuildObject.AddComponent<QRReader>();
            return this;
        }

        public GameObjectBuilder WithImage()
        {
            BuildObject.AddComponent<Image>();
            return this;
        }
        
        public GameObjectBuilder WithCanvas()
        {
            BuildObject.AddComponent<Canvas>();
            return this;
        }
        public GameObject Build()
        {
            return BuildObject;
        }

        public static implicit operator GameObject(GameObjectBuilder builder)
        {
            return builder.Build();
        }
    }
}
