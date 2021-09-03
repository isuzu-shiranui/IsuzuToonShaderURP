// ReSharper disable CppInconsistentNaming
// ReSharper disable CppLocalVariableMayBeConst
#ifndef ITS_LIGHTING_INCLUDED
#define ITS_LIGHTING_INCLUDED

half Pow5(half base)
{
    half a = base * base;
    return a * a * base;
}

float3 SampleEnvironment(InputData inputData, half smoothness, float2 uv)
{
    half metallicMap = SAMPLE_TEXTURE2D(_MetallicMask, sampler_MetallicMask, uv).r;
    float perceptualRoughness = PerceptualSmoothnessToRoughness(smoothness);
    half3 uvw = reflect(-inputData.viewDirectionWS, inputData.normalWS);
    float mip = PerceptualRoughnessToMipmapLevel(perceptualRoughness);
    float4 environment = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, uvw, mip);
    return DecodeHDREnvironment(environment, unity_SpecCube0_HDR) * metallicMap;
}

half HalfLambert(half3 normal, half3 lightDir)
{
    return 0.5 * dot(normal, lightDir) + 0.5;
}

half BackLighting(InputData inputData, ITSLight light)
{
     return Pow5(saturate(-dot(light.direction, inputData.viewDirectionWS) + _BacklightOffset)) * _BacklightIntensity;
}

half3 MatCap(InputData inputData, float2 uv)
{
    half3 normal = mul((float3x3)UNITY_MATRIX_V, inputData.normalWS);
    half3 detail = normal * float3(-1, -1, 1);
    half3 base = mul(UNITY_MATRIX_V, float4(inputData.viewDirectionWS, 0)).rgb*float3(-1, -1, 1) + float3(0, 0, 1);
    half3 noSknewViewNormal = base *dot(base, detail) / base.b - detail;
    half3 matCap = SAMPLE_TEXTURE2D(_MatCapTex, sampler_MatCapTex, noSknewViewNormal.xy * 0.5 + 0.5);
    half mask = SAMPLE_TEXTURE2D(_MatCapMask, sampler_MatCapMask, uv).r;
    return matCap * mask * _MatCapColor;
}

half3 SubsurfaceScattering(float2 uv, half shadow)
{
#if _USESUBSURFACE_ON
    half map = SAMPLE_TEXTURE2D(_SubsurfaceMask, sampler_SubsurfaceMask, uv).r;
    half alpha = _SubsurfaceRadius;
    half theta = max(0, shadow + alpha) - alpha;
    half normalization_jgt = (2 + alpha) / (2 * (1 + alpha));
    half wrapped_jgt = (pow((theta + alpha) / (1 + alpha), 1 + alpha)) * normalization_jgt;
    return  _SubsurfaceColor * wrapped_jgt * map;
#else
    return 0;
#endif
}

half Specular(InputData inputData, ITSLight light, float2 uv)
{
    half specularMap = SAMPLE_TEXTURE2D(_SpecularMask, sampler_SpecularMask, uv).r;
    half3 halfDir = normalize(light.direction + inputData.viewDirectionWS);
    half NdotH = 0.5 * saturate(dot(inputData.normalWS, halfDir)) + 0.5;
    // half ggx = D_GGX(NdotH, _Specular * _Specular);
    return NdotH * specularMap;
}

half3 Rim(InputData inputData, half shadow, float2 uv)
{
    #if _USERIM_ON
    half rimMask = SAMPLE_TEXTURE2D(_RimMask, sampler_RimMask, uv).r;
    half f = 1.0 - saturate(dot(inputData.viewDirectionWS, inputData.normalWS));
    half rim = smoothstep(_RimMin, _RimMax, f);
    rim /= _RimSmooth;
    rim *=_RimColor * rimMask;
    return lerp(rim, rim * shadow, _RimLightDirectionMask);
    #else
    return 0;
    #endif
}



LightingData CreateLightingData(InputData inputData, half3 emission)
{
    LightingData lightingData;

    lightingData.giColor = inputData.bakedGI;
    lightingData.emissionColor = emission;
    lightingData.vertexLightingColor = 0;
    lightingData.mainLightColor = 0;
    lightingData.additionalLightsColor = 0;

    return lightingData;
}

