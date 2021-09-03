using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;


namespace IsuzuToonShaderURP.Editor
{
    public class IsuzuToonShader : IsuzuToonShaderGUIBase
    {
        [Flags]
        private enum Expandable
        {
            SurfaceOptions       = 1 << 0,
            MainColorInputs      = 1 << 1,
            ShadowsControl       = 1 << 2,
            NormalInputs         = 1 << 3,
            SpecularInputs       = 1 << 4,
            MetallicInputs       = 1 << 5,
            MatCapInputs         = 1 << 6,
            SubsurfaceInputs     = 1 << 7,
            RimInputs            = 1 << 8,
            EmissionInputs       = 1 << 9,
            LightColorBlend      = 1 << 10,
            AmbientLightControls = 1 << 11,
            OutlineControls      = 1 << 12,
            Advanced             = 1 << 13,

        }
        
        private IsuzuToonShaderGUI.IsuzuToonShaderProperties properties;

        private readonly MaterialHeaderScopeList materialHeaderScopeList = new(uint.MaxValue & ~(uint)Expandable.OutlineControls);
        
        protected override void ShaderPropertiesGUI(MaterialProperty[] properties)
        {
            this.properties = new IsuzuToonShaderGUI.IsuzuToonShaderProperties(properties);
            EditorGUI.BeginChangeCheck();
            
            this.materialHeaderScopeList.DrawHeaders(this.MaterialEditor, this.Material);

            if (!EditorGUI.EndChangeCheck()) return;
            foreach (var obj in this.MaterialEditor.targets) MaterialChanged((Material)obj);
        }

