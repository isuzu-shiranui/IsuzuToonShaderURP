using IsuzuToonShaderURP.Runtime.Rendering.Passes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace IsuzuToonShaderURP.Runtime.Rendering.RendererFeatures
{
    // ReSharper disable once InconsistentNaming
    public class ITSPostProcessRendererFeature : ScriptableRendererFeature
    {
        private ITSPostProcessPass itsPostProcessPass;
        
        public override void Create()
        {
            var screenSpaceGradientMaterial =
                CoreUtils.CreateEngineMaterial("Hidden/Universal Render Pipeline/ITS/ScreenSpaceGradient");
            
            this.itsPostProcessPass = new ITSPostProcessPass(screenSpaceGradientMaterial)
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            this.itsPostProcessPass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget);
            renderer.EnqueuePass(this.itsPostProcessPass);
        }
    }
}