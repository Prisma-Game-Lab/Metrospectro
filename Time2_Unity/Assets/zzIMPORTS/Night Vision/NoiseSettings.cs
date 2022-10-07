using UnityEngine;

namespace Nightvision
{
    [System.Serializable]
    public class NoiseSettings
    {
        public enum NoiseMode
        {
            Off,
            Texture,
            Procedural
        }
        [System.Serializable]
        public class TextureSettings
        {
            public const float DefaultScale = 1.0f;

            public Texture Texture;
            public float Scale = DefaultScale;
        }

        public const float DefaultUpdateTime = 0;
        public const float DefaultPower = 0.5f;

        public NoiseMode Mode;
        public TextureSettings Texture = new TextureSettings();
        public float UpdateTime = DefaultUpdateTime;
        public float Power = DefaultPower;
    }
}