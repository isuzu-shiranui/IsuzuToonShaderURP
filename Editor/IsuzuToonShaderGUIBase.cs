using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using BlendMode = UnityEditor.BaseShaderGUI.BlendMode;
using RenderFace = UnityEditor.BaseShaderGUI.RenderFace;

namespace IsuzuToonShaderURP.Editor
{
    public abstract class IsuzuToonShaderGUIBase : ShaderGUI
    {
        protected enum SurfaceType
        {
            Opaque,
            Transparent,
            Fade
        }
        
        protected enum StencilMode
        {
            None,
            StencilOut,
            StencilMask
        }
        
        protected class Styles
        {
            public static readonly string[] SurfaceTypeNames = Enum.GetNames(typeof(SurfaceType));
            public static readonly string[] BlendModeNames = Enum.GetNames(typeof(BlendMode));
            public static readonly string[] RenderFaceNames = Enum.GetNames(typeof(RenderFace));
            
            public static readonly GUIContent SurfaceOptions =
                EditorGUIUtility.TrTextContent("Surface Options", "Controls how Universal RP renders the Material on a screen.");

            public static readonly GUIContent MainColorInputsLabel = EditorGUIUtility.TrTextContent("MainColor Inputs", "These settings describe the base look of the surface itself.");
            
            public static readonly GUIContent ShadowsControlLabel = EditorGUIUtility.TrTextContent("Shadows Control", "These settings control self or receive shadows.");
            
            public static readonly GUIContent NormalInputsLabel = EditorGUIUtility.TrTextContent("Normal Inputs", "These settings describe the base look of the surface normal itself.");
            public static readonly GUIContent SpecularInputsLabel = EditorGUIUtility.TrTextContent("Specular Inputs", "These settings describe the base look of the surface specular itself.");
            public static readonly GUIContent MetallicInputsLabel = EditorGUIUtility.TrTextContent("Metallic Inputs", "These settings describe the base look of the surface metallic itself.");
            public static readonly GUIContent MatCapInputsLabel = EditorGUIUtility.TrTextContent("MatCap Inputs", "These settings describe the base look of the surface MatCap itself.");
            public static readonly GUIContent SubsurfaceInputsLabel = EditorGUIUtility.TrTextContent("Subsurface Inputs", "These settings describe the base look of the subsurface itself.");
            public static readonly GUIContent RimInputsLabel = EditorGUIUtility.TrTextContent("Rim Inputs", "These settings describe the base look of the surface rim itself.");
            public static readonly GUIContent EmissionInputsLabel = EditorGUIUtility.TrTextContent("Emission Inputs", "These settings describe the base look of the surface emission itself.");
            public static readonly GUIContent LightColorBlendLabel = EditorGUIUtility.TrTextContent("Light Color Blend");
            public static readonly GUIContent AmbientLightControlsLabel = EditorGUIUtility.TrTextContent("Ambient Light Controls");
            public static readonly GUIContent OutlineControlsLabel = EditorGUIUtility.TrTextContent("Outline Controls");
            
            public static readonly GUIContent AdvancedLabel = EditorGUIUtility.TrTextContent("Advanced Options",
                                                                                             "These settings affect behind-the-scenes rendering and underlying calculations.");
            
            public static readonly GUIContent SurfaceType = EditorGUIUtility.TrTextContent("Surface Type",
                                                                                           "Select a surface type for your texture. Choose between Opaque or Transparent.");
            
            public static readonly GUIContent BlendingMode = EditorGUIUtility.TrTextContent("Blending Mode",
                                                                                            "Controls how the color of the Transparent surface blends with the Material color in the background.");

            public static readonly GUIContent CullingText = EditorGUIUtility.TrTextContent("Render Face",
                                                                                           "Specifies which faces to cull from your geometry. Front culls front faces. Back culls backfaces. None means that both sides are rendered.");

            public static readonly GUIContent AlphaClipText = EditorGUIUtility.TrTextContent("Alpha Clipping",
                                                                                             "Makes your Material act like a Cutout shader. Use this to create a transparent effect with hard edges between opaque and transparent areas.");

            public static readonly GUIContent AlphaClipThresholdText = EditorGUIUtility.TrTextContent("Threshold",
                                                                                                      "Sets where the Alpha Clipping starts. The higher the value is, the brighter the  effect is when clipping starts.");

            public static readonly GUIContent CastShadowText = EditorGUIUtility.TrTextContent("Cast Shadows",
                                                                                              "When enabled, this GameObject will cast shadows onto any geometry that can receive them.");

