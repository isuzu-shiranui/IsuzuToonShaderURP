#ifndef ITS_OUTLINE_INCLUDED
#define ITS_OUTLINE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

CBUFFER_START(UnityPerMaterial)

half4 _OutlineMask_ST;
half _OutlineThickness;
half4 _OutlineColor;

CBUFFER_END

TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
TEXTURE2D(_OutlineMask); SAMPLER(sampler_OutlineMask);

struct VertexInput
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT;
    float2 uv           : TEXCOORD0;
    float4 vertexColor	: COLOR;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput
{
    float4 uv                       : TEXCOORD0;
    half3  normalWS                 : TEXCOORD1;
    float4 positionWSAndFogFactor   : TEXCOORD2; 
    float4 posWorld					: TEXCOORD3;
    float4 projPos					: TEXCOORD4;
    float4 fogFactorAndVertexLight  : TEXCOORD5;
    float3 wNormal                  : TEXCOORD6;
    float4 shadowCoord              : TEXCOORD7;
    float3 positionWS               : TEXCOORD8;
    float4 vertexColor				: COLOR;
    float4 clipPos               : SV_POSITION;
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 9);
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

VertexOutput Vert(VertexInput v)
{
    VertexOutput o = (VertexOutput)0;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.uv.xy = v.uv;
    o.vertexColor = v.vertexColor;

    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(v.normalOS, v.tangentOS);
    VertexPositionInputs positionInputs = GetVertexPositionInputs(v.positionOS.xyz);
    VertexPositionInputs vertexInput = (VertexPositionInputs)0;
    vertexInput.positionWS = positionInputs.positionWS;
    vertexInput.positionCS = positionInputs.positionCS;
    o.positionWS = float4(positionInputs.positionWS, positionInputs.positionNDC.w);
    o.shadowCoord = GetShadowCoord(vertexInput);

    o.normalWS = vertexNormalInput.normalWS;
    o.posWorld = mul(unity_ObjectToWorld, v.positionOS);

    half2 coord2 = TRANSFORM_TEX(v.uv, _OutlineMask);
    half4 outlineMask = SAMPLE_TEXTURE2D_LOD(_OutlineMask, sampler_OutlineMask, coord2, 0) * _OutlineThickness * 0.01;

    o.clipPos = mul(GetWorldToHClipMatrix(), mul(GetObjectToWorldMatrix(), float4(v.positionOS.xyz + v.normalOS.xyz * outlineMask.r, 1)));

    VertexNormalInputs normalInput = GetVertexNormalInputs(v.normalOS, v.tangentOS);
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    half fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
    o.fogFactorAndVertexLight = float4(fogFactor, vertexLight);
    o.uv.zw = GetNormalizedScreenSpaceUV(positionInputs.positionCS);
    OUTPUT_SH(o.wNormal.xyz, o.vertexSH);

    return o;
}

half4 Frag (const VertexOutput input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    half2 uv = input.uv.xy;

    half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
    float3 finalShade = mainTex * saturate(_MainLightColor);

    float3 color = finalShade * _OutlineColor;
    #if _USEOUTLINE_ON
    #else
    discard;
    #endif

    color = MixFog(_OutlineColor, input.fogFactorAndVertexLight.x);
    return half4(color, 1);
}

#endif