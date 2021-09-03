// ReSharper disable CppInconsistentNaming
#ifndef ITS_FORWARD_INPUT_INCLUDED
#define ITS_FORWARD_INPUT_INCLUDED

CBUFFER_START(UnityPerMaterial)

half _Cutoff;
half _Transparency;
half _UseMainTexAlpha;
half _InvertAlpha;

half4 _MainTex_ST;

half4 _BaseColor;
half _LightColor_MainColor;

half4 _FirstShadeColor;
half _LightColor_FirstShadeColor;

half4 _SecondShadeColor;
half _LightColor_SecondShadeColor;

half _FirstShadeColorStep;
half _FirstShadeSoftness ;

half _SecondShadeColorStep;
half _SecondShadeSoftness ;

half _ReceiveSystemShadow;
half _SystemShadowLevel;

half _NormalScale;

half _Specular;
half _SpecularPower;
half4 _SpecularColor;
half _SpecularStep;
half _SpecularSoftness;
half _LightColor_SpecularColor;

half _Metallic;
half _Smoothness;

half _MatCap;
half4 _MatCapColor;
half _LightColor_MatCapColor;

half _SubsurfaceRadius;
half4 _SubsurfaceColor;
half _SubsurfaceScattering;
half _LightColor_SubsurfaceColor;

half _RimMin;
half _RimMax;
half _RimSmooth;
half4 _RimColor;
half _RimLightDirectionMask;
half _LightColor_RimColor;

half4 _EmissionColor;

half _AmbientColorBlend;
half _GIIntensity;
half _BacklightOffset;
half _BacklightIntensity;

CBUFFER_END

TEXTURE2D(_TransparentMask); SAMPLER(sampler_TransparentMask);
TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
TEXTURE2D(_FirstShadeMap); SAMPLER(sampler_FirstShadeMap);
TEXTURE2D(_SecondShadeMap); SAMPLER(sampler_SecondShadeMap);
TEXTURE2D(_Normal); SAMPLER(sampler_Normal);
TEXTURE2D(_SpecularMask); SAMPLER(sampler_SpecularMask);
TEXTURE2D(_MetallicMask); SAMPLER(sampler_MetallicMask);
TEXTURE2D(_MatCapTex); SAMPLER(sampler_MatCapTex);
TEXTURE2D(_MatCapMask); SAMPLER(sampler_MatCapMask);
TEXTURE2D(_SubsurfaceMask); SAMPLER(sampler_SubsurfaceMask);
TEXTURE2D(_RimMask); SAMPLER(sampler_RimMask);
TEXTURE2D(_EmissionMap); SAMPLER(sampler_EmissionMap);

half3 SampleNormal(float2 uv, TEXTURE2D_PARAM(bumpMap, sampler_bumpMap), half scale = half(1.0))
{
    #ifdef _NORMALMAP
    half4 n = SAMPLE_TEXTURE2D(bumpMap, sampler_bumpMap, uv);
    #if BUMP_SCALE_NOT_SUPPORTED
    return UnpackNormal(n);
    #else
    return UnpackNormalScale(n, scale);
    #endif
    #else
    return half3(0.0h, 0.0h, 1.0h);
    #endif
}

half3 SampleEmission(float2 uv, half3 emissionColor, TEXTURE2D_PARAM(emissionMap, sampler_emissionMap))
{
    #ifndef _EMISSION
    return 0;
    #else
    return SAMPLE_TEXTURE2D(emissionMap, sampler_emissionMap, uv).rgb * emissionColor;
    #endif
}

#endif
