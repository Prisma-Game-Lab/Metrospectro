using UnityEngine;

namespace Nightvision
{
    [System.Serializable]
    public class VignetteSettings
    {
        public enum VignetteMode
        {
            Off,
            Texture,
            Procedural
        }
        [System.Serializable]
        public class TextureSettings
        {
            public Texture Texture;
        }
        [System.Serializable]
        public class ProceduralSettings
        {
            public const float DefaultRadius = 1.0f;
            public const float DefaultSharpness = 50;

            public float Radius = DefaultRadius;
            public float Sharpness = DefaultSharpness;
        }

        public VignetteMode Mode;

        public Color color = Color.black;

        public TextureSettings Texture = new TextureSettings();
        public ProceduralSettings Procedural = new ProceduralSettings();
    }
}