            public static readonly GUIContent ReceiveShadowText = EditorGUIUtility.TrTextContent("Receive All Shadows",
                                                                                                 "When enabled, other GameObjects can cast shadows onto this GameObject.");
            
            public static readonly GUIContent TransparentMaskText = EditorGUIUtility.TrTextContent("Transparent Mask");
            public static readonly GUIContent AlphaPreMultiplyText = EditorGUIUtility.TrTextContent("Alpha Pre Multiply");
            public static readonly GUIContent UseMainTexAlphaText = EditorGUIUtility.TrTextContent("Use Main Tex Alpha");
            public static readonly GUIContent InvertAlphaText = EditorGUIUtility.TrTextContent("Invert Alpha");
            
            public static readonly GUIContent MainTex = EditorGUIUtility.TrTextContent("Base Map / Color",
                                                                                       "Specifies the base Material and/or Color of the surface. If you’ve selected Transparent or Alpha Clipping under Surface Options, your Material uses the Texture’s alpha channel or color.");
            
            public static readonly GUIContent FirstShadeMap = EditorGUIUtility.TrTextContent("Shade Map 1 / Color",
                                                                                             "First shade color.");
            
            public static readonly GUIContent SecondShadeMap = EditorGUIUtility.TrTextContent("Shade Map 2 / Color", "Second shade color.");
            
            public static readonly GUIContent FirstShadeColorStep = EditorGUIUtility.TrTextContent("First Shade Step", "First shade position.");
            public static readonly GUIContent FirstShadeColorSoftness = EditorGUIUtility.TrTextContent("First Shade Softness", "First shade boundary degree.");
            
            public static readonly GUIContent SecondShadeColorStep = EditorGUIUtility.TrTextContent("Second Shade Step", "Second shade position.");
            public static readonly GUIContent SecondShadeColorSoftness = EditorGUIUtility.TrTextContent("Second Shade Softness", "Second shade boundary degree.");
            
            public static readonly GUIContent ReceiveSystemShadowText = EditorGUIUtility.TrTextContent("Receive System Shadow ", "When enabled, receive other object shadows.");
            public static readonly GUIContent SystemShadowLevel = EditorGUIUtility.TrTextContent("System Shadow Level", "Receive other object shadow level.");
            
            public static readonly GUIContent Specular = EditorGUIUtility.TrTextContent("Specular", "Control specular level.");
            public static readonly GUIContent SpecularMask = EditorGUIUtility.TrTextContent("Specular Mask / Color", "Mask where enable specular and control specular color.");
            public static readonly GUIContent SpecularStep = EditorGUIUtility.TrTextContent("Specular Step", "Specular edge position.");
            public static readonly GUIContent SpecularSoftness = EditorGUIUtility.TrTextContent("Specular Softness", "Specular boundary degree.");
            
            public static readonly GUIContent Metallic = EditorGUIUtility.TrTextContent("Metallic", "Control metallic level.");
            public static readonly GUIContent Smoothness = EditorGUIUtility.TrTextContent("Smoothness", "Control smoothness level.");
            public static readonly GUIContent MetallicMask = EditorGUIUtility.TrTextContent("Metallic Mask", "Mask where enable metallic.");
            
            public static readonly GUIContent MatCap = EditorGUIUtility.TrTextContent("MatCap", "Control MatCap level.");
            public static readonly GUIContent MatCapTex = EditorGUIUtility.TrTextContent("MatCap Tex / Color", "MatCap Texture and color.");
            public static readonly GUIContent MatCapMask = EditorGUIUtility.TrTextContent("MatCapMask", "Mask where enable MatCap.");
            
            public static readonly GUIContent UseSubsurfaceText = EditorGUIUtility.TrTextContent("Use Subsurface", "When enabled, add subsurface scattering effect.");
            public static readonly GUIContent SubsurfaceMask = EditorGUIUtility.TrTextContent("Subsurface Mask / Color", "Mask where enable subsurface and control color.");
            public static readonly GUIContent SubsurfaceScattering = EditorGUIUtility.TrTextContent("SubsurfaceScattering", "Control subsurface level.");
            public static readonly GUIContent SubsurfaceRadius = EditorGUIUtility.TrTextContent("SubsurfaceRadius", "Control subsurface area level.");
            
