using IsuzuToonShaderURP.Runtime.Rendering.Overrides;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using BlendMode = IsuzuToonShaderURP.Runtime.Rendering.Overrides.BlendMode;

namespace IsuzuToonShaderURP.Runtime.Rendering.Passes
{
    // ReSharper disable once InconsistentNaming
    public class ITSPostProcessPass : ScriptableRenderPass
    {
        private const string RENDER_POST_PROCESSING_TAG = "Render PostProcessing Effects";
        private static readonly ProfilingSampler ProfilingRenderPostProcessing = new ProfilingSampler(RENDER_POST_PROCESSING_TAG);
        
        private readonly Material screenSpaceGradientMaterial;
        private ScreenSpaceGradient screenSpaceGradient;

        private RenderTargetIdentifier source;
        private RenderTargetHandle destination;
        private RenderTargetHandle tempTargetHandle;

        public ITSPostProcessPass(Material screenSpaceGradientMaterial)
        {
            this.screenSpaceGradientMaterial = screenSpaceGradientMaterial;

            this.tempTargetHandle.Init("_TemporaryColorTexture");
        }

        public void Setup(in RenderTargetIdentifier source, RenderTargetHandle destination)
        {
            this.source = source;
            this.destination = destination;
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!renderingData.cameraData.postProcessEnabled) return;
            
            var stack = VolumeManager.instance.stack;
            this.screenSpaceGradient = stack.GetComponent<ScreenSpaceGradient>();

            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, ProfilingRenderPostProcessing))
            {
                this.Render(cmd, ref renderingData);
            }
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;
            var isSceneViewCamera = cameraData.isSceneViewCamera;

            if (!isSceneViewCamera)
            {
                if (this.screenSpaceGradient != null)
                {
                    this.SetupScreenSpaceGradient(cameraData, cmd);
                }
            }

            this.Blit(cmd, this.tempTargetHandle.Identifier(),
                this.destination == RenderTargetHandle.CameraTarget
                    ? this.source
                    : this.destination.Identifier());
        }
        

        private void SetupScreenSpaceGradient(CameraData cameraData, CommandBuffer cmd)
        {
            if(!this.screenSpaceGradient.IsActive()) return;

            var blendMode = this.screenSpaceGradient.blendMode.value;
            var intensity = this.screenSpaceGradient.intensity.value;
            var noiseTex = this.screenSpaceGradient.noiseTexture.value;
            var topColor = this.screenSpaceGradient.topColor.value;
            var bottomColor = this.screenSpaceGradient.bottomColor.value;
            var offset = this.screenSpaceGradient.offset.value;

            this.screenSpaceGradientMaterial.SetFloat(ShaderConstants.IntensityParam, intensity);
            this.screenSpaceGradientMaterial.SetTexture(ShaderConstants.NoiseTexParam, noiseTex);
            this.screenSpaceGradientMaterial.SetColor(ShaderConstants.TopColorParam, topColor);
            this.screenSpaceGradientMaterial.SetColor(ShaderConstants.BottomColorParam, bottomColor);
            this.screenSpaceGradientMaterial.SetFloat(ShaderConstants.OffsetParam, offset);
            
            switch (blendMode)
            {
                case BlendMode.Multiply:
                    CoreUtils.SetKeyword(this.screenSpaceGradientMaterial, "_BLENDMODE_ADD", false);
                    CoreUtils.SetKeyword(this.screenSpaceGradientMaterial, "_BLENDMODE_MULTIPLY", true);
                    break;
                case BlendMode.Add:
                    CoreUtils.SetKeyword(this.screenSpaceGradientMaterial, "_BLENDMODE_ADD", true);
                    CoreUtils.SetKeyword(this.screenSpaceGradientMaterial, "_BLENDMODE_MULTIPLY", false);
                    break;
            }

            cmd.SetGlobalTexture(ShaderConstants.SourceTexParam, this.source);
            cmd.GetTemporaryRT(this.tempTargetHandle.id, cameraData.cameraTargetDescriptor, FilterMode.Bilinear);
            this.Blit(cmd, this.source, this.tempTargetHandle.Identifier(), this.screenSpaceGradientMaterial);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (this.destination == RenderTargetHandle.CameraTarget)
            {
                cmd.ReleaseTemporaryRT(this.tempTargetHandle.id);
            }
        }

        private static class ShaderConstants
        {
            public static readonly int IntensityParam   = Shader.PropertyToID("_Intensity");
            public static readonly int SourceTexParam   = Shader.PropertyToID("_MainTex");
            public static readonly int NoiseTexParam   = Shader.PropertyToID("_NoiseTex");
            public static readonly int TopColorParam   = Shader.PropertyToID("_TopColor");
            public static readonly int BottomColorParam   = Shader.PropertyToID("_BottomColor");
            public static readonly int OffsetParam   = Shader.PropertyToID("_Offset");
        }
    }
}