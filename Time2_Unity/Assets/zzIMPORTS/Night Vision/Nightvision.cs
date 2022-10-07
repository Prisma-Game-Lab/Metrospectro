using UnityEngine;

namespace Nightvision
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class Nightvision : MonoBehaviour
    {
        public NightvisionSettings Settings;

        private Material nvMaterial;
        private Vector2 noiseOffset = Vector2.zero;
        private float lastTime = 0;

        void Awake()
        {
            nvMaterial = new Material(Shader.Find("Hidden/Nightvision"));
            UpdateMaterial();
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            UpdateMaterial();
            Graphics.Blit(src, dst, nvMaterial);
        }

        public void UpdateMaterial()
        {
            if (lastTime <= Time.time - Settings.Noise.UpdateTime)
            {
                noiseOffset = new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                lastTime = Time.time;
            }

            Settings.UpdateMaterial(ref nvMaterial, noiseOffset);
        }
    }
}