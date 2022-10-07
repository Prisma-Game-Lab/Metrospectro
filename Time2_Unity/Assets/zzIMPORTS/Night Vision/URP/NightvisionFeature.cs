using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Nightvision.URP
{
    //Credits to https://gamedevbill.com/full-screen-shaders-in-unity/ for providing good explanation on how to implement URP Post-Processing Effects
    public class NightvisionFeature : ScriptableRendererFeature
    {
        public NightvisionSettings Settings = new NightvisionSettings();

        private Material material;
        private NightvisionPass nightVisionPass;

        public override void Create()
        {
            material = new Material(Shader.Find("Hidden/Nightvision"));
            nightVisionPass = new NightvisionPass(material, Settings.Noise.UpdateTime, name);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (material != null)
            {
                RenderTargetIdentifier source = renderer.cameraColorTarget;
                RenderTargetHandle destination = RenderTargetHandle.CameraTarget;

                UpdateMaterial();

                nightVisionPass.Setup(source, destination);
                renderer.EnqueuePass(nightVisionPass);
            }
        }

        public void UpdateMaterial()
        {
            Settings.UpdateMaterial(ref material, Vector2.zero);
        }
    }
}