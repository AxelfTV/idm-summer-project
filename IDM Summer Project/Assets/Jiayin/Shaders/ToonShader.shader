Shader "Custom/URPToonShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _StylishShadow ("Stylish Shadow", Float) = 0.5
        _ShadowColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
        _ShadowColor2("ShadowColor2",color)=(0.5,0.5,0.5,0.5)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" 
            "Queue"="Geometry" "UniversalMaterialType"="Lit"}
        //LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            ZWrite On
            ZTest LEqual
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _ShadowThreshold;
                float4 _ShadowColor;
                float4 _ShadowColor2;
                float _StylishShadow;//shadow edge 
            CBUFFER_END

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv ;
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
               // return half4(albedo,1);
                Light mainLight = GetMainLight(IN.shadowCoord);

               //point light support
                half3 totalLight = mainLight.color.rgb * mainLight.distanceAttenuation;
                #if defined(_ADDITIONAL_LIGHTS)
                uint lightCount = GetAdditionalLightsCount();
                for (uint i = 0; i < lightCount; ++i)
                {
                    Light addLight = GetAdditionalLight(i, IN.positionWS);
                    totalLight += addLight.color.rgb * addLight.distanceAttenuation;
                }
                #endif

                // Toon shadow
                half NdotL = saturate(dot(IN.normalWS, mainLight.direction));
                half shadowAtten = mainLight.shadowAttenuation;
             //  return half4(NdotL * shadowAtten.xxx,1);
                half toonStep1 = NdotL * shadowAtten > _ShadowThreshold ? 1.0: 0.0;
                half toonStep2 = NdotL * shadowAtten>_ShadowThreshold + _StylishShadow ? 1.0 : 0.0;
          
                float4 shadowColor=lerp(_ShadowColor, _ShadowColor2, NdotL * shadowAtten);
                half3 color = lerp(shadowColor.rgb*albedo*totalLight, albedo * totalLight, toonStep2);

                return half4(color, 1.0);
            }
            ENDHLSL
        }

        // 阴影投射
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"

    }
    FallBack "Universal Render Pipeline/Lit"
}