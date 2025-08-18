Shader "Unlit/WindRing"
{

    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color0 ("Color0", Color) = (1,1,1,1)
        _Color1 ("Color1", Color) = (1,1,1,1)
        _Color2("Color2",Color)=(1,1,1,1)
        _Color3("Color3",Color)=(1,1,1,1)

        _HeightFactor("Height Factor", float) = 1
        _HeightMove("Height Move",float) = 0
        _HeightTex("Height Texture", 2D) = "white" {}

        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _StylishShadow ("Stylish Shadow", Float) = 0.5
        _ShadowColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
        _ShadowColor2("ShadowColor2",color)=(0.5,0.5,0.5,0.5)

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline" 
            "Queue"="Transparent" "UniversalMaterialType"="Lit" }
        //LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "../ShaderTools/NoiseLib.hlsl"

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
                float _StylishShadow;

                //height mix
                float _HeightFactor;
                float _HeightMove;

                float4 _HeightTex_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_HeightTex); SAMPLER(sampler_HeightTex);
            


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

            float4 frag(Varyings IN) : SV_Target
            {
                float3 positionOS= TransformWorldToObject(IN.positionWS);

                // wind
                
                float2 winduv= IN.uv - 0.5;
                float WindNoise=voronoiNoise(IN.uv+_Time.y);
                float r = length(winduv);
                float theta = atan2(winduv.y, winduv.x);
                float spiral = sin(theta * 2 + r *20 - _Time.y* 15);
                float spiral2 = sin(theta  + r *20 - _Time.y* 12);
                float spiral3 = sin(theta  + r *20 - _Time.y*5);

                float mixspiral=saturate( spiral*spiral2* spiral3*WindNoise)*r;
                    mixspiral=mixspiral*10;
                 float band = smoothstep(0.2, 0.8,  mixspiral)*WindNoise;
                 float alpha = saturate(1.0 - r * 1.5);
                float3 col = lerp(float3(0.0, 0.3, 0.7), float3(0.9, 0.9, 1.0), band);

              
                return float4( mixspiral.xxxx);

                float heightMap=positionOS.y*_HeightFactor+_HeightMove;
                float heightTex=SAMPLE_TEXTURE2D(_HeightTex, sampler_HeightTex, IN.uv*_HeightTex_ST.xy+_HeightTex_ST.zw).r;
                _Color2.rgb = lerp(_Color2.rgb, _Color3.rgb, saturate(heightTex));
                _Color0.rgb = lerp(_Color0.rgb, _Color1.rgb, saturate(heightTex));
                float Grassmask =saturate( heightMap*heightTex);
                
                float3 BaseColor= lerp(_Color0.rgb, _Color2.rgb, Grassmask);
                float3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
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
                float NdotL = saturate(dot(IN.normalWS, mainLight.direction));
                float shadowAtten=MainLightRealtimeShadow(IN.shadowCoord);
                float toonStep1 = NdotL * shadowAtten > _ShadowThreshold ? 1.0: 0.0;
                float toonStep2 = NdotL * shadowAtten>_ShadowThreshold + _StylishShadow ? 1.0 : 0.0;        

                float4 shadowColor=lerp(_ShadowColor, _ShadowColor2, NdotL * shadowAtten);
                float3 baseColor = lerp(shadowColor.rgb*albedo*totalLight, albedo * totalLight, toonStep2); 
                return float4(baseColor, 1.0);
            }
            ENDHLSL
        }

 
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"

    }
    FallBack "Universal Render Pipeline/Lit"
}
