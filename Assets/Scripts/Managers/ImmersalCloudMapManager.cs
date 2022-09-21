using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Immersal.AR;
using Immersal.REST;
using UnityEngine;

namespace ARtChat
{
    public class ImmersalCloudMapManager : ARMapManager
    {
        private ARMap _currentActiveARMap;
        private GameObject _currentActiveMapCanvas;
        private Transform _root;

        private void Awake()
        {
            _root = GameObject.FindObjectOfType<ARSpace>().transform;
        }

        public override async UniTask<GameObject> Activate(int mapID)
        {
            await LoadMap(mapID);
            return _currentActiveMapCanvas;
        }

        public override void Unload()
        {
            if (_currentActiveARMap == null) return;
            _currentActiveARMap.FreeMap(true);
            _currentActiveMapCanvas = null;
            _currentActiveARMap = null;
        }
        
        private async UniTask LoadMap(int id)
        {
            JobLoadMapBinaryAsync job = new JobLoadMapBinaryAsync();
            job.id = id;
            job.OnResult += result =>
            {
                Debug.Log($"Loading Map {job.id} with {result.mapData.Length} bytes");

                Color pointCloudColor =
                    ARMap.pointCloudColors[UnityEngine.Random.Range(0, ARMap.pointCloudColors.Length)];

                ARMap.RenderMode renderMode = ARMap.RenderMode.EditorOnly;

                _currentActiveARMap = ARSpace.LoadAndInstantiateARMap(_root, result, renderMode, pointCloudColor);
                UpdateCurrentMap();
            };

            await job.RunJobAsync();
        }

        private void UpdateCurrentMap()
        {
            createCanvas(_currentActiveARMap.gameObject);
            _currentActiveMapCanvas = _currentActiveARMap.transform.GetChild(0).gameObject;
        }
    }
}