            public static readonly GUIContent UseRimText = EditorGUIUtility.TrTextContent("Use Rim", "When enabled, add rim light effect.");
            public static readonly GUIContent RimMask = EditorGUIUtility.TrTextContent("Rim Mask / Color", "Mask where enable rim and control color.");
            public static readonly GUIContent RimMin = EditorGUIUtility.TrTextContent("Rim Min", "Set rim min position.");
            public static readonly GUIContent RimMax = EditorGUIUtility.TrTextContent("Rim Max", "Set rim max position.");
            public static readonly GUIContent RimSmooth = EditorGUIUtility.TrTextContent("Rim Smooth", "Control rim edge smoothness level.");
            public static readonly GUIContent RimLightDirectionMaskText = EditorGUIUtility.TrTextContent("Rim LightDirection Mask", "When enabled, mask from light direction.");
            
            public static readonly GUIContent UseEmissiveText = EditorGUIUtility.TrTextContent("Use Emissive");
            public static readonly GUIContent EmissionMap = EditorGUIUtility.TrTextContent("Emission Map / Color", "Sets a Texture map to use for emission. You can also select a color with the color picker. Colors are multiplied over the Texture.");
            
            public static readonly GUIContent LightColorMainColor = EditorGUIUtility.TrTextContent("LightColor [MainColor]", "Reflect light color to main color.");
            public static readonly GUIContent LightColorFirstShadeColor = EditorGUIUtility.TrTextContent("LightColor [FirstShadeColor]", "Reflect light color to first shade color.");
            public static readonly GUIContent LightColorSecondShadeColor = EditorGUIUtility.TrTextContent("LightColor [SecondShadeColor]", "Reflect light color to second shade color.");
            public static readonly GUIContent LightColorRimColor = EditorGUIUtility.TrTextContent("LightColor [RimColor]", "Reflect light color to rim color.");
            public static readonly GUIContent LightColorSpecularColor = EditorGUIUtility.TrTextContent("LightColor [SpecularColor]", "Reflect light color to specular color.");
            public static readonly GUIContent LightColorSubsurfaceColor = EditorGUIUtility.TrTextContent("LightColor [SubsurfaceColor]", "Reflect light color to subsurface color.");
            
            public static readonly GUIContent AmbientColorBlend = EditorGUIUtility.TrTextContent("Ambient Color Blend", "Blend sky light.");
            public static readonly GUIContent GIIntensity = EditorGUIUtility.TrTextContent("GI Intensity", "GI intensity.");
            public static readonly GUIContent BacklightIntensity = EditorGUIUtility.TrTextContent("Backlight Intensity", "Backlight intensity.");
            public static readonly GUIContent BacklightOffset = EditorGUIUtility.TrTextContent("Backlight Offset", "Backlight offset.");
            
            public static readonly GUIContent UseOutlineText = EditorGUIUtility.TrTextContent("Use Outline");
            public static readonly GUIContent OutlineMask = EditorGUIUtility.TrTextContent("Outline Mask");
            public static readonly GUIContent OutlineThickness = EditorGUIUtility.TrTextContent("Outline Thickness");

            public static readonly GUIContent QueueSlider = EditorGUIUtility.TrTextContent("Sorting Priority",
                                                                                           "Determines the chronological rendering order for a Material. Materials with lower value are rendered first.");
        }
        
        protected const int QUEUE_OFFSET_RANGE = 50;
        
        private static readonly int EmissionEnabled = Shader.PropertyToID("_UseEmission");
        private static readonly int Normal = Shader.PropertyToID("_Normal");
        private static readonly int UseSubsurface = Shader.PropertyToID("_UseSubsurface");
        private static readonly int Stencil              = Shader.PropertyToID("_Stencil");
        private static readonly int Comp                 = Shader.PropertyToID("_Comp");
        private static readonly int Pass                 = Shader.PropertyToID("_Pass");
        private static readonly int Fail                 = Shader.PropertyToID("_Fail");

        private bool isInitialized;
        protected MaterialEditor MaterialEditor;
        protected Material Material;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (materialEditor == null) throw new ArgumentNullException(nameof(materialEditor));

            this.Material = materialEditor.target as Material;
            this.MaterialEditor = materialEditor;

            if (this.Material == null) return;
            
            if (!this.isInitialized)
            {
                this.OnOpenGUI(this.Material, materialEditor);
                this.isInitialized = true;
            }
            
            if (!materialEditor.isVisible) return;

            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200f;
            
            this.ShaderPropertiesGUI(properties);
            
            EditorGUIUtility.labelWidth = labelWidth;
        }

        protected abstract void OnOpenGUI(Material material, MaterialEditor materialEditor);

        protected abstract void ShaderPropertiesGUI(MaterialProperty[] properties);

