using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace IsuzuToonShaderURP.Runtime.Rendering.Overrides
{
    public enum BlendMode
    {
        Multiply,
        Add
    }
    
    [Serializable, VolumeComponentMenu("ITS Post-processing/ScreenSpaceGradient")]
    public sealed class ScreenSpaceGradient : VolumeComponent, IPostProcessComponent
    {
        public BlendModeParameter blendMode = new BlendModeParameter(BlendMode.Add);
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public ColorParameter topColor = new ColorParameter(Color.black, false, true, false);
        public ColorParameter bottomColor = new ColorParameter(Color.black, false, true, false);
        public FloatParameter offset = new FloatParameter(0.0f);
        public TextureParameter noiseTexture = new TextureParameter(null);
        
        public bool IsActive() => this.intensity.value > 0.0f;

        public bool IsTileCompatible() => false;
    }

    [Serializable]
    public sealed class BlendModeParameter : VolumeParameter<BlendMode>
    {
        public BlendModeParameter(BlendMode value, bool overrideState = false) : base(value, overrideState)
        {
        }
    }
}