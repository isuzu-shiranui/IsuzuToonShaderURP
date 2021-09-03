Shader "IsuzuToonShader/Normal"
{
    Properties
    {
    	[Toggle][NoScaleOffset][Header(Transparency)]_AlphaTest("Alpha Clip", Float) = 0
        _Cutoff("Alpha Clipping", Range(0.0, 1.0)) = 0.5
        _Transparency("Transparency",  Range(-1 , 1)) = 0
        [NoScaleOffset]_TransparentMask("Transparent Mask", 2D) = "black" {}
    	[Toggle]_UseMainTexAlpha("Use MainTex Alpha", Float) = 1
	    [Toggle]_InvertAlpha("Invert Alpha", Float) = 0
    	
    	// Main Color
	    [Header(SurfaceInputs)][MainTexture]_MainTex ("Texture", 2D) = "white" {}
    	[MainColor]_BaseColor("Base Color", Color) = (1, 1, 1, 0)
    	
        _FirstShadeMap("Shade Map", 2D) = "white" {}
    	_FirstShadeColor("Shade Color", Color) = (1, 0.89, 0.87, 1)
    	
    	_SecondShadeMap("Second Shade Map", 2D) = "white" {}
    	_SecondShadeColor("Second Shade Color", Color) = (1, 0.89, 0.87, 1)
    	
    	// Shadow Control
    	_FirstShadeColorStep("First Shade Color Step", Range(0, 1)) = 0
		_FirstShadeSoftness ("First Shade Softness", Range(0, 1)) = 0
    	
    	_SecondShadeColorStep("Second Shade Color Step", Range(0, 1)) = 0
		_SecondShadeSoftness ("Second Shade Softness", Range(0, 1)) = 0
    	
    	[ToggleUI] _ReceiveSystemShadow("Receive System Shadows", Float) = 1.0
        _SystemShadowLevel ("System Shadow Level", Range(0, 1)) = 1
    	
    	// Normal
    	[Normal][Header(Normal Map)]_Normal("Normal", 2D) = "bump" {}
        _NormalScale("Normal Scale", Range(0, 10)) = 1.0
    	
    	// Specular
    	_Specular("Specular", Range(0, 1)) = 0
        [HDR]_SpecularColor("Specular Color", Color) = (0, 0, 0, 0)
    	_SpecularMask("Specular Mask", 2D) = "white" {}
    	_SpecularStep("Specular Step", Range(0.001, 1)) = 0.001
		_SpecularSoftness ("Specular Softness", Range(0.001, 1)) = 0.001
    	
    	// Metallic
    	_Metallic("Metallic", Range(0, 1)) = 0
    	_Smoothness("Smoothness", Range(0, 1)) = 1
    	_MetallicMask("Metallic Mask", 2D) = "white" {}
    	
    	// MatCap
    	_MatCap("MatCap", Range(0, 1)) = 0
    	[HDR]_MatCapColor("MatCap Color", Color) = (0, 0, 0, 0)
    	_MatCapTex("MatCap Tex", 2D) = "white" {}
    	_MatCapMask("MatCap Mask", 2D) = "white" {}
    	
    	// SSS
    	[Toggle(_USESUBSURFACE_ON)] _UseSubsurface("Use Subsurface", Float) = 0
		_SubsurfaceMask("Subsurface Mask", 2D) = "white" {}
        _SubsurfaceColor("Subsurface Color", Color) = (0, 0, 0, 0)
    	_SubsurfaceScattering("Subsurface Scattering", Range(0, 1)) = 1
    	_SubsurfaceRadius("Subsurface Radius", Range(0, 1)) = 1
    	
    	// Rim
	    [Toggle(_USERIM_ON)] _UseRim("Use Rim", Float) = 0
    	[NoScaleOffset]_RimMask("Rim Mask", 2D) = "white" {}
    	[HDR]_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_RimMin("Rim Min", Range(0, 1)) = 0.95
    	_RimMax("Rim Max", Range(0, 1)) = 1
    	_RimSmooth("Rim Smooth", Range(0.001, 1)) = 1
    	_RimLightDirectionMask("Rim LightDirection Mask", Range(0, 1)) = 1
    	
    	// Emission
    	[Header(Emission)][HDR][Toggle(_EMISSION)]_UseEmission("Use Emission", Float) = 0
    	_EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}    	
    	
    	// Light Color Blend
	    _LightColor_MainColor("Use LightColor to MainColor", Range(0, 1)) = 1
    	_LightColor_FirstShadeColor("Use LightColor to First Shade Color", Range(0, 1)) = 1
    	_LightColor_SecondShadeColor("Use LightColor to Second Shade Color", Range(0, 1)) = 1
    	_LightColor_RimColor("Use LightColor to Rim Color", Range(0, 1)) = 1
    	_LightColor_SpecularColor("Use LightColor to Specular Color", Range(0, 1)) = 1
    	_LightColor_SubsurfaceColor("Use LightColor to Subsurface Color", Range(0, 1)) = 1
    	_LightColor_MatCapColor("Use LightColor to MatCap Color", Range(0, 1)) = 1
	    
    	// Ambient Light
    	_AmbientColorBlend("Ambient Color Blend", Range(0, 1)) = 0.1
    	_GIIntensity("GI Intensity", Range(0, 1)) = 0
    	_BacklightOffset("Backlight Offset", Range(0, 1)) = 0
    	_BacklightIntensity("Backlight Intensity", Range(0, 1)) = 0
    	
    	// Outline
    	[Toggle(_USEOUTLINE_ON)]_UseOutline("Use Outline", Float) = 0
    	[NoScaleOffset]_OutlineMask("Outline Mask", 2D) = "white" {}
    	_OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
    	_OutlineThickness("Outline Width", Float) = 1
    	
    	// Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
    	[ToggleUI] _ReceiveShadows("Receive Shadows", Float) = 1.0
    	_QueueOffset("Queue offset", Float) = 0.0
    	
    	// Stencil
    	[HideInInspector] _Stencil("__stencil", Float) = 0.0
		[HideInInspector] _Reference("Reference", Range(0, 255)) = 2
		[HideInInspector] _Comp("__comp", Float) = 0.0
		[HideInInspector] _Pass("__pass", Float) = 0.0
		[HideInInspector] _Fail("__fail", Float) = 0.0
    }
    SubShader
    {
        Tags {"RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" "ShaderModel"="4.5"}
        LOD 100
    	
    	Pass
    	{
			Name "Outline"
    		Tags {  }
    		Cull Front
    		
    		HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

    		#pragma shader_feature_local _ _USEOUTLINE_ON

            #pragma multi_compile_fog
            #pragma multi_compile_instancing
    		
    		#pragma vertex Vert
            #pragma fragment Frag

    		#include "ITSOutline.hlsl"
    		ENDHLSL
		}
    	
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
        	
        	Stencil
        	{
                Ref[_Reference]
                Comp[_Comp]
                Pass[_Pass]
                Fail[_Fail]
            }

            Blend [_SrcBlend][_DstBlend]
            ZWrite On
            Cull [_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

			// -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local_fragment _ _SPECGLOSSMAP _SPECULAR_COLOR
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #pragma shader_feature_local_fragment _EMISSION

            #pragma shader_feature_local _ISFACE_ON
            #pragma shader_feature_local_fragment _USESUBSURFACE_ON
            #pragma shader_feature_local_fragment _USERIM_ON

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            // #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            // #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_fragment _ DEBUG_DISPLAY

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex Vert
            #pragma fragment Frag

            #include "ITSForward.hlsl"

            ENDHLSL
        }
    	
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
			ZTest LEqual
			ColorMask 0
        	Cull [_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma multi_compile_instancing

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
			ColorMask 0
			Cull [_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"

            ENDHLSL
        }
    	
    	Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitDepthNormalsPass.hlsl"
            ENDHLSL
        }
    }
    Fallback "Hidden/InternalErrorShader"
	CustomEditor "IsuzuToonShaderURP.Editor.IsuzuToonShader"
}