using UnityEngine;

namespace ARtChat
{
    public class RotateImageEffect 
    {
        private Shader _rotateImageShader;
        private Material _rotateImageMaterial;
        private float _angle = 0;

        public RotateImageEffect(float angle = 0)
        {
            _rotateImageShader = Shader.Find("Hidden/RotateImage");
            _rotateImageMaterial = new Material(_rotateImageShader);
            _angle = angle;
        }

        public Texture2D RotateTexture(Texture2D _texture, float angle = 0)
        {
            _angle = angle;
            
            RenderTexture output = new RenderTexture(_texture.width, _texture.height, 0);
            
            _rotateImageMaterial.SetFloat("_RotationAngle", GetRadiantAngle());
            
            Graphics.Blit(_texture, output, _rotateImageMaterial);
            
            Texture2D outputTex2D = ConvertToTexture2D(output);
            
            output.Release();
            return outputTex2D;
        }

        private float GetRadiantAngle()
        {
            return _angle * Mathf.PI / 180;
        }

        private Texture2D ConvertToTexture2D(RenderTexture source)
        {
            Texture2D output = new Texture2D(source.width, source.height);
            RenderTexture old_active = RenderTexture.active;
            RenderTexture.active = source;
            
            output.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            output.Apply();

            RenderTexture.active = old_active;
            return output;
        }
    }
}