        protected override void OnOpenGUI(Material material, MaterialEditor materialEditor)
        {
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.SurfaceOptions, (uint)Expandable.SurfaceOptions, this.DrawSurfaceOptions);
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.MainColorInputsLabel, (uint)Expandable.MainColorInputs, _ => this.DrawBaseProperties());
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.ShadowsControlLabel, (uint)Expandable.ShadowsControl, _ => this.DrawShadowProperties());
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.NormalInputsLabel, (uint)Expandable.NormalInputs, _ => this.DrawNormalProperties());
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.SpecularInputsLabel, (uint)Expandable.SpecularInputs, _ => this.DrawSpecularProperties());
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.MetallicInputsLabel, (uint)Expandable.MetallicInputs, _ => this.DrawMetallicProperties());
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.MatCapInputsLabel, (uint)Expandable.MatCapInputs, _ => this.DrawMatCapProperties());
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.SubsurfaceInputsLabel, (uint)Expandable.SubsurfaceInputs, _ => this.DrawSubsurfaceProperties());
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.RimInputsLabel, (uint)Expandable.RimInputs, _ => this.DrawRimProperties());
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.EmissionInputsLabel, (uint)Expandable.EmissionInputs, _ => this.DrawEmissionProperties());
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.LightColorBlendLabel, (uint)Expandable.LightColorBlend, _ => this.DrawLightColorBlendProperties());
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.AmbientLightControlsLabel, (uint)Expandable.AmbientLightControls, _ => this.DrawAmbientLightControlsProperties());
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.OutlineControlsLabel, (uint)Expandable.OutlineControls, _ => this.DrawOutlineControlsProperties());
            this.materialHeaderScopeList.RegisterHeaderScope(Styles.AdvancedLabel, (uint)Expandable.Advanced, _ => this.DrawAdvancedOptions());
        }

        private void DrawSurfaceOptions(Material material)
        {
            this.DoPopup(Styles.SurfaceType, this.properties.surfaceTypeProp, Styles.SurfaceTypeNames);
            var surfaceType = (SurfaceType)this.properties.surfaceTypeProp.floatValue;
            if (this.properties.surfaceTypeProp != null && (surfaceType == SurfaceType.Transparent || surfaceType == SurfaceType.Fade))
            {
                this.DoPopup(Styles.BlendingMode, this.properties.blendModeProp, Styles.BlendModeNames);
                
                EditorGUI.indentLevel++;
                this.DrawTexturePropertySingleLine(Styles.TransparentMaskText, this.properties.transparentMaskProp, this.properties.transparencyProp);
                EditorGUI.indentLevel--;
                this.DrawShaderProperty(Styles.UseMainTexAlphaText,  this.properties.useMainTexAlphaProp,  1);
                this.DrawShaderProperty(Styles.InvertAlphaText,      this.properties.invertAlphaProp,      1);

            }

            DrawFloatToggleProperty(Styles.AlphaClipText, this.properties.alphaClipProp);
            
            if (this.properties.alphaClipProp != null && this.properties.cutoffProp != null && Mathf.Approximately(this.properties.alphaClipProp.floatValue, 1))
                this.MaterialEditor.ShaderProperty(this.properties.cutoffProp, Styles.AlphaClipThresholdText, 1);
            
            this.DoPopup(Styles.CullingText, this.properties.cullingProp, Styles.RenderFaceNames);
        }
        
        private void DrawBaseProperties()
        {
            this.DrawTexturePropertySingleLine(Styles.MainTex, this.properties.mainTexProp, this.properties.baseColorProp);
            this.DrawTexturePropertySingleLine(Styles.FirstShadeMap, this.properties.firstShadeMapProp, this.properties.firstShadeColorProp);
            this.DrawTexturePropertySingleLine(Styles.SecondShadeMap, this.properties.secondShadeMapProp, this.properties.secondShadeColorProp);
        }
        
        private void DrawShadowProperties()
        {
            DrawFloatToggleProperty(Styles.CastShadowText, this.properties.castShadowsProp);
            DrawFloatToggleProperty(Styles.ReceiveShadowText, this.properties.receiveShadowsProp);
            EditorGUI.BeginDisabledGroup(this.properties.receiveShadowsProp.floatValue < 0.5);
            {
                this.DrawShaderProperty(Styles.FirstShadeColorStep, this.properties.firstShadeColorStepProp, 1);
                this.DrawShaderProperty(Styles.FirstShadeColorSoftness, this.properties.firstShadeSoftnessProp, 1);
                this.DrawShaderProperty(Styles.SecondShadeColorStep, this.properties.secondShadeColorStepProp, 1);
                this.DrawShaderProperty(Styles.SecondShadeColorSoftness, this.properties.secondShadeSoftnessProp, 1);
                EditorGUILayout.Space();
                DrawFloatToggleProperty(Styles.ReceiveSystemShadowText, this.properties.receiveSystemShadowProp);
                EditorGUI.BeginDisabledGroup(this.properties.receiveSystemShadowProp.floatValue < 0.5);
                this.DrawShaderProperty(Styles.SystemShadowLevel, this.properties.systemShadowLevelProp, 1);
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndDisabledGroup();
        }
        
        private void DrawNormalProperties()
        {
            BaseShaderGUI.DrawNormalArea(this.MaterialEditor, this.properties.normalProp, this.properties.normalScaleProp);
        }

        private void DrawSpecularProperties()
        {
            this.DrawShaderProperty(Styles.Specular, this.properties.specularProp);
            this.DrawTexturePropertySingleLine(Styles.SpecularMask, this.properties.specularMaskProp, this.properties.specularColorProp);
            
            this.DrawShaderProperty(Styles.SpecularStep, this.properties.specularStepProp);
            this.DrawShaderProperty(Styles.SpecularSoftness, this.properties.specularSoftnessProp);
        }
        
        private void DrawMetallicProperties()
        {
            this.DrawShaderProperty(Styles.Metallic, this.properties.metallicProp);
            this.DrawShaderProperty(Styles.Smoothness, this.properties.smoothnessProp);
            this.DrawTexturePropertySingleLine(Styles.MetallicMask, this.properties.metallicMaskProp, null);
        }
        
        private void DrawMatCapProperties()
        {
            this.DrawShaderProperty(Styles.MatCap, this.properties.matCapProp);
            this.DrawTexturePropertySingleLine(Styles.MatCapTex, this.properties.matCapTexProp, this.properties.matCapColorProp);
            this.DrawTexturePropertySingleLine(Styles.MatCapMask, this.properties.matCapMaskProp, null);
        }
        
        private void DrawSubsurfaceProperties()
        {
            DrawFloatToggleProperty(Styles.UseSubsurfaceText, this.properties.useSubsurfaceProp);
            EditorGUI.BeginDisabledGroup(this.properties.useSubsurfaceProp.floatValue < 0.5);
            {
                this.DrawTexturePropertySingleLine(Styles.SubsurfaceMask, this.properties.subsurfaceMaskProp, this.properties.subsurfaceColorProp);
                this.DrawShaderProperty(Styles.SubsurfaceScattering, this.properties.subsurfaceScatteringProp);
                this.DrawShaderProperty(Styles.SubsurfaceRadius, this.properties.subsurfaceRadiusProp);
            }
            EditorGUI.EndDisabledGroup();
        }
        
        private void DrawRimProperties()
        {
            DrawFloatToggleProperty(Styles.UseRimText, this.properties.useRimProp);
            EditorGUI.BeginDisabledGroup(this.properties.useRimProp.floatValue < 0.5);
            {
                if (this.properties.rimMaskProp != null && this.properties.rimColorProp != null)
                    this.MaterialEditor.TexturePropertyWithHDRColor(Styles.RimMask, this.properties.rimMaskProp, this.properties.rimColorProp, false);
                
                this.DrawShaderProperty(Styles.RimMin, this.properties.rimMinProp);
                this.DrawShaderProperty(Styles.RimMax, this.properties.rimMaxProp);
                this.DrawShaderProperty(Styles.RimSmooth, this.properties.rimSmoothProp);
                DrawFloatToggleProperty(Styles.RimLightDirectionMaskText, this.properties.rimLightDirectionMaskProp);
            }
            EditorGUI.EndDisabledGroup();
        }
        
        private void DrawEmissionProperties()
        {
            DrawFloatToggleProperty(Styles.UseEmissiveText, this.properties.useEmissionProp);
            EditorGUI.BeginDisabledGroup(this.properties.useEmissionProp.floatValue < 0.5);
            {
                if (this.properties.emissionMapProp != null && this.properties.emissionColorProp != null)
                    this.MaterialEditor.TexturePropertyWithHDRColor(Styles.EmissionMap, this.properties.emissionMapProp, this.properties.emissionColorProp, false);
            }
            EditorGUI.EndDisabledGroup();
        }
        
        private void DrawLightColorBlendProperties()
        {
            this.DrawShaderProperty(Styles.LightColorMainColor, this.properties.lightColorMainColorProp);
            this.DrawShaderProperty(Styles.LightColorFirstShadeColor, this.properties.lightColorFirstShadeColorProp);
            this.DrawShaderProperty(Styles.LightColorSecondShadeColor, this.properties.lightColorSecondShadeColorProp);
            this.DrawShaderProperty(Styles.LightColorRimColor, this.properties.lightColorRimColorProp);
            this.DrawShaderProperty(Styles.LightColorSpecularColor, this.properties.lightColorSpecularColorProp);
            this.DrawShaderProperty(Styles.LightColorSubsurfaceColor, this.properties.lightColorSubsurfaceColorProp);
        }
        
        private void DrawAmbientLightControlsProperties()
        {
            this.DrawShaderProperty(Styles.AmbientColorBlend, this.properties.ambientColorBlendProp);
            this.DrawShaderProperty(Styles.GIIntensity, this.properties.gIIntensityProp);
            EditorGUILayout.Space();
            this.DrawShaderProperty(Styles.BacklightIntensity, this.properties.backlightIntensityProp);
            this.DrawShaderProperty(Styles.BacklightOffset, this.properties.backlightOffsetProp);
        }
        
        private void DrawOutlineControlsProperties()
        {
            DrawFloatToggleProperty(Styles.UseOutlineText, this.properties.useOutlineProp);
            EditorGUI.BeginDisabledGroup(this.properties.useOutlineProp.floatValue < 0.5);
            {
                this.DrawTexturePropertySingleLine(Styles.OutlineMask, this.properties.outlineMaskProp, this.properties.outlineColorProp);
                this.DrawShaderProperty(Styles.OutlineThickness, this.properties.outlineThicknessProp);
            }
            EditorGUI.EndDisabledGroup();
        }


        private void DrawAdvancedOptions()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.properties.stencilProp.hasMixedValue;
            var stencilMode = (StencilMode) this.properties.stencilProp.floatValue;
            stencilMode = (StencilMode)EditorGUILayout.EnumPopup("Stencil Mode", stencilMode);
            if (EditorGUI.EndChangeCheck())
            {
                this.MaterialEditor.RegisterPropertyChangeUndo("Stencil Mode");
                this.properties.stencilProp.floatValue = (float)stencilMode;
            }
            EditorGUI.showMixedValue = false;
            
            if (stencilMode > StencilMode.None)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = this.properties.referenceProp.hasMixedValue;
                var queue = EditorGUILayout.IntSlider("Reference", (int) this.properties.referenceProp.floatValue, 0, 255);
                if (EditorGUI.EndChangeCheck()) this.properties.referenceProp.floatValue = queue;
                EditorGUI.showMixedValue = false;
            }
            
            EditorGUILayout.Space();
            
            this.MaterialEditor.EnableInstancingField();
            this.DrawQueueOffsetField();
        }

        private void DrawQueueOffsetField()
        {
            if (this.properties.queueOffsetProp != null)
            {
                this.MaterialEditor.IntSliderShaderProperty(this.properties.queueOffsetProp, -QUEUE_OFFSET_RANGE, QUEUE_OFFSET_RANGE, Styles.QueueSlider);
            }
        }
    }
}