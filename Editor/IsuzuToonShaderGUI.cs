using UnityEditor;
using static UnityEditor.BaseShaderGUI;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace IsuzuToonShaderURP.Editor
{
    internal static class IsuzuToonShaderGUI
    {
        public readonly struct IsuzuToonShaderProperties
        {
            public readonly MaterialProperty surfaceTypeProp;
            public readonly MaterialProperty blendModeProp;
            public readonly MaterialProperty cullingProp;
            public readonly MaterialProperty alphaClipProp;
            public readonly MaterialProperty castShadowsProp;
            public readonly MaterialProperty receiveShadowsProp;
            public readonly MaterialProperty referenceProp;
            public readonly MaterialProperty cutoffProp;
            public readonly MaterialProperty transparencyProp;
            public readonly MaterialProperty transparentMaskProp;
            public readonly MaterialProperty useMainTexAlphaProp;
            public readonly MaterialProperty invertAlphaProp;
            public readonly MaterialProperty mainTexProp;
            public readonly MaterialProperty baseColorProp;
            public readonly MaterialProperty firstShadeMapProp;
            public readonly MaterialProperty firstShadeColorProp;
            public readonly MaterialProperty secondShadeMapProp;
            public readonly MaterialProperty secondShadeColorProp;
            public readonly MaterialProperty firstShadeColorStepProp;
            public readonly MaterialProperty firstShadeSoftnessProp;
            public readonly MaterialProperty secondShadeColorStepProp;
            public readonly MaterialProperty secondShadeSoftnessProp;
            public readonly MaterialProperty receiveSystemShadowProp;
            public readonly MaterialProperty systemShadowLevelProp;
            public readonly MaterialProperty normalProp;
            public readonly MaterialProperty normalScaleProp;
            public readonly MaterialProperty specularProp;
            public readonly MaterialProperty specularColorProp;
            public readonly MaterialProperty specularMaskProp;
            public readonly MaterialProperty specularStepProp;
            public readonly MaterialProperty specularSoftnessProp;
            public readonly MaterialProperty metallicProp;
            public readonly MaterialProperty smoothnessProp;
            public readonly MaterialProperty metallicMaskProp;
            public readonly MaterialProperty matCapProp;
            public readonly MaterialProperty matCapColorProp;
            public readonly MaterialProperty matCapTexProp;
            public readonly MaterialProperty matCapMaskProp;
            public readonly MaterialProperty useSubsurfaceProp;
            public readonly MaterialProperty subsurfaceMaskProp;
            public readonly MaterialProperty subsurfaceColorProp;
            public readonly MaterialProperty subsurfaceScatteringProp;
            public readonly MaterialProperty subsurfaceRadiusProp;
            public readonly MaterialProperty useRimProp;
            public readonly MaterialProperty rimMaskProp;
            public readonly MaterialProperty rimColorProp;
            public readonly MaterialProperty rimMinProp;
            public readonly MaterialProperty rimMaxProp;
            public readonly MaterialProperty rimSmoothProp;
            public readonly MaterialProperty rimLightDirectionMaskProp;
            public readonly MaterialProperty useEmissionProp;
            public readonly MaterialProperty emissionColorProp;
            public readonly MaterialProperty emissionMapProp;
            public readonly MaterialProperty lightColorMainColorProp;
            public readonly MaterialProperty lightColorFirstShadeColorProp;
            public readonly MaterialProperty lightColorSecondShadeColorProp;
            public readonly MaterialProperty lightColorRimColorProp;
            public readonly MaterialProperty lightColorSpecularColorProp;
            public readonly MaterialProperty lightColorSubsurfaceColorProp;
            public readonly MaterialProperty ambientColorBlendProp;
            public readonly MaterialProperty gIIntensityProp;
            public readonly MaterialProperty backlightOffsetProp;
            public readonly MaterialProperty backlightIntensityProp;
            public readonly MaterialProperty useOutlineProp;
            public readonly MaterialProperty outlineMaskProp;
            public readonly MaterialProperty outlineColorProp;
            public readonly MaterialProperty outlineThicknessProp;
            public readonly MaterialProperty queueOffsetProp;
            public readonly MaterialProperty stencilProp;
            public readonly MaterialProperty compProp;
            public readonly MaterialProperty passProp;
            public readonly MaterialProperty failProp;
            public IsuzuToonShaderProperties(MaterialProperty[] properties)
            {
                this.surfaceTypeProp                = FindProperty(Property.SurfaceType,           properties, false);
                this.blendModeProp                  = FindProperty(Property.BlendMode,             properties, false);
                this.cullingProp                    = FindProperty(Property.CullMode,              properties, false);
                this.alphaClipProp                  = FindProperty(Property.AlphaClip,             properties, false);
                this.receiveShadowsProp             = FindProperty(Property.ReceiveShadows,        properties, false);
                this.castShadowsProp                = FindProperty(Property.CastShadows,           properties, false);
                this.referenceProp                  = FindProperty("_Reference",                   properties, false);
                this.cutoffProp                     = FindProperty("_Cutoff",                      properties, false);
                this.transparencyProp               = FindProperty("_Transparency",                properties, false);
                this.transparentMaskProp            = FindProperty("_TransparentMask",             properties, false);
                this.useMainTexAlphaProp            = FindProperty("_UseMainTexAlpha",             properties, false);
                this.invertAlphaProp                = FindProperty("_InvertAlpha",                 properties, false);
                this.mainTexProp                    = FindProperty("_MainTex",                     properties, false);
                this.baseColorProp                  = FindProperty("_BaseColor",                   properties, false);
                this.firstShadeMapProp              = FindProperty("_FirstShadeMap",               properties, false);
                this.firstShadeColorProp            = FindProperty("_FirstShadeColor",             properties, false);
                this.secondShadeMapProp             = FindProperty("_SecondShadeMap",              properties, false);
                this.secondShadeColorProp           = FindProperty("_SecondShadeColor",            properties, false);
                this.firstShadeColorStepProp        = FindProperty("_FirstShadeColorStep",         properties, false);
                this.firstShadeSoftnessProp         = FindProperty("_FirstShadeSoftness",          properties, false);
                this.secondShadeColorStepProp       = FindProperty("_SecondShadeColorStep",        properties, false);
                this.secondShadeSoftnessProp        = FindProperty("_SecondShadeSoftness",         properties, false);
                this.receiveSystemShadowProp        = FindProperty("_ReceiveSystemShadow",         properties, false);
                this.systemShadowLevelProp          = FindProperty("_SystemShadowLevel",           properties, false);
                this.normalProp                     = FindProperty("_Normal",                      properties, false);
                this.normalScaleProp                = FindProperty("_NormalScale",                 properties, false);
                this.specularProp                   = FindProperty("_Specular",                    properties, false);
                this.specularColorProp              = FindProperty("_SpecularColor",               properties, false);
                this.specularMaskProp               = FindProperty("_SpecularMask",                properties, false);
                this.specularStepProp               = FindProperty("_SpecularStep",                properties, false);
                this.specularSoftnessProp           = FindProperty("_SpecularSoftness",            properties, false);
                this.metallicProp                   = FindProperty("_Metallic",                    properties, false);
                this.smoothnessProp                 = FindProperty("_Smoothness",                  properties, false);
                this.metallicMaskProp               = FindProperty("_MetallicMask",                properties, false);
                this.matCapProp                     = FindProperty("_MatCap",                      properties, false);
                this.matCapColorProp                = FindProperty("_MatCapColor",                 properties, false);
                this.matCapTexProp                  = FindProperty("_MatCapTex",                   properties, false);
                this.matCapMaskProp                 = FindProperty("_MatCapMask",                  properties, false);
                this.useSubsurfaceProp              = FindProperty("_UseSubsurface",               properties, false);
                this.subsurfaceMaskProp             = FindProperty("_SubsurfaceMask",              properties, false);
                this.subsurfaceColorProp            = FindProperty("_SubsurfaceColor",             properties, false);
                this.subsurfaceScatteringProp       = FindProperty("_SubsurfaceScattering",        properties, false);
                this.subsurfaceRadiusProp           = FindProperty("_SubsurfaceRadius",            properties, false);
                this.useRimProp                     = FindProperty("_UseRim",                      properties, false);
                this.rimMaskProp                    = FindProperty("_RimMask",                     properties, false);
                this.rimColorProp                   = FindProperty("_RimColor",                    properties, false);
                this.rimMinProp                     = FindProperty("_RimMin",                      properties, false);
                this.rimMaxProp                     = FindProperty("_RimMax",                      properties, false);
                this.rimSmoothProp                  = FindProperty("_RimSmooth",                   properties, false);
                this.rimLightDirectionMaskProp      = FindProperty("_RimLightDirectionMask",       properties, false);
                this.useEmissionProp                = FindProperty("_UseEmission",                 properties, false);
                this.emissionColorProp              = FindProperty("_EmissionColor",               properties, false);
                this.emissionMapProp                = FindProperty("_EmissionMap",                 properties, false);
                this.lightColorMainColorProp        = FindProperty("_LightColor_MainColor",        properties, false);
                this.lightColorFirstShadeColorProp  = FindProperty("_LightColor_FirstShadeColor",  properties, false);
                this.lightColorSecondShadeColorProp = FindProperty("_LightColor_SecondShadeColor", properties, false);
                this.lightColorRimColorProp         = FindProperty("_LightColor_RimColor",         properties, false);
                this.lightColorSpecularColorProp    = FindProperty("_LightColor_SpecularColor",    properties, false);
                this.lightColorSubsurfaceColorProp  = FindProperty("_LightColor_SubsurfaceColor",  properties, false);
                this.ambientColorBlendProp          = FindProperty("_AmbientColorBlend",           properties, false);
                this.gIIntensityProp                = FindProperty("_GIIntensity",                 properties, false);
                this.backlightOffsetProp            = FindProperty("_BacklightOffset",             properties, false);
                this.backlightIntensityProp         = FindProperty("_BacklightIntensity",          properties, false);
                this.useOutlineProp                 = FindProperty("_UseOutline",                  properties, false);
                this.outlineMaskProp                = FindProperty("_OutlineMask",                 properties, false);
                this.outlineColorProp               = FindProperty("_OutlineColor",                properties, false);
                this.outlineThicknessProp           = FindProperty("_OutlineThickness",            properties, false);
                this.queueOffsetProp                = FindProperty("_QueueOffset",                 properties, false);
                this.stencilProp                    = FindProperty("_Stencil",                     properties, false);
                this.compProp                       = FindProperty("_Comp",                        properties, false);
                this.passProp                       = FindProperty("_Pass",                        properties, false);
                this.failProp                       = FindProperty("_Fail",                        properties, false);
            }
        }
    }
}