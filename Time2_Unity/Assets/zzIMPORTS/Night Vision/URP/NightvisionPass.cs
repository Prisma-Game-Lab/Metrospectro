using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Nightvision.URP
{
    public class NightvisionPass : ScriptableRenderPass
    {
        public Material material = null;

        private RenderTargetIdentifier source;
        private RenderTargetHandle destination;

        private RenderTargetHandle tempTexture;
        private string tag;

        private Vector2 noiseOffset = Vector2.zero;
        private float updateTime = 0;
        private float lastUpdateTime = 0;

        public NightvisionPass(Material _material, float noiseUpdateTime, string _tag)
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            material = _material;
            updateTime = noiseUpdateTime;

            tag = _tag;

            tempTexture.Init("_TemporaryTexture");
        }

        public void Setup(RenderTargetIdentifier _source, RenderTargetHandle _destination)
        {
            source = _source;
            destination = _destination;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            UpdateNoise();

            CommandBuffer commandBuffer = CommandBufferPool.Get(tag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            commandBuffer.GetTemporaryRT(tempTexture.id, opaqueDesc, FilterMode.Point);
            Blit(commandBuffer, source, tempTexture.Identifier(), material);
            Blit(commandBuffer, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        public void UpdateNoise()
        {
            if (lastUpdateTime <= Time.time - updateTime)
            {
                noiseOffset = new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                lastUpdateTime = Time.time;
            }

            material.SetFloat("_NoiseOffsetX", noiseOffset.x);
            material.SetFloat("_NoiseOffsetY", noiseOffset.y);
        }

        public override void FrameCleanup(CommandBuffer commandBuffer)
        {
            commandBuffer.ReleaseTemporaryRT(tempTexture.id);
        }
    }
}