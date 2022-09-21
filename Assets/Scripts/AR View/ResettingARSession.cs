using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARtChat
{
    public class ResettingARSession : MonoBehaviour
    {
        private ARSession _arSession;

        private void Awake()
        {
            _arSession = GetComponent<ARSession>();
        }

        void Start()
        {
            _arSession.Reset();
        }
    }
}