half3 LightingToon(InputData inputData, float2 uv, ITSLight mainLight)
{
    half3 mainLightColor = GetLightColor(mainLight, inputData);
    mainLightColor = max(0.03, mainLightColor * (1 - BackLighting(inputData, mainLight)));

    half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
    half4 firstShadeMap = SAMPLE_TEXTURE2D(_FirstShadeMap, sampler_FirstShadeMap, uv);
    half4 secondShadeMap = SAMPLE_TEXTURE2D(_SecondShadeMap, sampler_SecondShadeMap, uv);

    half3 mainTexColor = mainTex * _BaseColor;
    mainTexColor = lerp(mainTexColor, mainTexColor * mainLightColor, _LightColor_MainColor);

    half3 firstShadeColor = firstShadeMap * _FirstShadeColor;
    firstShadeColor = lerp(firstShadeColor, firstShadeColor * mainLightColor, _LightColor_FirstShadeColor);

    half3 secondShadeColor = secondShadeMap * _SecondShadeColor;
    secondShadeColor = lerp(secondShadeColor, secondShadeColor * mainLightColor, _LightColor_SecondShadeColor);

    half halfLambert = HalfLambert(inputData.normalWS, mainLight.direction);
    half systemShadow = saturate(0.5 * mainLight.shadowAttenuation + (_SystemShadowLevel * 0.5 + 0.01));
    half shadow = lerp(halfLambert, systemShadow * halfLambert, _ReceiveSystemShadow);

    half shade1Ramp = smoothstep(0, _FirstShadeSoftness, shadow - _FirstShadeColorStep);
    half shade2Ramp = smoothstep(0, _SecondShadeSoftness, shadow - _SecondShadeColorStep);
    half3 shadeColor = lerp(secondShadeColor, firstShadeColor, shade2Ramp);
    half3 shadedMainColor = lerp(shadeColor, mainTexColor, shade1Ramp);
    
    half3 color = shadedMainColor;

    half specular = Specular(inputData, mainLight, uv);
    half specularRamp = smoothstep(0, _SpecularSoftness, specular - (1 - _SpecularStep));
    half3 specularColor = lerp(color + _SpecularColor, color + _SpecularColor * mainLightColor, _LightColor_SpecularColor);
    color = lerp(color, specularColor, specularRamp);

    float3 metallic = SampleEnvironment(inputData, _Smoothness, uv);
    color = lerp(color, color * metallic, _Metallic);

    half3 rim = Rim(inputData, halfLambert, uv);
    rim = lerp(rim, rim * mainLightColor, _LightColor_RimColor);
    color += rim;

    half3 matCap = MatCap(inputData, uv);
    matCap = lerp(matCap, matCap * mainLightColor, _LightColor_MatCapColor);
    color += matCap * _MatCap;
    
    half3 subsurfaceContribution = SubsurfaceScattering(uv, halfLambert);
    subsurfaceContribution = lerp(subsurfaceContribution, subsurfaceContribution * mainLightColor, _LightColor_SubsurfaceColor);
    color *= 1 - _SubsurfaceScattering;
    color += subsurfaceContribution * _SubsurfaceScattering;

    return color;
}

half4 UniversalFragmentITS(InputData inputData, float2 uv)
{
    half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
    ITSLight mainLight = GetITSMainLight(inputData.shadowCoord);
    
    half3 emission = SampleEmission(uv, _EmissionColor, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
    LightingData lightingData = CreateLightingData(inputData, emission);

    BRDFData brdfData = (BRDFData)0;
    half alpha = 0;
    InitializeBRDFData(_BaseColor, _Metallic, _SpecularColor, _Smoothness, mainTex.a, brdfData);
    Light light = ITSLightToLight(mainLight);
    MixRealtimeAndBakedGI(light, inputData.normalWS, inputData.bakedGI);
    lightingData.giColor = ITSGlobalIllumination(brdfData, inputData.bakedGI, _GIIntensity, inputData.positionWS, inputData.normalWS, inputData.viewDirectionWS);
    
    uint meshRenderingLayers = GetMeshRenderingLightLayer();
    if(IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
    {
        lightingData.mainLightColor = LightingToon(inputData, uv, mainLight);
    }

#if defined(_ADDITIONAL_LIGHTS)
    uint pixelLightCount = GetAdditionalLightsCount();

    #if USE_CLUSTERED_LIGHTING
    for (uint lightIndex = 0; lightIndex < min(_AdditionalLightsDirectionalCount, MAX_VISIBLE_LIGHTS); lightIndex++)
    {
        ITSLight light = GetAdditionalITSLight(lightIndex, inputData.positionWS);

        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        {
            lightingData.additionalLightsColor += LightingToon(inputData, uv, light);
        }
    }
    #endif

    LIGHT_LOOP_BEGIN(pixelLightCount)
    ITSLight light = GetAdditionalITSLight(lightIndex, inputData.positionWS);

    if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
    {
        lightingData.additionalLightsColor += LightingToon(inputData, uv, light);
    }
    LIGHT_LOOP_END
#endif

    #if defined(_ADDITIONAL_LIGHTS_VERTEX)
    lightingData.vertexLightingColor += inputData.vertexLighting;
    #endif
    
    return CalculateFinalColor(lightingData, mainTex.a);
}

#endif