using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARtChat
{
    public class ShowARFloorPlaneConnection : MonoBehaviour
    {
        [Tooltip("Needs to have a lineRenderer")]
        public GameObject visualizationPrefab;
        
        private ARPlaneManager _planeManager;
        private ARRaycastManager _arRaycastManager;

        private LineRenderer _lineRenderer;
        private GameObject _visualizationInstance;
        

        private Vector3 bottomPosition
        {
            get => GetBottomPosition();
        }

        private Vector3 GetBottomPosition()
        {
                
                Vector3[] corners = new Vector3[4];
                GetComponent<RectTransform>().GetWorldCorners(corners);
                Vector3 bottomLeftCorner = corners[0];

                float deltaY = bottomLeftCorner.y - transform.position.y;
                Vector3 btnPos = transform.position;
                btnPos.y += deltaY;
                
                return btnPos;
        }
        private void Awake()
        {
            _planeManager = FindObjectOfType<ARPlaneManager>();
            _arRaycastManager = FindObjectOfType<ARRaycastManager>();

        }

        private void Update()
        {
            UpdateVisualizationPosition();
        }

        public void Show()
        {
            InstantiateVisualization();
        }

        public void Hide()
        {
            DestroyVisulaization();
        }

        private void InstantiateVisualization()
        {
            if (_visualizationInstance != null) return;
            _visualizationInstance = Instantiate(visualizationPrefab, this.transform);
            
            _lineRenderer = _visualizationInstance.GetComponentInChildren<LineRenderer>();
        }

        private void DestroyVisulaization()
        {
            if (_visualizationInstance == null) return;
            
            DestroyImmediate(_visualizationInstance);
            _visualizationInstance = null;
        }
        private void UpdateVisualizationPosition()
        {
            if (_visualizationInstance == null) return;
            
            Vector3 floorPos = GetShadowPosition();

            _visualizationInstance.transform.position = floorPos;
            _visualizationInstance.transform.rotation = Quaternion.identity;
            _lineRenderer.SetPosition(0, bottomPosition);
            _lineRenderer.SetPosition(1, floorPos);
        }
        private Vector3 GetShadowPosition()
        {
            Ray ray = BuildDownwardRay();
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            Vector3 floorPos = new Vector3(transform.position.x, 10000000, transform.position.z);
            if(_arRaycastManager.Raycast(ray, hits, TrackableType.Planes))
            {
                float minDist = hits[0].distance;
                foreach (ARRaycastHit hit in hits)
                {
                    minDist = Mathf.Min(minDist, hit.distance);
                }

                floorPos.y = bottomPosition.y - minDist;
            }
            else
            {
                floorPos.y = bottomPosition.y;
            }

            return floorPos;
        }

        private Ray BuildDownwardRay()
        {
            Vector3 startPos = transform.position;
            Vector3 direction = Vector3.down;
            Ray ray = new Ray(startPos, direction);
            return ray;
        }
    }
}