        protected static void DrawFloatToggleProperty(GUIContent styles, MaterialProperty prop)
        {
            if (prop == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            var newValue = EditorGUILayout.Toggle(styles, Mathf.Approximately(prop.floatValue, 1));
            if (EditorGUI.EndChangeCheck())
                prop.floatValue = newValue ? 1.0f : 0.0f;
            EditorGUI.showMixedValue = false;
        }

        protected static void MaterialChanged(Material material)
        {
            if (material == null) throw new ArgumentNullException(nameof(material));
            SetMaterialKeywords(material);
        }

        private static void SetMaterialKeywords(Material material)
        {
            UpdateMaterialSurfaceOptions(material, true);
            UpdateMaterialOutlineOptions(material);
            
            // Setup double sided GI based on Cull state
            if (material.HasProperty(Property.CullMode))
            {
                var doubleSidedGI = (RenderFace)material.GetFloat(Property.CullMode) != RenderFace.Front;
                if (doubleSidedGI != material.doubleSidedGI)
                    material.doubleSidedGI = doubleSidedGI;
            }
            
            // Emission
            if (material.HasProperty(Property.EmissionColor))
                MaterialEditor.FixupEmissiveFlag(material);

            var shouldEmissionBeEnabled = false;
            if(material.HasProperty(EmissionEnabled))
                shouldEmissionBeEnabled = material.GetFloat(EmissionEnabled) >= 0.5f;

            CoreUtils.SetKeyword(material, ShaderKeywordStrings._EMISSION, shouldEmissionBeEnabled);
            
            // Normal Map
            if (material.HasProperty(Normal))
                CoreUtils.SetKeyword(material, ShaderKeywordStrings._NORMALMAP, material.GetTexture(Normal));

            // Subsurface
            var shouldSubsurfaceBeEnabled = false;
            if(material.HasProperty(UseSubsurface))
                shouldSubsurfaceBeEnabled = material.GetFloat(UseSubsurface) >= 0.5f;
            
            CoreUtils.SetKeyword(material, ShaderKeywordStrings._USESUBSURFACE_ON, shouldSubsurfaceBeEnabled);
        }

        private static void UpdateMaterialOutlineOptions(Material material)
        {
            if (material.HasProperty(Property.Outline))
                CoreUtils.SetKeyword(material, ShaderKeywordStrings._USEOUTLINE_ON, material.GetFloat(Property.Outline) != 0.0f);
        }


        private static void UpdateMaterialSurfaceOptions(Material material, bool automaticRenderQueue)
        {
            SetupMaterialBlendModeInternal(material, out var renderQueue);
            
            if (automaticRenderQueue && renderQueue != material.renderQueue)
                material.renderQueue = renderQueue;
            
            var castShadows = true;
            if (material.HasProperty(Property.CastShadows))
            {
                castShadows = material.GetFloat(Property.CastShadows) != 0.0f;
            }
            material.SetShaderPassEnabled("ShadowCaster", castShadows);
            
            // Receive Shadows
            if (material.HasProperty(Property.ReceiveShadows))
                CoreUtils.SetKeyword(material, ShaderKeywordStrings._RECEIVE_SHADOWS_OFF, material.GetFloat(Property.ReceiveShadows) == 0.0f);

            if (material.HasProperty("_Stencil"))
            {
                var stencilMode = (StencilMode)material.GetFloat(Stencil);
                switch (stencilMode)
                {
                    case StencilMode.None:
                        material.SetInt(Comp, (int)CompareFunction.Always);
                        material.SetInt(Pass, (int)StencilOp.Keep);
                        material.SetInt(Fail, (int)StencilOp.Keep);
                        break;
                    case StencilMode.StencilOut:
                        material.SetInt(Comp, (int)CompareFunction.NotEqual);
                        material.SetInt(Pass, (int)StencilOp.Keep);
                        material.SetInt(Fail, (int)StencilOp.Keep);
                        break;
                    case StencilMode.StencilMask:
                        material.SetInt(Comp, (int)CompareFunction.Always);
                        material.SetInt(Pass, (int)StencilOp.Replace);
                        material.SetInt(Fail, (int)StencilOp.Replace);
                        break;
                }
            }
        }

        private static void SetupMaterialBlendModeInternal(Material material, out int automaticRenderQueue)
        {
            if (material == null) throw new ArgumentNullException(nameof(material));
            
            var alphaClip = false;
            if (material.HasProperty(Property.AlphaClip))
                alphaClip = material.GetFloat(Property.AlphaClip) >= 0.5;
            CoreUtils.SetKeyword(material, ShaderKeywordStrings._ALPHATEST_ON, alphaClip);
            
            // default is to use the shader render queue
            var renderQueue = material.shader.renderQueue;
            material.SetOverrideTag("RenderType", "");      // clear override tag
            if (material.HasProperty(Property.SurfaceType))
            {
                var surfaceType = (SurfaceType)material.GetFloat(Property.SurfaceType);
                CoreUtils.SetKeyword(material, ShaderKeywordStrings._SURFACE_TYPE_TRANSPARENT, surfaceType == SurfaceType.Transparent);
                if (surfaceType == SurfaceType.Opaque)
                {
                    if (alphaClip)
                    {
                        renderQueue = (int)RenderQueue.AlphaTest;
                        material.SetOverrideTag("RenderType", "TransparentCutout");
                    }
                    else
                    {
                        renderQueue = (int)RenderQueue.Geometry;
                        material.SetOverrideTag("RenderType", "Opaque");
                    }

                    SetMaterialSrcDstBlendProperties(material, UnityEngine.Rendering.BlendMode.One, UnityEngine.Rendering.BlendMode.Zero);
                    material.DisableKeyword(ShaderKeywordStrings._ALPHAPREMULTIPLY_ON);
                    material.DisableKeyword(ShaderKeywordStrings._SURFACE_TYPE_TRANSPARENT);
                }
                else // SurfaceType Transparent
                {
                    var blendMode = (BlendMode)material.GetFloat(Property.BlendMode);

                    // Specific Transparent Mode Settings
                    switch (blendMode)
                    {
                        case BlendMode.Alpha:
                            SetMaterialSrcDstBlendProperties(material,
                                UnityEngine.Rendering.BlendMode.SrcAlpha,
                                UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            material.DisableKeyword(ShaderKeywordStrings._ALPHAPREMULTIPLY_ON);
                            break;
                        case BlendMode.Premultiply:
                            SetMaterialSrcDstBlendProperties(material,
                                UnityEngine.Rendering.BlendMode.One,
                                UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            material.EnableKeyword(ShaderKeywordStrings._ALPHAPREMULTIPLY_ON);
                            break;
                        case BlendMode.Additive:
                            SetMaterialSrcDstBlendProperties(material,
                                UnityEngine.Rendering.BlendMode.SrcAlpha,
                                UnityEngine.Rendering.BlendMode.One);
                            material.DisableKeyword(ShaderKeywordStrings._ALPHAPREMULTIPLY_ON);
                            break;
                        case BlendMode.Multiply:
                            SetMaterialSrcDstBlendProperties(material,
                                UnityEngine.Rendering.BlendMode.DstColor,
                                UnityEngine.Rendering.BlendMode.Zero);
                            material.DisableKeyword(ShaderKeywordStrings._ALPHAPREMULTIPLY_ON);
                            material.EnableKeyword(ShaderKeywordStrings._ALPHAMODULATE_ON);
                            break;
                    }

                    // General Transparent Material Settings
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.EnableKeyword(ShaderKeywordStrings._SURFACE_TYPE_TRANSPARENT);

                    switch (surfaceType)
                    {
                        case SurfaceType.Transparent:
                            renderQueue = (int) RenderQueue.Transparent;
                            break;
                        case SurfaceType.Fade:
                            renderQueue = (int) RenderQueue.AlphaTest;
                            break;
                    }
                }
            }

            // must always apply queue offset, even if not set to material control
            if (material.HasProperty(Property.QueueOffset))
                renderQueue += (int)material.GetFloat(Property.QueueOffset);

            automaticRenderQueue = renderQueue;
        }

        private static void SetMaterialSrcDstBlendProperties(Material material, UnityEngine.Rendering.BlendMode srcBlend, UnityEngine.Rendering.BlendMode dstBlend)
        {
            if (material.HasProperty(Property.SrcBlend))
                material.SetFloat(Property.SrcBlend, (float)srcBlend);

            if (material.HasProperty(Property.DstBlend))
                material.SetFloat(Property.DstBlend, (float)dstBlend);
        }

        protected void DrawShaderProperty(GUIContent label, MaterialProperty property, int labelIndent = 0)
        {
            if (property == null) return;
            this.MaterialEditor.ShaderProperty(property, label, labelIndent);
        }

        protected void DrawTexturePropertySingleLine(GUIContent label, MaterialProperty textureProp, MaterialProperty extraProperty1)
        {
            if (textureProp == null) return;
            this.MaterialEditor.TexturePropertySingleLine(label, textureProp, extraProperty1);
        }

        protected void DoPopup(GUIContent label, MaterialProperty property, string[] options)
        {
            if (property == null) return;
            this.MaterialEditor.PopupShaderProperty(property, label, options);
        }
    }
}