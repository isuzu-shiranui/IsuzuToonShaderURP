Shader "Hidden/Universal Render Pipeline/ITS/ScreenSpaceGradient"
{
    Properties
    {
        _MainTex("SourceTex", 2D) = "white" {}
    	_Intensity("Intensity", float) = 0
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_TopColor("Top Color", Color) = (0,0,0,0)
		_BottomColor("Bottom Color", Color) = (0,0,0,0)
		_Offset("Offset", float) = 0
    }
    
    HLSLINCLUDE

    #pragma exclude_renderers gles
    #pragma multi_compile_local _ _BLENDMODE_ADD _BLENDMODE_MULTIPLY

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

    TEXTURE2D_X(_MainTex);
    TEXTURE2D_X(_NoiseTex);

    float4 _TopColor;
    float4 _BottomColor;
    float _Offset;
    float _Intensity;

    float3 getNoise(float2 uv)
	{
		float3 noise = SAMPLE_TEXTURE2D_X(_NoiseTex, sampler_LinearClamp, uv * 100 + _Time * 50);
		noise = mad(noise, 2.0, -0.5);
		return noise / 255;
	}
    
    half4 Frag(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        half4 mainTex = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, input.uv);

    	float factor = mad(input.uv.y + _Offset, -0.5, 0.5);
		factor = clamp(factor, 0, 1);
    	
		half4 c = half4(0,0,0,0);
		c.rgb += getNoise(input.uv) + mainTex;
		c.rgb *= mainTex.a;

    	half4 finalColor = half4(0,0,0,0);
    	#if _BLENDMODE_ADD
    		finalColor = c + lerp(_TopColor, _BottomColor, factor);
    	#elif _BLENDMODE_MULTIPLY
    		finalColor = c * lerp(_TopColor, _BottomColor, factor);
    	#else
    		finalColor = c;
    	#endif
    	
        return lerp(c, finalColor, _Intensity);
    }

    ENDHLSL
    
    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "ScreenSpaceGradient"

            HLSLPROGRAM
                #pragma vertex FullscreenVert
                #pragma fragment Frag
            ENDHLSL
        }
    }
}