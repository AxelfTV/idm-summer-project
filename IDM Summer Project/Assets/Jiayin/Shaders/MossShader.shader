Shader "Custom/MossShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color0 ("Color0", Color) = (1,1,1,1)
        _Color1 ("Color1", Color) = (1,1,1,1)

        //moss Texture
        _MossBase("MossBase",2D)="white"{}
        _MossNormal("MossNormal",2D)="white"{}
        _MossHeight("MossHeight",2D)="white"{}
        //moss params
        _MossOffset("MossOffset",float)=1
        _MossCut("Moss Cut",float)=0
        _NormalStrength("NormalStrength",float)=1

        //map moss Range
        _HeightFactor("Height Factor", float) = 1
        _HeightMove("Height Move",float) = 0
        _HeightTex("Height Texture", 2D) = "white" {}

        //lighting
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _StylishShadow ("Stylish Shadow", Float) = 0.5
        _ShadowColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
        _ShadowColor2("ShadowColor2",color)=(0.5,0.5,0.5,0.5)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" 
            "Queue"="Geometry" "UniversalMaterialType"="Lit"  }
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
                float4 tangentOS:TANGENT;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
                float4 screenUV : TEXCOORD4; 
                float MossMask:TEXCOORD5;

                float3 tangentWS : TEXCOORD6;
                float3 binormalWS : TEXCOORD7;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color0;
                float4 _Color1;
                //moss
                float _MossOffset;
                float _MossCut;
                float _NormalStrength;

                //lighting
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
            //moss textures
             TEXTURE2D(_MossBase);SAMPLER(sampler_MossBase);
             TEXTURE2D(_MossNormal);SAMPLER(sampler_MossNormal);
             TEXTURE2D(_MossHeight);SAMPLER(sampler_MossHeight);
        
            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                //moss mask
                float heightMap=saturate(IN.positionOS.y*_HeightFactor+_HeightMove);
                float heightTex=SAMPLE_TEXTURE2D_LOD(_HeightTex, sampler_HeightTex, IN.uv*_HeightTex_ST.xy+_HeightTex_ST.zw,0).x;
                float MossMask=saturate( pow(heightMap+heightTex,_MossCut));

                float MossHeight= SAMPLE_TEXTURE2D_LOD(_MossHeight, sampler_MossHeight, IN.uv, 0).x;
                IN.positionOS.xyz+=MossHeight*IN.normalOS*_MossOffset*MossMask;

                OUT.MossMask=MossMask;

                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv ;
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
                OUT.screenUV=ComputeScreenPos(OUT.positionCS);

                OUT.tangentWS=normalize(TransformObjectToWorldDir(IN.tangentOS.xyz));
                OUT.binormalWS=cross(OUT.normalWS,OUT.tangentWS)*IN.tangentOS.w;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float3 BaseColor= lerp(_Color0.rgb, _Color1.rgb,IN.MossMask);
                float3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb*BaseColor;
              //  return float4(IN.MossMask.xxx,1);

                //moss color
                float3 MossColor=SAMPLE_TEXTURE2D(_MossBase,sampler_MossBase,IN.uv).rgb;
                float height=SAMPLE_TEXTURE2D(_MossHeight, sampler_MossHeight, IN.uv).z*_MossOffset;
                albedo=lerp(albedo,MossColor,IN.MossMask);

                Light mainLight = GetMainLight(IN.shadowCoord);
                
               //point light 
               float3 totalLight = mainLight.color.rgb * mainLight.distanceAttenuation;
                #if defined(_ADDITIONAL_LIGHTS)
                uint lightCount = GetAdditionalLightsCount();
                for (uint i = 0; i < lightCount; ++i)
                {
                    Light addLight = GetAdditionalLight(i, IN.positionWS);
                    totalLight += addLight.color.rgb * addLight.distanceAttenuation;
                }
                #endif

                //normal map
                float3 normalTS=SAMPLE_TEXTURE2D(_MossNormal,sampler_MossNormal,IN.uv).xyz*2-1;
                normalTS = normalize(lerp(float3(0,0,1), normalTS, _NormalStrength));
                float3x3 TBN=float3x3(IN.tangentWS, IN.binormalWS,IN.normalWS);
                float3 normalWS=lerp(IN.normalWS,normalize(mul(normalTS,TBN)),IN.MossMask);

                //shadow
                float NdotL = saturate(dot(normalWS, mainLight.direction));
                float shadowAtten=MainLightRealtimeShadow(IN.shadowCoord);
        
                half toonStep1 = NdotL * shadowAtten > _ShadowThreshold ? 1.0: 0.0;
                half toonStep2 = NdotL * shadowAtten>_ShadowThreshold + _StylishShadow ? 1.0 : 0.0;

                float4 shadowColor=lerp(_ShadowColor, _ShadowColor2, NdotL * shadowAtten);
                float3 baseColor = lerp(shadowColor.rgb*albedo*totalLight, albedo * totalLight, toonStep2); 
                return float4(baseColor, 1.0);
            }
            ENDHLSL
        }
       UsePass "Universal Render Pipeline/Lit/ShadowCaster"
       UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"
    }
    FallBack "Universal Render Pipeline/Lit"
}