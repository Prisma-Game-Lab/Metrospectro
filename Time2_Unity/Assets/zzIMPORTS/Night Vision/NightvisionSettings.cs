using UnityEngine;

namespace Nightvision
{
    [System.Serializable]
    public class NightvisionSettings
    {
        public const float DefaultPower = 20;
        public float Power = DefaultPower;

        public VignetteSettings Vignette = new VignetteSettings();
        public NoiseSettings Noise = new NoiseSettings();
        public Color EffectColor = Color.green;

        public void UpdateMaterial(ref Material material, Vector2 noiseOffset)
        {
            material.SetFloat("_Power", Power);
            material.SetColor("_Color", EffectColor);

            //Vignette
            if (Vignette.Mode == VignetteSettings.VignetteMode.Off)
            {
                material.EnableKeyword("Vignette_Off");
                material.DisableKeyword("Vignette_Texture");
                material.DisableKeyword("Vignette_Procedural");
            }
            else if (Vignette.Mode == VignetteSettings.VignetteMode.Texture)
            {
                material.SetColor("_VignetteColor", Vignette.color);
                material.SetTexture("_VignetteTex", Vignette.Texture.Texture);

                material.DisableKeyword("Vignette_Off");
                material.EnableKeyword("Vignette_Texture");
                material.DisableKeyword("Vignette_Procedural");
            }
            else if (Vignette.Mode == VignetteSettings.VignetteMode.Procedural)
            {
                material.SetColor("_VignetteColor", Vignette.color);
                material.SetFloat("_VignetteRadius", Vignette.Procedural.Radius);
                material.SetFloat("_VignetteSharpness", Vignette.Procedural.Sharpness);

                material.DisableKeyword("Vignette_Off");
                material.DisableKeyword("Vignette_Texture");
                material.EnableKeyword("Vignette_Procedural");
            }

            //Noise
            if (Noise.Mode == NoiseSettings.NoiseMode.Off)
            {
                material.EnableKeyword("Noise_Off");
                material.DisableKeyword("Noise_Texture");
                material.DisableKeyword("Noise_Procedural");
            }
            else if (Noise.Mode == NoiseSettings.NoiseMode.Texture)
            {
                material.SetTexture("_NoiseTex", Noise.Texture.Texture);
                if (Noise.Texture.Texture != null)
                {
                    Vector2 PixTile = new Vector2((float)Screen.width / Noise.Texture.Texture.width, (float)Screen.height / Noise.Texture.Texture.width);
                    Vector2 Tile = Vector2.one;
                    if (PixTile.x > PixTile.y)
                    {
                        Tile.x = PixTile.x / PixTile.y;
                    }
                    else
                    {
                        Tile.y = PixTile.y / PixTile.x;
                    }
                    Tile /= Noise.Texture.Scale;
                    material.SetFloat("_NoiseTileX", Tile.x);
                    material.SetFloat("_NoiseTileY", Tile.y);
                }
                material.SetFloat("_NoiseOffsetX", noiseOffset.x);
                material.SetFloat("_NoiseOffsetY", noiseOffset.y);
                material.SetFloat("_NoisePower", Noise.Power);

                material.DisableKeyword("Noise_Off");
                material.EnableKeyword("Noise_Texture");
                material.DisableKeyword("Noise_Procedural");
            }
            else if (Noise.Mode == NoiseSettings.NoiseMode.Procedural)
            {
                material.SetFloat("_NoiseOffsetX", noiseOffset.x);
                material.SetFloat("_NoiseOffsetY", noiseOffset.y);
                material.SetFloat("_NoisePower", Noise.Power);

                material.DisableKeyword("Noise_Off");
                material.DisableKeyword("Noise_Texture");
                material.EnableKeyword("Noise_Procedural");
            }
        }
    }
}
