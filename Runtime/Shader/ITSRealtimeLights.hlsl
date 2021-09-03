// ReSharper disable CppInconsistentNaming
#ifndef ITS_REALTIME_LIGHTS_INCLUDED
#define ITS_REALTIME_LIGHTS_INCLUDED

struct ITSLight
{
    half3 direction;
    half3 color;
    half  distanceAttenuation;
    half  shadowAttenuation;
    uint  type;
    uint  layerMask;
};

half ITSMainLightRealtimeShadow(float4 shadowCoord)
{
    #if !defined(MAIN_LIGHT_CALCULATE_SHADOWS)
    return half(1.0);
    #elif defined(_MAIN_LIGHT_SHADOWS_SCREEN)
    return SampleScreenSpaceShadowmap(shadowCoord);
    #else
    ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
    half4 shadowParams = GetMainLightShadowParams();
    return SampleShadowmap(TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowCoord, shadowSamplingData, shadowParams, false);
    #endif
}

// returns 0.0 if position is in light's shadow
// returns 1.0 if position is in light
half AdditionalITSLightRealtimeShadow(int lightIndex, float3 positionWS, half3 lightDirection)
{
    #if !defined(ADDITIONAL_LIGHT_CALCULATE_SHADOWS)
    return half(1.0);
    #endif

    ShadowSamplingData shadowSamplingData = GetAdditionalLightShadowSamplingData();

    half4 shadowParams = GetAdditionalLightShadowParams(lightIndex);

    int shadowSliceIndex = shadowParams.w;


    if (shadowSliceIndex < 0)
        return 1.0;

    half isPointLight = shadowParams.z;

    UNITY_BRANCH
    if (isPointLight)
    {
        // This is a point light, we have to find out which shadow slice to sample from
        float cubemapFaceId = CubeMapFaceID(-lightDirection);
        shadowSliceIndex += cubemapFaceId;
    }

    #if USE_STRUCTURED_BUFFER_FOR_LIGHT_DATA
    float4 shadowCoord = mul(_AdditionalLightsWorldToShadow_SSBO[shadowSliceIndex], float4(positionWS, 1.0));
    #else
    float4 shadowCoord = mul(_AdditionalLightsWorldToShadow[shadowSliceIndex], float4(positionWS, 1.0));
    #endif

    return SampleShadowmap(TEXTURE2D_ARGS(_AdditionalLightsShadowmapTexture, sampler_AdditionalLightsShadowmapTexture), shadowCoord, shadowSamplingData, shadowParams, true);
}

ITSLight GetITSMainLight()
{
    ITSLight light;
    light.direction = half3(_MainLightPosition.xyz);
    
    #if USE_CLUSTERED_LIGHTING
    light.distanceAttenuation = 1.0;
    #else
    light.distanceAttenuation = unity_LightData.z; // unity_LightData.z is 1 when not culled by the culling mask, otherwise 0.
    #endif
    light.shadowAttenuation = 1.0;
    light.color = _MainLightColor.rgb;

    #ifdef _LIGHT_LAYERS
    light.layerMask = _MainLightLayerMask;
    #else
    light.layerMask = DEFAULT_LIGHT_LAYERS;
    #endif

    light.type = _MainLightPosition.w;

    return light;
}

ITSLight GetITSMainLight(float4 shadowCoord)
{
    ITSLight light = GetITSMainLight();
    light.shadowAttenuation = ITSMainLightRealtimeShadow(shadowCoord);

    #if defined(_LIGHT_COOKIES)
    real3 cookieColor = URP_LightCookie_SampleMainLightCookie(positionWS);
    light.color *= cookieColor;
    #endif
    
    return light;
}

ITSLight GetAdditionalPerObjectITSLight(int perObjectLightIndex, float3 positionWS)
{
    // Abstraction over Light input constants
    #if USE_STRUCTURED_BUFFER_FOR_LIGHT_DATA
    float4 lightPositionWS = _AdditionalLightsBuffer[perObjectLightIndex].position;
    half3 color = _AdditionalLightsBuffer[perObjectLightIndex].color.rgb;
    half4 distanceAndSpotAttenuation = _AdditionalLightsBuffer[perObjectLightIndex].attenuation;
    half4 spotDirection = _AdditionalLightsBuffer[perObjectLightIndex].spotDirection;
    #ifdef _LIGHT_LAYERS
    uint lightLayerMask = _AdditionalLightsBuffer[perObjectLightIndex].layerMask;
    #else
    uint lightLayerMask = DEFAULT_LIGHT_LAYERS;
    #endif

    #else
    float4 lightPositionWS = _AdditionalLightsPosition[perObjectLightIndex];
    half3 color = _AdditionalLightsColor[perObjectLightIndex].rgb;
    half4 distanceAndSpotAttenuation = _AdditionalLightsAttenuation[perObjectLightIndex];
    half4 spotDirection = _AdditionalLightsSpotDir[perObjectLightIndex];
    #ifdef _LIGHT_LAYERS
    uint lightLayerMask = asuint(_AdditionalLightsLayerMasks[perObjectLightIndex]);
    #else
    uint lightLayerMask = DEFAULT_LIGHT_LAYERS;
    #endif

    #endif

    // Directional lights store direction in lightPosition.xyz and have .w set to 0.0.
    // This way the following code will work for both directional and punctual lights.
    float3 lightVector = lightPositionWS.xyz - positionWS * lightPositionWS.w;
    float distanceSqr = max(dot(lightVector, lightVector), HALF_MIN);

    half3 lightDirection = half3(lightVector * rsqrt(distanceSqr));
    half attenuation = half(DistanceAttenuation(distanceSqr, distanceAndSpotAttenuation.xy) * AngleAttenuation(spotDirection.xyz, lightDirection, distanceAndSpotAttenuation.zw));

    ITSLight light;
    light.direction = lightDirection;
    light.distanceAttenuation = attenuation;
    light.shadowAttenuation = AdditionalITSLightRealtimeShadow(perObjectLightIndex, positionWS, lightDirection);
    light.color = color;
    light.type = lightPositionWS.w;
    light.layerMask = lightLayerMask;

    return light;
}

ITSLight GetAdditionalITSLight(uint i, float3 positionWS)
{
    #if USE_CLUSTERED_LIGHTING
    int lightIndex = i;
    #else
    int lightIndex = GetPerObjectLightIndex(i);
    #endif
    return GetAdditionalPerObjectITSLight(lightIndex, positionWS);
}

half3 GetLightColor(ITSLight light, InputData inputData)
{
    half3 color =  light.color * light.distanceAttenuation;
    color *= lerp(1, SampleSH(inputData.normalWS), _AmbientColorBlend);
    return color;
}

Light ITSLightToLight(ITSLight itsLight)
{
    Light light;
    light.color = itsLight.color;
    light.direction = itsLight.direction;
    light.distanceAttenuation = itsLight.distanceAttenuation;
    light.shadowAttenuation = itsLight.shadowAttenuation;
    light.layerMask = itsLight.layerMask;
    return light;
}

#endif
