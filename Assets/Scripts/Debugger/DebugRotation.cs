using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARtChat
{
    public class DebugRotation : MonoBehaviour
    {
        public RawImage output;

        public Texture2D textureToRotate;
        public float Angle = 0;

        private RotateImageEffect _rotateImageEffect;

        private void Awake()
        {
            _rotateImageEffect = new RotateImageEffect();
        }

        private void Start()
        {
            ApplyRotation();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ApplyRotation();
            }
        }
        private void ApplyRotation()
        {
            Texture2D rotatedTex = _rotateImageEffect.RotateTexture(textureToRotate, Angle);
            output.texture = rotatedTex;
        }
    }
}
