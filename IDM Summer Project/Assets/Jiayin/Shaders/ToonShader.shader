Shader "Custom/URPToonShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color0 ("Color0", Color) = (1,1,1,1)
        _Color1 ("Color1", Color) = (1,1,1,1)
        _Color2("Color2",Color)=(1,1,1,1)
        _Color3("Color3",Color)=(1,1,1,1)
        _GlossyColor("GlossyColor",Color)=(0,0,0,1)
        _GlossyAmount("GlossyAmount",float)=0
        _HeightFactor("Height Factor", float) = 1
        _HeightMove("Height Move",float) = 0
        _HeightTex("Height Texture", 2D) = "white" {}
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _StylishShadow ("Stylish Shadow", Float) = 0.5
        _ShadowColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
        _ShadowColor2("ShadowColor2",color)=(0.5,0.5,0.5,0.5)
        _OutLineColor("OutlineColor",Color)=(0,0,0,1)
        _OutLineBlend("OutlineBlend",Range(0,1)) = 1  
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" 
            "Queue"="Geometry" "UniversalMaterialType"="Lit" "DecalMeshForwardEmissive" = "True" }
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
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
                float4 screenUV : TEXCOORD4; 
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color0;
                float4 _Color1;
                float4 _Color2;
                float4 _Color3;
                float _ShadowThreshold;
                float4 _ShadowColor;
                float4 _ShadowColor2;
                float _StylishShadow;//shadow edge 
                float4 _OutLineColor;
                float _OutLineBlend;
                //height mix
                float _HeightFactor;
                float _HeightMove;
                //HDR
                float3 _GlossyColor;
               float _GlossyAmount;

                float4 _HeightTex_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalLineTex);
            SAMPLER(sampler_NormalLineTex);
            TEXTURE2D(_HeightTex); SAMPLER(sampler_HeightTex);
            
            TEXTURE2D(_DepthTex);
            SAMPLER(sampler_DepthTex);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv ;
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
                OUT.screenUV=ComputeScreenPos(OUT.positionCS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                //fog
                float3 positionOS= TransformWorldToObject(IN.positionWS);
                float heightMap=positionOS.y*_HeightFactor+_HeightMove;
               // return half4(heightMap.xxx,1);
                float heightTex=SAMPLE_TEXTURE2D(_HeightTex, sampler_HeightTex, IN.uv*_HeightTex_ST.xy+_HeightTex_ST.zw).r;
                _Color2.rgb = lerp(_Color2.rgb, _Color3.rgb, saturate(heightTex));
                _Color0.rgb = lerp(_Color0.rgb, _Color1.rgb, saturate(heightTex));
                float Grassmask =saturate( floor(heightMap*heightTex*10/3)/2);
                float3 BaseColor= lerp(_Color0.rgb, _Color2.rgb, Grassmask);
                half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
                clip(albedo-0.02);
                albedo*=BaseColor;

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
              //  half shadowAtten = mainLight.shadowAttenuation;
                half shadowAtten=MainLightRealtimeShadow(IN.shadowCoord);
             //  return half4(NdotL * shadowAtten.xxx,1);
                half toonStep1 = NdotL * shadowAtten > _ShadowThreshold ? 1.0: 0.0;
                half toonStep2 = NdotL * shadowAtten>_ShadowThreshold + _StylishShadow ? 1.0 : 0.0;
          
                //decal support              

                float4 shadowColor=lerp(_ShadowColor, _ShadowColor2, NdotL * shadowAtten);
                half3 baseColor = lerp(shadowColor.rgb*albedo*totalLight, albedo * totalLight, toonStep2);
                baseColor+=_GlossyColor*_GlossyAmount;
                ApplyDecalToBaseColor(IN.positionCS, baseColor);
                return half4(baseColor, 1.0);
            }
            ENDHLSL
        }

        // 阴影投射
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"
   
// Pass
// {
//     Name "DepthNormals"
//     Tags { "LightMode"="DepthNormals" }
//     ZWrite On
//     ColorMask RGBA
//     Cull Off
//     HLSLPROGRAM
//     #pragma vertex vert
//     #pragma fragment frag

//     #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

//     CBUFFER_START(UnityPerMaterial)
//     CBUFFER_END

//      TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
//     struct Attributes
//     {
//         float4 positionOS : POSITION;
//         float3 normalOS : NORMAL;
//         float2 uv : TEXCOORD0;
//     };

//     struct Varyings
//     {
//         float4 positionCS : SV_POSITION;
//         float3 normalWS : TEXCOORD0;
//         float2 uv:TEXCOORD1;
//     };

//     Varyings vert(Attributes IN)
//     {
//         Varyings OUT;
//             float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
//             OUT.positionCS = TransformWorldToHClip(positionWS);
//             OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
//             OUT.uv=IN.uv;
//         return OUT;
//     }

//     float4 frag(Varyings IN) : SV_Target
//     {
//         half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
//         clip(albedo-0.02);
//         float3 normal = normalize(IN.normalWS);
//         return float4(normal * 0.5 + 0.5, 1.0);
//     }
//     ENDHLSL
// }
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"

    }
    FallBack "Universal Render Pipeline/Lit"
}